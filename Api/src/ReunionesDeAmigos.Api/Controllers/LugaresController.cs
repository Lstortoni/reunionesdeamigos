using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReunionesDeAmigos.Application.DTOs.Lugares;
using ReunionesDeAmigos.Application.Interfaces.Services;
using ReunionesDeAmigos.Domain.Enums;

namespace ReunionesDeAmigos.Api.Controllers;

[ApiController]
[Route("api/lugares")]
[AllowAnonymous]
public sealed class LugaresController(ILugarService lugarService)
    : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyCollection<LugarDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<LugarDto>>> BuscarAsync(
        [FromQuery] string? texto,
        [FromQuery] TipoLugar? tipo,
        [FromQuery] string? barrio,
        [FromQuery] Guid? ciudadId,
        CancellationToken cancellationToken)
    {
        var lugares = await lugarService.BuscarAsync(
            new BuscarLugaresRequest(texto, tipo, barrio, ciudadId),
            cancellationToken);

        return Ok(lugares);
    }

    [HttpGet("{lugarId:guid}")]
    [ProducesResponseType<LugarDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LugarDto>> ObtenerPorIdAsync(
        Guid lugarId,
        CancellationToken cancellationToken)
    {
        var lugar = await lugarService.ObtenerPorIdAsync(
            lugarId,
            cancellationToken);

        return Ok(lugar);
    }
}
