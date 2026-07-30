# Modelo de dominio

## Objetivo

La aplicación permite organizar encuentros entre amigos.

Un usuario registrado crea una salida y comparte un enlace o su código general
de acceso por WhatsApp u otro medio. Quienes ingresan con una cuenta o como
invitados se convierten en participantes, agregan propuestas y votan una vez
finalizado el período de propuestas.

El lugar elegido puede pertenecer al catálogo público o ser una opción manual,
como una casa particular o pedir comida.

## Modelo general

```text
Usuario
  ├── crea muchas Salidas
  └── puede participar en muchas Salidas

Salida
  ├── tiene muchos Participantes
  └── tiene muchas Propuestas

ParticipanteSalida
  ├── puede estar asociado a un Usuario
  ├── crea Propuestas
  └── emite como máximo un Voto

Propuesta
  ├── puede hacer referencia a un Lugar
  └── recibe muchos Votos

Lugar
  └── puede utilizarse en muchas Propuestas
```

Las propuestas y los votos se relacionan con `ParticipanteSalida`, no
directamente con `Usuario`. De esta manera, un usuario registrado y un invitado
pueden realizar las mismas acciones dentro de una salida.

## Usuario

Representa una cuenta permanente.

Datos iniciales:

- Identificador.
- Nombre.
- Email único.
- Fecha de creación.
- Estado activo o inactivo.

La contraseña y los detalles de autenticación se resolverán con el mecanismo de
identidad de la infraestructura. El dominio no almacenará contraseñas en texto
plano.

Solo un usuario registrado puede crear una salida.

## Salida

Representa el encuentro que se está organizando, no el lugar elegido.

Datos iniciales:

- Identificador.
- Nombre.
- Descripción opcional.
- Fecha del encuentro.
- Fin del período de propuestas.
- Fin de la votación.
- Código general de acceso único.
- Creador.
- Fecha de creación.
- Indicador o fecha de cancelación.

Todas las fechas se representan con `DateTimeOffset` y se almacenan en UTC.

Al crear una salida:

1. Se asigna el usuario creador.
2. Se agrega al creador como participante registrado.
3. Se genera un código general de acceso único.
4. La salida comienza recibiendo propuestas.

La aplicación construye un enlace compartible a partir del código:

```text
https://dominio-aplicacion/salidas/{codigo}
```

El enlace y el ingreso manual del código representan el mismo acceso general.
No se crean invitaciones individuales ni se registran anticipadamente los
nombres de las personas invitadas.

La salida comienza con un único participante: el creador. La colección aumenta
a medida que otras personas ingresan.

Las fechas deben cumplir:

```text
fecha de creación
    < fin de propuestas
    < fin de votación
    < fecha del encuentro
```

### Estados

El estado temporal se calcula a partir de las fechas:

```text
Antes del fin de propuestas -> RecibiendoPropuestas
Antes del fin de votación   -> VotacionAbierta
Después del fin de votación y antes del encuentro -> Confirmada
```

Una salida cancelada siempre tiene estado `Cancelada`, independientemente de
sus fechas.

`Finalizada` podrá representar una salida cuya fecha de encuentro ya pasó. No
necesita comportamiento especial en la primera versión.

### Modificaciones

Mientras la votación no haya terminado, el creador puede modificar:

- Nombre.
- Descripción.
- Fecha del encuentro.

El fin de propuestas solo puede modificarse mientras se reciben propuestas. El
fin de votación puede modificarse antes de su vencimiento.

Toda modificación debe conservar el orden de las fechas y no puede mover un
plazo al pasado.

Solo el creador puede cancelar una salida. La cancelación conserva la
información existente, pero impide nuevos participantes, propuestas y votos.

## ParticipanteSalida

Representa a una persona dentro de una salida concreta.

Datos iniciales:

- Identificador.
- Salida.
- Usuario opcional.
- Nombre visible.
- Fecha de ingreso.
- Hash de la credencial privada de acceso para invitados.

Un participante puede ser:

- Registrado: tiene un `UsuarioId`.
- Invitado: no tiene usuario y proporciona un nombre visible.

El creador siempre es un participante registrado.

Reglas:

- Un usuario registrado solo puede participar una vez en una salida.
- Los nombres visibles pueden repetirse porque no identifican al participante.
- Un invitado recibe una credencial privada para volver a ingresar.
- El código general o el enlace compartido permiten encontrar la salida.
- Al ingresar con una cuenta, la identidad se obtiene del usuario autenticado.
- Al ingresar sin cuenta, la API entrega una credencial privada y conserva una
  representación segura para validarla posteriormente.
- La credencial privada identifica al invitado después de haber ingresado y no
  debe compartirse.
- En la primera versión, un participante no puede abandonar ni ser expulsado.
- Se permite ingresar durante todo el período de propuestas.
- Se permite ingresar durante todo el período de votación, incluso pocos
  minutos antes de su finalización.
- Quien ingresa durante la votación puede votar, pero no agregar propuestas.
- Votar no es obligatorio. Quien no vote sigue siendo participante y puede
  consultar el resultado y la información final de la salida.
- No se permite ingresar cuando la salida está confirmada, finalizada o
  cancelada.

La aplicación recibe el valor original de la credencial una sola vez y lo
guarda en el almacenamiento seguro del dispositivo. Esto permite que el
invitado vuelva a entrar sin crear otro participante.

## Lugar

Representa un lugar permanente del catálogo público y existe
independientemente de las salidas.

Datos iniciales:

- Identificador.
- Nombre.
- Descripción opcional.
- Dirección.
- Barrio.
- Ciudad.
- Tipo de lugar.
- Latitud y longitud opcionales.
- Estado activo o inactivo.

Tipos iniciales:

- Restaurante.
- Bar.
- Café.
- Parrilla.
- Pizzería.
- Cervecería.
- Otro.

Un lugar inactivo permanece en el historial, pero no puede seleccionarse para
crear propuestas nuevas.

La búsqueda y consulta del catálogo son públicas. La creación, modificación y
desactivación de lugares requieren un usuario con rol administrador.

Inicialmente podrán cargarse lugares mediante datos precargados o endpoints
administrativos protegidos. Más adelante podrá incorporarse una interfaz
interna de administración. La carga habitual no se realizará modificando
PostgreSQL directamente, para conservar validaciones y trazabilidad.

## Propuesta

Representa una opción dentro de una única salida.

Datos iniciales:

- Identificador.
- Salida.
- Participante que la creó.
- Tipo de propuesta.
- Lugar opcional.
- Nombre manual opcional.
- Descripción manual opcional.
- Dirección manual opcional.
- Fecha de creación.

Existen dos tipos:

### Propuesta de catálogo

- `LugarId` es obligatorio.
- El nombre y la dirección visibles se obtienen del lugar.
- Los campos manuales no se utilizan.

### Propuesta manual

- `LugarId` no está presente.
- El nombre manual es obligatorio.
- La descripción y la dirección son opcionales.
- No se incorpora automáticamente al catálogo.

Reglas:

- Solo un participante de la salida puede crear propuestas.
- Solo se crean mientras la salida está recibiendo propuestas.
- No se crean propuestas en una salida cancelada.
- Una propuesta pertenece únicamente a la salida donde fue creada.
- Un mismo lugar del catálogo no puede proponerse más de una vez en la misma
  salida.
- Dos propuestas manuales no pueden tener el mismo nombre después de quitar
  espacios sobrantes e ignorar diferencias entre mayúsculas y minúsculas.
- No se intentará detectar si nombres diferentes representan conceptualmente la
  misma opción.
- En la primera versión, las propuestas no se editan ni eliminan.

## Voto

Representa la elección de un participante.

Datos iniciales:

- Identificador.
- Participante.
- Propuesta elegida.
- Fecha de creación.
- Fecha de última modificación.

Reglas:

- Solo puede votar un participante de la salida.
- La propuesta debe pertenecer a esa misma salida.
- Solo se vota durante `VotacionAbierta`.
- Cada participante tiene como máximo un voto por salida.
- Un participante puede votar una propuesta propia o la de otro participante.
- Mientras la votación está abierta puede cambiar su elección.
- Cambiar la elección actualiza el voto existente; no crea otro.
- No se muestran resultados parciales durante la votación.

## Resultado

Después del cierre se cuentan los votos de cada propuesta.

- Una única propuesta con más votos es la ganadora.
- Si varias propuestas comparten el máximo, se informa un empate.
- Si no hay votos, el resultado no tiene ganador.

No habrá desempate automático en la primera versión.

## Acceso y seguridad inicial

La seguridad será proporcional al tipo de aplicación, pero se aplicarán buenas
prácticas desde el comienzo:

- Todo acceso a la API utilizará HTTPS fuera del entorno local.
- El código general será aleatorio, único y suficientemente difícil de adivinar.
- La API limitará intentos repetidos de búsqueda por código.
- Los usuarios registrados se autenticarán con mecanismos estándar.
- Las credenciales de invitados serán largas, aleatorias y no se almacenarán en
  texto plano.
- En el MVP, los invitados utilizarán directamente esa credencial para
  identificarse en solicitudes posteriores. No se emitirán JWT para invitados.
- La API calculará el hash de la credencial recibida y lo comparará con el valor
  almacenado para identificar al participante.
- Los secretos y cadenas de conexión se configurarán mediante variables de
  entorno y no se subirán al repositorio.
- Los códigos y credenciales no se escribirán completos en los registros de la
  aplicación.

El código general es compartible y permite solicitar el ingreso a la salida. No
reemplaza la identidad del participante: las acciones posteriores se autorizan
con la cuenta registrada o con la credencial privada del invitado.

## Restricciones que también protegerá la base de datos

- Email de usuario único.
- Código general de acceso único.
- Credencial privada de invitado única.
- Un usuario registrado no puede participar dos veces en la misma salida.
- Un participante tiene como máximo un voto.
- Las claves foráneas conservan las relaciones entre las entidades.
- Una propuesta es manual o de catálogo, pero no ambas.

Las reglas que dependen del estado, las fechas o la pertenencia a una misma
salida también se validarán en el dominio y en los casos de uso.

## Fuera del alcance inicial

- Grupos reutilizables.
- Notificaciones.
- Desempate automático.
- Resultados parciales.
- Reapertura de propuestas o votaciones.
- Expulsión o abandono de participantes.
- Edición o eliminación de propuestas.
- Eliminación definitiva de salidas.
- Administración completa del catálogo.
- Invitaciones individuales con seguimiento de aceptación o rechazo.

Estas decisiones pueden modificarse a medida que se pruebe la aplicación. Este
documento describe el punto de partida, no un contrato inmutable.
