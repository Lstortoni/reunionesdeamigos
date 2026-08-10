using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using ReunionesDeAmigos.Application.DTOs.Salidas;
using ReunionesDeAmigos.Application.Interfaces.Services;

namespace ReunionesDeAmigos.Api.Controllers;

[ApiController]
[Route("api/salidas")]
[Authorize]
public sealed class SalidasController(ISalidaService salidaService)
    : ControllerBase
{
    [HttpGet("mias")]
    [ProducesResponseType<IReadOnlyCollection<SalidaResumenDto>>(
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyCollection<SalidaResumenDto>>>
        ObtenerMiasAsync(CancellationToken cancellationToken)
    {
        if (!TryObtenerUsuarioId(out var usuarioId))
        {
            return Unauthorized();
        }

        var salidas = await salidaService.ObtenerMiasAsync(
            usuarioId,
            cancellationToken);

        return Ok(salidas);
    }

    [HttpPost]
    [ProducesResponseType<SalidaDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<SalidaDto>> CrearAsync(
        [FromBody] CrearSalidaRequest request,
        CancellationToken cancellationToken)
    {
        if (!TryObtenerUsuarioId(out var usuarioId))
        {
            return Unauthorized();
        }

        var salida = await salidaService.CrearAsync(
            request,
            usuarioId,
            cancellationToken);

        return Created(
            $"/api/salidas/{salida.Id}",
            salida);
    }

    [HttpGet("{salidaId:guid}")]
    [ProducesResponseType<SalidaDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SalidaDto>> ObtenerPorIdAsync(
        Guid salidaId,
        CancellationToken cancellationToken)
    {
        if (!TryObtenerUsuarioId(out var usuarioId))
        {
            return Unauthorized();
        }

        var salida = await salidaService.ObtenerPorIdAsync(
            salidaId,
            usuarioId,
            cancellationToken);

        return Ok(salida);
    }

    private bool TryObtenerUsuarioId(out Guid usuarioId)
    {
        var usuarioIdClaim = User.FindFirst(
            JwtRegisteredClaimNames.Sub)?.Value;

        return Guid.TryParse(usuarioIdClaim, out usuarioId);
    }
}
