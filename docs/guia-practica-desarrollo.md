# Guía práctica de desarrollo

Esta guía reúne los comandos habituales para levantar, probar y mantener el proyecto. Está pensada como referencia práctica: indica desde qué carpeta ejecutar cada comando, qué hace y cuándo utilizarlo.

> Los valores de contraseñas, hashes y tokens incluidos en los ejemplos son ficticios. No se deben guardar credenciales reales en este documento.

## 1. Estructura general

```text
reunionesdeamigos/
├── Api/                     Solución y proyectos del backend
├── App/                     Aplicación .NET MAUI Blazor Hybrid
├── docs/                    Documentación
├── scripts/                 Scripts manuales de datos
├── compose.yaml             PostgreSQL y API para desarrollo
└── Dockerfile.postgres      Imagen de PostgreSQL
```

Los comandos de Docker Compose se ejecutan desde la raíz `reunionesdeamigos`. Los comandos de Entity Framework se ejecutan desde `reunionesdeamigos/Api`. Los comandos de la aplicación móvil pueden ejecutarse desde `reunionesdeamigos/App`.

## 2. Docker Compose

### A — Validar la configuración

Desde la raíz:

```powershell
docker compose config
```

Valida `compose.yaml` y muestra la configuración resultante. No crea imágenes ni contenedores.

### B — Construir las imágenes

Solo PostgreSQL:

```powershell
docker compose build postgres
```

Solo la API:

```powershell
docker compose build api
```

Todos los servicios:

```powershell
docker compose build
```

`postgres` y `api` son los nombres de los servicios declarados dentro de `compose.yaml`.

### C — Crear e iniciar los contenedores

Solo PostgreSQL:

```powershell
docker compose up -d postgres
```

PostgreSQL y API:

```powershell
docker compose up -d
```

`up` crea el contenedor si todavía no existe y lo inicia. `-d` lo deja ejecutándose en segundo plano.

En este proyecto:

- PostgreSQL queda accesible desde Windows en `localhost:5433`.
- La API queda accesible desde Windows en `http://localhost:5080`.
- Dentro de Docker, la API se conecta a PostgreSQL usando `postgres:5432`.

### D — Consultar el estado

```powershell
docker compose ps
```

Permite comprobar si los servicios están `running` y si PostgreSQL está `healthy`.

### E — Consultar logs

PostgreSQL:

```powershell
docker compose logs postgres
```

API:

```powershell
docker compose logs api
```

Seguir los logs en tiempo real:

```powershell
docker compose logs -f api
```

Se sale del seguimiento con `Ctrl + C`. Esto no detiene el contenedor.

### F — Detener y volver a iniciar

Detener sin eliminar:

```powershell
docker compose stop
```

Volver a iniciar los mismos contenedores:

```powershell
docker compose start
```

### G — Eliminar los contenedores conservando la base

```powershell
docker compose down
```

Elimina los contenedores y la red de Compose, pero conserva el volumen de PostgreSQL y sus datos.

### H — Reconstruir después de cambiar el código o un Dockerfile

Solo la API:

```powershell
docker compose up -d --build api
```

Todos los servicios:

```powershell
docker compose up -d --build
```

La aplicación que ya está dentro de un contenedor no cambia automáticamente al editar código. Hay que reconstruir su imagen y recrear el contenedor.

### I — Borrar también la base de datos

```powershell
docker compose down -v
```

**Cuidado:** elimina los contenedores y el volumen. Se pierden definitivamente la base y todos sus datos. Solo debe utilizarse para comenzar desde cero.

## 3. Conexión a PostgreSQL desde DBeaver

Con el contenedor de PostgreSQL iniciado:

```text
Host: localhost
Port: 5433
Database: reuniones_de_amigos
Username: reuniones
Password: consultar POSTGRES_PASSWORD en compose.yaml
```

El puerto es `5433` porque `compose.yaml` publica `5433:5432`: Windows utiliza 5433 y PostgreSQL continúa usando 5432 dentro del contenedor.

## 4. Herramienta dotnet-ef

`dotnet-ef` es la herramienta de consola de Entity Framework Core. Durante el desarrollo permite inspeccionar el `DbContext`, crear migraciones y aplicarlas a PostgreSQL.

La herramienta local está registrada en:

```text
Api/.config/dotnet-tools.json
```

Esto fija la versión utilizada por el repositorio. Después de clonar el proyecto, se restaura desde la carpeta `Api` con:

```powershell
dotnet tool restore
```

Verificar la versión:

```powershell
dotnet ef --version
```

La instalación inicial que ya se realizó fue:

```powershell
dotnet new tool-manifest
dotnet tool install dotnet-ef --version 8.0.8
```

No es necesario repetirla mientras exista el manifiesto. En otra computadora se utiliza `dotnet tool restore`.

## 5. Entity Framework y migraciones

Los siguientes comandos se ejecutan desde la carpeta `Api`.

### A — Comprobar que Entity Framework encuentra el DbContext

```powershell
dotnet ef dbcontext info --project src/ReunionesDeAmigos.Infrastructure --startup-project src/ReunionesDeAmigos.Api
```

- `--project`: proyecto que contiene `AppDbContext`, configuraciones y migraciones.
- `--startup-project`: proyecto ejecutable que proporciona `Program.cs`, configuración, conexión y registro de dependencias.

La separación es necesaria porque `Infrastructure` es una biblioteca y no puede arrancarse sola.

### B — Crear una migración

```powershell
dotnet ef migrations add NombreDeLaMigracion --project src/ReunionesDeAmigos.Infrastructure --startup-project src/ReunionesDeAmigos.Api --output-dir Persistence/Migrations
```

Ejemplo:

```powershell
dotnet ef migrations add AgregarTelefonoUsuario --project src/ReunionesDeAmigos.Infrastructure --startup-project src/ReunionesDeAmigos.Api --output-dir Persistence/Migrations
```

- `migrations add`: crea una migración; todavía no modifica la base.
- `NombreDeLaMigracion`: describe el cambio realizado.
- `--output-dir`: guarda los archivos en `Infrastructure/Persistence/Migrations`.

Después de crearla hay que revisar el método `Up`, que aplica el cambio, y el método `Down`, que intenta revertirlo.

### C — Aplicar las migraciones pendientes

```powershell
dotnet ef database update --project src/ReunionesDeAmigos.Infrastructure --startup-project src/ReunionesDeAmigos.Api
```

Este comando sí modifica la base. Entity Framework consulta `__EFMigrationsHistory` y aplica solamente las migraciones pendientes.

Flujo habitual:

```text
Modificar entidad o configuración de EF
→ crear una migración
→ revisar los archivos generados
→ ejecutar database update
```

### D — Ver las migraciones

```powershell
dotnet ef migrations list --project src/ReunionesDeAmigos.Infrastructure --startup-project src/ReunionesDeAmigos.Api
```

## 6. API en Docker

Levantar PostgreSQL y API:

```powershell
docker compose up -d --build
```

Verificar:

```powershell
docker compose ps
docker compose logs api
```

Cuando se modifica únicamente el código de la API:

```powershell
docker compose up -d --build api
```

## 7. Pruebas básicas de autenticación

La API en Docker se prueba en `http://localhost:5080`.

### A — Registrar un usuario

```http
POST http://localhost:5080/api/auth/registrar
Content-Type: application/json
```

```json
{
  "nombre": "Usuario de prueba",
  "email": "usuario@example.com",
  "password": "ContraseñaDePrueba123"
}
```

La respuesta incluye el usuario, un `accessToken` JWT y su vencimiento.

### B — Iniciar sesión

```http
POST http://localhost:5080/api/auth/login
Content-Type: application/json
```

```json
{
  "email": "usuario@example.com",
  "password": "ContraseñaDePrueba123"
}
```

### C — Consultar el usuario autenticado

```http
GET http://localhost:5080/api/usuarios/me
Authorization: Bearer <ACCESS_TOKEN>
```

Si el token es válido devuelve `200 OK` y el usuario. Si falta, está vencido o es inválido devuelve `401 Unauthorized`.

### D — Consultar mis salidas

```http
GET http://localhost:5080/api/salidas/mias
Authorization: Bearer <ACCESS_TOKEN>
```

## 8. Aplicación Android

### A — Compilar sin instalar

Desde la raíz:

```powershell
dotnet build App/ReunionesDeAmigos.App/ReunionesDeAmigos.App.csproj -f net8.0-android
```

Desde la carpeta `App`:

```powershell
dotnet build ReunionesDeAmigos.App/ReunionesDeAmigos.App.csproj -f net8.0-android
```

### B — Compilar, instalar y ejecutar en el emulador

Con un emulador iniciado, desde la carpeta `App`:

```powershell
dotnet build ReunionesDeAmigos.App/ReunionesDeAmigos.App.csproj -t:Run -f net8.0-android
```

`-f net8.0-android` elige Android. `-t:Run` compila, instala la aplicación en el dispositivo disponible y la ejecuta.

### C — Limpiar archivos compilados

```powershell
dotnet clean ReunionesDeAmigos.App/ReunionesDeAmigos.App.csproj -f net8.0-android
```

`clean` elimina resultados locales de compilaciones anteriores. No desinstala la aplicación del emulador.

### D — Desinstalar la aplicación mediante ADB

```powershell
& "$env:LOCALAPPDATA\Android\Sdk\platform-tools\adb.exe" uninstall com.companyname.reunionesdeamigos.app
```

Si funciona devuelve `Success`.

## 9. Comunicación entre app, API y base

```text
App en emulador Android
    │ http://10.0.2.2:5080
    ▼
API publicada por Docker en Windows
    │ postgres:5432 (red interna de Docker)
    ▼
PostgreSQL
```

En el emulador Android, `localhost` representa al propio emulador. La dirección especial `10.0.2.2` permite acceder al `localhost` de Windows. Por eso la app usa `http://10.0.2.2:5080`, mientras Postman usa `http://localhost:5080`.

## 10. Secuencia habitual para comenzar a trabajar

Desde la raíz del repositorio:

```powershell
docker compose up -d
docker compose ps
```

Si cambió el código de la API:

```powershell
docker compose up -d --build api
```

Después, con el emulador iniciado y desde `App`:

```powershell
dotnet build ReunionesDeAmigos.App/ReunionesDeAmigos.App.csproj -t:Run -f net8.0-android
```

Al terminar de trabajar, si se desea detener todo conservando los datos:

```powershell
docker compose stop
```
