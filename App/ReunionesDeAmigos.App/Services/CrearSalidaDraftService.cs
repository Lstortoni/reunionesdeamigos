using ReunionesDeAmigos.App.Models.Lugares;
using ReunionesDeAmigos.App.Models.Salidas;

namespace ReunionesDeAmigos.App.Services;

public sealed class CrearSalidaDraftService
{
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public DateTime Fecha { get; set; } = DateTime.Today.AddDays(7);
    public TimeSpan Hora { get; set; } = new(20, 0, 0);
    public int? DiasParaPropuestas { get; set; }
    public int? DiasParaVotar { get; set; }
    public List<PropuestaSalidaDraft> Propuestas { get; } = [];

    public bool EstaCompleto => Propuestas.Count == 3;

    public string? AgregarManual(
        string nombre,
        string? descripcion,
        string? direccion)
    {
        if (Propuestas.Count >= 3) return "Ya elegiste las tres opciones.";
        var normalizado = nombre.Trim();
        if (string.IsNullOrWhiteSpace(normalizado)) return "Ingresá un nombre.";
        if (Propuestas.Any(x => string.Equals(
                x.NombreVisible, normalizado, StringComparison.OrdinalIgnoreCase)))
            return "Ya elegiste una opción con ese nombre.";

        Propuestas.Add(new PropuestaSalidaDraft(
            TipoPropuesta.Manual,
            null,
            normalizado,
            Normalizar(descripcion),
            Normalizar(direccion)));
        return null;
    }

    public string? AgregarExterna(LugarExternoDto lugar)
    {
        if (Propuestas.Count >= 3) return "Ya elegiste las tres opciones.";
        if (Propuestas.Any(x => x.GooglePlaceId == lugar.GooglePlaceId))
            return "Ese lugar ya fue elegido.";

        Propuestas.Add(new PropuestaSalidaDraft(
            TipoPropuesta.LugarExterno,
            lugar.GooglePlaceId,
            lugar.Nombre,
            null,
            lugar.Direccion));
        return null;
    }

    public void Quitar(int indice)
    {
        if (indice >= 0 && indice < Propuestas.Count) Propuestas.RemoveAt(indice);
    }

    public CrearSalidaRequest CrearRequest()
    {
        var fechaEncuentro = new DateTimeOffset(Fecha.Date + Hora);
        return new CrearSalidaRequest(
            Nombre.Trim(),
            Normalizar(Descripcion),
            fechaEncuentro,
            DiasParaPropuestas ?? 3,
            DiasParaVotar ?? 2,
            Propuestas.Select(x => new CrearPropuestaInicialRequest(
                x.Tipo,
                x.GooglePlaceId,
                x.Tipo == TipoPropuesta.Manual ? x.NombreVisible : null,
                x.DescripcionManual,
                x.Tipo == TipoPropuesta.Manual ? x.DireccionVisible : null))
                .ToArray());
    }

    public void Limpiar()
    {
        Nombre = string.Empty;
        Descripcion = string.Empty;
        Fecha = DateTime.Today.AddDays(7);
        Hora = new TimeSpan(20, 0, 0);
        DiasParaPropuestas = null;
        DiasParaVotar = null;
        Propuestas.Clear();
    }

    private static string? Normalizar(string? valor) =>
        string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();
}

public sealed record PropuestaSalidaDraft(
    TipoPropuesta Tipo,
    string? GooglePlaceId,
    string NombreVisible,
    string? DescripcionManual,
    string? DireccionVisible);
