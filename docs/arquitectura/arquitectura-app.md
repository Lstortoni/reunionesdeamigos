# Arquitectura inicial de la app

## Tecnología

La aplicación es .NET MAUI Blazor Hybrid. MAUI crea la aplicación nativa y un
`BlazorWebView` muestra las pantallas implementadas como componentes Razor.

```text
MainPage.xaml
  -> BlazorWebView
     -> Components/Routes.razor
        -> Components/Pages/*.razor
```

## Primer flujo implementado

```text
Login.razor
  -> IAuthApiService
     -> POST /api/auth/login
        -> AutenticacionDto
           -> ISessionService
              -> SecureStorage guarda el JWT
                 -> navegación a /inicio
```

## Carpetas

- `Components/Pages`: pantallas y su comportamiento visual.
- `Models/Auth`: contratos que la app intercambia con autenticación de la API.
- `Models/Api`: formato de errores HTTP recibido desde la API.
- `Services`: comunicación HTTP y estado de la sesión.
- `wwwroot/css`: estilos de las pantallas Blazor.
- `Platforms/Android`: configuración exclusiva de Android.

La pantalla no crea directamente un `HttpClient` ni accede directamente a
`SecureStorage`; utiliza interfaces inyectadas para mantener separadas la vista,
la comunicación y la sesión.

## Dirección de desarrollo

`MauiProgram.cs` configura la base de la API según la plataforma:

```text
Android Emulator -> http://10.0.2.2:5080/
Windows          -> http://localhost:5080/
```

En Android, `10.0.2.2` representa la computadora anfitriona. `localhost` dentro
del emulador representaría al propio dispositivo virtual.

Durante el desarrollo Android permite HTTP mediante `usesCleartextTraffic`.
Esta excepción es temporal; una distribución real deberá utilizar HTTPS.

## Sesión inicial

`SessionService` guarda el `AccessToken` en `SecureStorage`, no en preferencias
comunes ni en archivos de texto. El usuario actual se conserva inicialmente en
memoria para mostrar la pantalla de bienvenida.

Todavía falta restaurar automáticamente la sesión al reiniciar la app y agregar
el JWT a las solicitudes protegidas. Esto se incorporará al implementar la
pantalla `Mis salidas`.
