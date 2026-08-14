using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReunionesDeAmigos.Application.DTOs.LugaresExternos;
using ReunionesDeAmigos.Application.Interfaces.Services;
using ReunionesDeAmigos.Domain.Enums;

namespace ReunionesDeAmigos.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/lugares/externos")]
public sealed class LugaresExternosController(
    ILugarExternoService lugarExternoService)
    : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyCollection<LugarExternoDto>>(
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    public async Task<ActionResult<IReadOnlyCollection<LugarExternoDto>>> BuscarAsync(
        [FromQuery] Guid ciudadId,
        [FromQuery] TipoLugar? tipo,
        [FromQuery] string? barrio,
        [FromQuery] string? texto,
        [FromQuery] string? idioma,
        [FromQuery] int? cantidad,
        CancellationToken cancellationToken)
    {
        var lugares = await lugarExternoService.BuscarAsync(
            new BuscarLugaresExternosRequest(
                ciudadId,
                tipo,
                barrio,
                texto,
                idioma,
                cantidad),
            cancellationToken);

        return Ok(lugares);
    }
}
