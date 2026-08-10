using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using ReunionesDeAmigos.Application.DTOs.Usuarios;
using ReunionesDeAmigos.Application.Interfaces.Services;

namespace ReunionesDeAmigos.Api.Controllers;

[ApiController]
[Route("api/usuarios")]
[Authorize]
public sealed class UsuariosController(IUsuarioService usuarioService)
    : ControllerBase
{
    [HttpGet("me")]
    [ProducesResponseType<UsuarioDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UsuarioDto>> ObtenerActualAsync(
        CancellationToken cancellationToken)
    {
        var usuarioIdClaim = User.FindFirst(
            JwtRegisteredClaimNames.Sub)?.Value;

        if (!Guid.TryParse(usuarioIdClaim, out var usuarioId))
        {
            return Unauthorized();
        }

        var usuario = await usuarioService.ObtenerPorIdAsync(
            usuarioId,
            cancellationToken);

        return Ok(usuario);
    }
}
