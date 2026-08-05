# Arquitectura del backend

## Objetivo

Este documento explica cómo organizamos el backend, qué responsabilidad tiene
cada parte y cómo circula una operación desde la API hasta la base de datos.

La intención es mantener el proyecto comprensible y evitar que controladores,
reglas del negocio y acceso a datos terminen mezclados.

## Solución

El backend se divide en cuatro proyectos:

```text
Api/
├── ReunionesDeAmigos.sln
└── src/
    ├── ReunionesDeAmigos.Domain/
    ├── ReunionesDeAmigos.Application/
    ├── ReunionesDeAmigos.Infrastructure/
    └── ReunionesDeAmigos.Api/
```

Son proyectos `.NET` diferentes, no solamente carpetas. Esta separación permite
que el compilador controle sus dependencias.

## Dirección de las dependencias

```text
Api ───────────────► Application ─────────► Domain
 │                         ▲
 └──► Infrastructure ──────┘
              │
              └────────────────────────────► Domain
```

Reglas:

- `Domain` no referencia ningún otro proyecto.
- `Application` referencia únicamente `Domain`.
- `Infrastructure` referencia `Application` y `Domain`.
- `Api` referencia `Application` e `Infrastructure`.

`Domain` no conoce ASP.NET, Entity Framework, PostgreSQL, Docker ni HTTP.

## Domain

`Domain` representa el negocio de la aplicación.

Contiene:

- Entidades.
- Enums.
- Reglas propias del negocio.
- Resultados del dominio.
- Excepciones de dominio.

Ejemplos:

```text
Usuario
Salida
ParticipanteSalida
Lugar
Propuesta
Voto
```

### DDD utilizado en el proyecto

DDD significa `Domain-Driven Design`, o diseño guiado por el dominio.

En este proyecto lo aplicamos de manera práctica: las reglas que determinan
cómo puede cambiar una entidad viven dentro de esa entidad.

Por ejemplo:

```csharp
salida.AgregarParticipanteRegistrado(usuario, fechaActual);
salida.AgregarPropuestaManual(...);
salida.RegistrarVoto(...);
salida.Cancelar(...);
```

`Salida` controla que:

- El usuario no participe dos veces.
- La etapa permita ingresar, proponer o votar.
- Una propuesta pertenezca a la salida.
- Un participante tenga un solo voto.
- Los plazos mantengan un orden válido.

La entidad no consulta la base, no envía mensajes y no genera respuestas HTTP.

## Application

`Application` contiene los casos de uso que ofrece la aplicación.

Su estructura inicial es:

```text
Application/
├── DTOs/
├── Interfaces/
│   ├── Repositories/
│   └── Services/
└── Services/
```

### Servicios de aplicación

Un servicio coordina los pasos de un caso de uso.

Ejemplo conceptual:

```csharp
public async Task AgregarParticipanteAsync(...)
{
    var salida = await _salidaRepository.ObtenerPorIdAsync(...);
    var usuario = await _usuarioRepository.ObtenerPorIdAsync(...);

    var participante = salida.AgregarParticipanteRegistrado(
        usuario,
        _clock.UtcNow);

    await _unitOfWork.SaveChangesAsync(...);

    return Mapear(participante);
}
```

El servicio:

1. Obtiene información mediante repositorios.
2. Coordina dependencias.
3. Llama a la entidad para aplicar reglas.
4. Confirma los cambios.
5. Devuelve un DTO.

El servicio no contiene consultas de Entity Framework ni responde directamente
una solicitud HTTP.

### Interfaces de servicios

Las interfaces describen los casos de uso disponibles:

```text
ISalidaService
IParticipanteSalidaService
IPropuestaService
IVotoService
ILugarService
```

La API dependerá de estas interfaces y no de implementaciones concretas.

### DTOs

Los DTOs transportan datos entre la API y Application.

Ejemplos:

```text
CrearSalidaRequest
SalidaDto
LugarDto
PropuestaDto
VotoDto
```

Un DTO no contiene reglas del negocio. Tampoco es una entidad que Entity
Framework deba guardar.

## Repositorios

Las interfaces de repositorios están en `Application`:

```text
ISalidaRepository
IUsuarioRepository
ILugarRepository
```

Sus implementaciones con Entity Framework estarán en `Infrastructure`.

El repositorio se ocupa de:

- Consultar entidades.
- Agregar entidades al contexto.
- Preparar cambios para su persistencia.

El repositorio no debería decidir reglas como quién puede votar o cuándo puede
agregarse una propuesta.

### Por qué no hay un repositorio por tabla

Los repositorios se definen para las entidades principales, no automáticamente
para cada tabla.

```text
Salida
├── ParticipanteSalida
├── Propuesta
└── Voto
```

`ParticipanteSalida`, `Propuesta` y `Voto` se administran mediante `Salida`.

Ejemplo:

```csharp
var salida = await _salidaRepository.ObtenerPorIdAsync(...);

salida.RegistrarVoto(...);

await _unitOfWork.SaveChangesAsync(...);
```

No necesitamos inicialmente un `IVotoRepository` para agregar el voto
directamente.

`Lugar` sí tiene repositorio porque existe como catálogo independiente y puede
consultarse sin crear una salida.

## Infrastructure

`Infrastructure` contendrá las implementaciones técnicas:

```text
Infrastructure/
├── Persistence/
│   ├── AppDbContext.cs
│   ├── Configurations/
│   └── Migrations/
├── Repositories/
├── Identity/
└── Services/
```

Aquí estarán:

- Entity Framework Core.
- PostgreSQL.
- Implementaciones de repositorios.
- Generación segura de códigos y credenciales.
- Autenticación.
- Servicios externos.

Infrastructure puede depender de Application porque implementa sus interfaces.

## Autenticación inicial

Los usuarios registrados utilizarán email y contraseña. La contraseña original
entrará únicamente en el caso de uso de registro o inicio de sesión y nunca se
guardará ni se incluirá en un DTO de respuesta.

```text
Registro
    ↓
Application valida la solicitud
    ↓
IPasswordHasher
    ↓
Infrastructure genera PasswordHash
    ↓
Usuario y PostgreSQL guardan solamente PasswordHash
```

Domain no conocerá el algoritmo de hash. Recibirá el valor ya generado para
proteger la creación de la entidad. La generación y verificación se expondrán a
Application mediante `IPasswordHasher` y se implementarán en Infrastructure.

El inicio de sesión buscará el usuario por email, verificará la contraseña y
utilizará `IAccessTokenGenerator` para emitir un JWT válido inicialmente durante
60 minutos. El token incluirá el `UsuarioId` y la API lo obtendrá desde la
identidad autenticada, en lugar de aceptar identificadores de usuario libres en
operaciones protegidas.

Como el MVP todavía no exige confirmación de email, tanto el registro exitoso
como el inicio de sesión devolverán un `AutenticacionDto` con el usuario, el JWT
y su fecha de vencimiento. De esta manera, una cuenta recién creada queda
autenticada sin realizar una segunda solicitud de login.

```text
Login
    ↓
Email + contraseña
    ↓
Verificación del PasswordHash
    ↓
JWT con UsuarioId
    ↓
Authorization: Bearer <token>
```

Los invitados conservarán un mecanismo separado: código general para encontrar
la salida y credencial privada para acreditar sus acciones posteriores. En el
MVP no recibirán un JWT.

Quedan fuera del alcance inicial los refresh tokens, confirmación de email,
recuperación de contraseña, doble factor y proveedores externos. Estas mejoras
se incorporarán después del primer flujo autenticado completo.

## Unit of Work

`IUnitOfWork` permite que Application confirme cambios sin conocer
`AppDbContext`.

Application llama:

```csharp
await _unitOfWork.SaveChangesAsync(cancellationToken);
```

La implementación real hará:

```csharp
return await dbContext.SaveChangesAsync(cancellationToken);
```

Es una forma indirecta de usar `DbContext.SaveChangesAsync()`.

Los repositorios preparan los cambios y el servicio decide cuándo confirmarlos:

```csharp
await _salidaRepository.AgregarAsync(salida, cancellationToken);
await _unitOfWork.SaveChangesAsync(cancellationToken);
```

## Api

`Api` será el punto de entrada HTTP.

Contendrá:

```text
Api/
├── Controllers/
├── Middleware/
├── Extensions/
└── Program.cs
```

Un controlador:

1. Recibe la solicitud HTTP.
2. Obtiene la identidad autenticada.
3. Llama a una interfaz de Application.
4. Convierte el resultado en una respuesta HTTP.

No consulta `AppDbContext` ni modifica entidades directamente.

Ejemplo:

```csharp
[HttpPost]
public async Task<ActionResult<SalidaDto>> Crear(
    CrearSalidaRequest request,
    CancellationToken cancellationToken)
{
    var salida = await _salidaService.CrearAsync(
        request,
        usuarioId,
        cancellationToken);

    return Ok(salida);
}
```

## Flujo completo de una escritura

Ejemplo: un usuario registrado ingresa a una salida.

```text
Controller
    │
    ▼
IParticipanteSalidaService
    │
    ▼
ParticipanteSalidaService
    ├── obtiene Salida con ISalidaRepository
    ├── obtiene Usuario con IUsuarioRepository
    │
    ▼
Salida.AgregarParticipanteRegistrado(...)
    ├── verifica el estado
    ├── evita duplicados
    └── agrega el participante a su colección
    │
    ▼
IUnitOfWork.SaveChangesAsync()
    │
    ▼
AppDbContext.SaveChangesAsync()
    │
    ▼
PostgreSQL
```

La entidad decide cómo cambia. El repositorio recupera o prepara la entidad y
`UnitOfWork` persiste cómo quedó.

## Flujo de una consulta

Una consulta simple no necesita ejecutar comportamiento del dominio:

```text
Controller
    ↓
ISalidaService.ObtenerPorIdAsync()
    ↓
ISalidaRepository
    ↓
Entity Framework
    ↓
SalidaDto
```

DDD tiene más relevancia en las operaciones que modifican estado y deben
proteger reglas. Una consulta puede ser principalmente acceso y transformación
de datos.

## Inyección de dependencias

Las clases dependen de interfaces:

```csharp
public SalidaService(
    ISalidaRepository salidaRepository,
    IUsuarioRepository usuarioRepository,
    IUnitOfWork unitOfWork,
    IClock clock)
{
}
```

La API configurará las implementaciones:

```text
ISalidaService    -> SalidaService
ISalidaRepository -> SalidaRepository
IUnitOfWork       -> UnitOfWork
IClock            -> SystemClock
```

Esto permite reemplazar una implementación sin cambiar el servicio.

## Pruebas y repositorios en memoria

Las interfaces permiten utilizar implementaciones en memoria:

```text
ISalidaRepository
├── SalidaRepository con PostgreSQL
└── SalidaRepositoryEnMemoria
```

Un servicio puede probarse con el repositorio en memoria:

```text
SalidaService
    ↓
ISalidaRepository
    ↓
SalidaRepositoryEnMemoria
```

Las reglas internas también pueden probarse directamente sobre `Salida`, sin
repositorios ni base de datos.

## Decisiones actuales

- Arquitectura limpia con cuatro proyectos.
- Reglas propias del negocio dentro de las entidades.
- Servicios para coordinar casos de uso.
- Interfaces para servicios y repositorios.
- Repositorios para `Usuario`, `Salida` y `Lugar`.
- Participantes, propuestas y votos administrados mediante `Salida`.
- `UnitOfWork` como abstracción de `SaveChangesAsync`.
- Persistencia real implementada con PostgreSQL y Entity Framework Core.

Estas decisiones pueden ajustarse si la implementación demuestra que otra
distribución resulta más clara.

## Estado implementado y verificado

La infraestructura real ya está configurada con PostgreSQL 17, Entity Framework
Core y migraciones. La base se ejecuta en Docker y conserva sus datos mediante
el volumen `reuniones_postgres_data`.

Las migraciones aplicadas son:

```text
InitialCreate
AgregarAutenticacionUsuarios
```

La autenticación inicial también está implementada:

- `PasswordHasher` genera y verifica hashes mediante ASP.NET Core Identity.
- La contraseña original nunca se guarda en PostgreSQL.
- `JwtAccessTokenGenerator` genera JWT firmados con vigencia de 60 minutos.
- La API valida emisor, destinatario, firma y vencimiento mediante JWT Bearer.
- El middleware central transforma las excepciones conocidas en respuestas HTTP.

### Endpoints de autenticación

`AuthController` ofrece inicialmente dos endpoints anónimos:

```http
POST /api/auth/registrar
POST /api/auth/login
```

Los dos devuelven un `AutenticacionDto` compuesto por:

```text
Usuario
AccessToken
ExpiraEn
```

El registro responde `201 Created` y el login exitoso responde `200 OK`. Ambos
flujos fueron comprobados con Postman contra la API y PostgreSQL ejecutados en
Docker.

Los futuros endpoints privados utilizarán `[Authorize]`. El cliente enviará el
JWT mediante:

```http
Authorization: Bearer <token>
```

Registro y login utilizan `[AllowAnonymous]` porque para obtener el primer token
el usuario todavía no puede estar autenticado.

## Ejecución local y con Docker

La configuración efectiva depende de dónde se ejecute la API.

### API ejecutada localmente

La API lee la conexión de `appsettings.Development.json`:

```text
API local -> localhost:5433 -> PostgreSQL en Docker
```

El puerto `5433` es el puerto publicado en Windows por el contenedor de
PostgreSQL.

### API ejecutada en Docker

Compose proporciona variables de entorno que tienen prioridad sobre los valores
de `appsettings.json` y `appsettings.Development.json`:

```text
API en Docker -> postgres:5432 -> PostgreSQL en Docker
```

Dentro de la red de Compose se utiliza `postgres`, que es el nombre del servicio,
y el puerto interno estándar `5432`. `localhost` dentro del contenedor de la API
representaría a ese mismo contenedor, no a PostgreSQL.

La publicación de puertos de la API es:

```yaml
ports:
  - "5080:8080"
```

Esto significa:

```text
localhost:5080 en Windows -> puerto 8080 del contenedor de la API
```

Swagger queda disponible en desarrollo en:

```text
http://localhost:5080/swagger
```

### Archivos Docker

- `Dockerfile.postgres` construye la imagen de PostgreSQL.
- `Api/src/ReunionesDeAmigos.Api/Dockerfile` restaura y publica los cuatro
  proyectos .NET mediante una construcción multietapa.
- `compose.yaml` define los servicios `postgres` y `api`.
- `api` espera a que el health check de PostgreSQL indique que la base está
  disponible.

Comandos habituales desde la raíz del repositorio:

```powershell
docker compose config
docker compose build api
docker compose up -d
docker compose ps
docker compose logs api
```

Las migraciones no se ejecutan automáticamente al iniciar el contenedor de la
API. Se administran explícitamente con las herramientas de Entity Framework.

## Siguiente paso

El siguiente caso pequeño será un endpoint protegido como
`GET /api/usuarios/me`. Permitirá comprobar tres escenarios antes de implementar
los controllers del negocio:

```text
Sin token      -> 401 Unauthorized
Token inválido -> 401 Unauthorized
Token válido   -> 200 OK
```

Los roles todavía no forman parte del modelo. Se definirán cuando exista un
caso concreto que los necesite, posiblemente la administración del catálogo de
lugares.
