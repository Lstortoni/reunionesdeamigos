using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReunionesDeAmigos.Application.DTOs.Ciudades;
using ReunionesDeAmigos.Application.Interfaces.Services;

namespace ReunionesDeAmigos.Api.Controllers;

[ApiController]
[Route("api/ciudades")]
[AllowAnonymous]
public sealed class CiudadesController(ICiudadService ciudadService)
    : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyCollection<CiudadDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<CiudadDto>>> ObtenerAsync(
        CancellationToken cancellationToken)
    {
        var ciudades = await ciudadService.ObtenerActivasAsync(
            cancellationToken);

        return Ok(ciudades);
    }
}
