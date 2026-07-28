# Modelo de dominio

## Objetivo

La aplicación permite organizar encuentros entre amigos.

Un usuario registrado crea una salida y comparte su código de invitación. Los
participantes ingresan con una cuenta o como invitados, agregan propuestas y
votan una vez finalizado el período de propuestas.

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
- Código de invitación único.
- Creador.
- Fecha de creación.
- Indicador o fecha de cancelación.

Todas las fechas se representan con `DateTimeOffset` y se almacenan en UTC.

Al crear una salida:

1. Se asigna el usuario creador.
2. Se agrega al creador como participante registrado.
3. Se genera un código de invitación único.
4. La salida comienza recibiendo propuestas.

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
Después del fin de votación -> VotacionCerrada
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
- Identificador privado de acceso para invitados.

Un participante puede ser:

- Registrado: tiene un `UsuarioId`.
- Invitado: no tiene usuario y proporciona un nombre visible.

El creador siempre es un participante registrado.

Reglas:

- Un usuario registrado solo puede participar una vez en una salida.
- Los nombres visibles pueden repetirse porque no identifican al participante.
- Un invitado recibe una credencial privada para volver a ingresar.
- El código de invitación identifica la salida; la credencial privada identifica
  al invitado.
- En la primera versión, un participante no puede abandonar ni ser expulsado.
- Se permite ingresar mientras la salida recibe propuestas o tiene la votación
  abierta.
- No se permite ingresar a una salida cancelada o con votación cerrada.

La credencial del invitado no se almacenará en texto plano. La aplicación
recibirá el valor original una sola vez y conservará una versión segura para
validaciones posteriores.

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

La administración y carga del catálogo quedan fuera del primer flujo
funcional. Inicialmente podrá utilizarse información precargada.

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
- Mientras la votación está abierta puede cambiar su elección.
- Cambiar la elección actualiza el voto existente; no crea otro.
- No se muestran resultados parciales durante la votación.

## Resultado

Después del cierre se cuentan los votos de cada propuesta.

- Una única propuesta con más votos es la ganadora.
- Si varias propuestas comparten el máximo, se informa un empate.
- Si no hay votos, el resultado no tiene ganador.

No habrá desempate automático en la primera versión.

## Restricciones que también protegerá la base de datos

- Email de usuario único.
- Código de invitación único.
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

Estas decisiones pueden modificarse a medida que se pruebe la aplicación. Este
documento describe el punto de partida, no un contrato inmutable.
