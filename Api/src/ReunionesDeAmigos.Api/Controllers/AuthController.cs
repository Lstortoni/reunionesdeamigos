using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReunionesDeAmigos.Application.DTOs.Auth;
using ReunionesDeAmigos.Application.DTOs.Usuarios;
using ReunionesDeAmigos.Application.Interfaces.Services;

namespace ReunionesDeAmigos.Api.Controllers;

[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public sealed class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("registrar")]
    [ProducesResponseType<AutenticacionDto>(StatusCodes.Status201Created)]
    public async Task<ActionResult<AutenticacionDto>> RegistrarAsync(
        [FromBody] CrearUsuarioRequest request,
        CancellationToken cancellationToken)
    {
        var autenticacion = await authService.RegistrarAsync(
            request,
            cancellationToken);

        return StatusCode(StatusCodes.Status201Created, autenticacion);
    }

    [HttpPost("login")]
    [ProducesResponseType<AutenticacionDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult<AutenticacionDto>> IniciarSesionAsync(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var autenticacion = await authService.IniciarSesionAsync(
            request,
            cancellationToken);

        return Ok(autenticacion);
    }
}
