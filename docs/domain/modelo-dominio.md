# Modelo de dominio

## Objetivo

La aplicación permite organizar encuentros entre amigos.

Un usuario registrado crea una salida y comparte un enlace o su código general
de acceso por WhatsApp u otro medio. Quienes ingresan con una cuenta o como
invitados se convierten en participantes, agregan propuestas y votan una vez
finalizado el período de propuestas.

El lugar elegido puede provenir de Google Places o ser una opción manual, como
una casa particular o pedir comida.

Al crear una salida se cargan exactamente tres propuestas iniciales. Pueden
combinar lugares externos con opciones manuales. El creador queda registrado
como participante y como autor de esas propuestas.

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

Ciudad
  └── contiene muchos Lugares
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
- Hash de contraseña.
- Fecha de creación.
- Estado activo o inactivo.

El usuario se registra con nombre, email y contraseña. La contraseña original
solo existe durante la solicitud: Infrastructure genera un hash seguro y
Application entrega únicamente ese hash al dominio para crear el usuario. Ni la
contraseña ni su hash se exponen en los DTO de respuesta.

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

## Ciudad

Representa una ciudad disponible para explorar el catálogo. Evita utilizar el
nombre de la ciudad como texto libre en cada lugar y permite filtrar mediante un
identificador estable.

Datos iniciales:

- Identificador.
- Nombre.
- Provincia o región.
- País.
- Estado activo o inactivo.

La combinación de país, provincia y nombre debe ser única. Inicialmente
provincia y país se mantienen como textos controlados; no son entidades
independientes.

Las migraciones modifican solamente la estructura geográfica. Las ciudades y
los lugares iniciales se cargarán mediante scripts separados ubicados en
`scripts/database`. Los scripts deben poder repetirse sin duplicar registros.

## Lugar

Representa el catálogo local creado durante la primera etapa. Después de
incorporar Google Places dejó de utilizarse para crear propuestas. Se conserva
temporalmente hasta decidir si se reutiliza o se elimina en una migración futura.

Datos iniciales:

- Identificador.
- Nombre.
- Descripción opcional.
- Dirección.
- Barrio.
- Ciudad asociada mediante `CiudadId`.
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

La falta de un lugar en el catálogo no impide organizar una salida: un
participante puede crear una propuesta manual que pertenezca solamente a esa
salida. En una etapa posterior podrá evaluarse una carga comunitaria moderada o
la integración con un proveedor externo de lugares.

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
- Identificador de Google Places opcional.
- Nombre manual opcional.
- Descripción manual opcional.
- Dirección manual opcional.
- Fecha de creación.

Existen dos tipos:

### Propuesta de lugar externo

- `GooglePlaceId` es obligatorio.
- El identificador puede almacenarse y se utiliza para consultar información
  actualizada a Google Places.
- Los campos manuales no se utilizan.

### Propuesta manual

- `GooglePlaceId` no está presente.
- El nombre manual es obligatorio.
- La descripción y la dirección son opcionales.
- No se incorpora automáticamente al catálogo.

Reglas:

- Solo un participante de la salida puede crear propuestas.
- Solo se crean mientras la salida está recibiendo propuestas.
- No se crean propuestas en una salida cancelada.
- Una propuesta pertenece únicamente a la salida donde fue creada.
- Un mismo `GooglePlaceId` no puede proponerse más de una vez en la misma
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
- Salida.
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
- Los usuarios registrados se autenticarán con email y contraseña.
- El email será único y la contraseña tendrá inicialmente un mínimo de ocho
  caracteres.
- Las contraseñas nunca se almacenarán ni registrarán en texto plano. Se
  guardará solamente un hash generado con un algoritmo específico para
  contraseñas.
- Un inicio de sesión válido entregará un JWT de acceso con una duración inicial
  de 60 minutos. El token identificará al usuario mediante su `UsuarioId`.
- Los endpoints protegidos obtendrán el usuario desde el JWT; no confiarán en un
  `usuarioId` enviado libremente por el cliente.
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
- Una propuesta es manual o externa, pero no ambas.

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
- Refresh tokens.
- Confirmación de email.
- Recuperación de contraseña.
- Autenticación de doble factor.
- Inicio de sesión con proveedores externos, como Google.

Estas decisiones pueden modificarse a medida que se pruebe la aplicación. Este
documento describe el punto de partida, no un contrato inmutable.
