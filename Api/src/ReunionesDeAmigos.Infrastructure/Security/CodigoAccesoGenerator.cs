using System.Security.Cryptography;
using ReunionesDeAmigos.Application.Interfaces.Services;

namespace ReunionesDeAmigos.Infrastructure.Security;

internal sealed class CodigoAccesoGenerator : ICodigoAccesoGenerator
{
    private const string Caracteres = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
    private const int Longitud = 8;

    public string Generar()
    {
        Span<char> codigoAleatorio = stackalloc char[Longitud];

        for (var posicion = 0; posicion < codigoAleatorio.Length; posicion++)
        {
            var indice = RandomNumberGenerator.GetInt32(Caracteres.Length);
            codigoAleatorio[posicion] = Caracteres[indice];
        }

        return $"SAL-{codigoAleatorio}";
    }
}
