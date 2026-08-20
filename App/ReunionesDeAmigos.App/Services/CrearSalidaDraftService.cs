using ReunionesDeAmigos.App.Models.Lugares;
using ReunionesDeAmigos.App.Models.Salidas;

namespace ReunionesDeAmigos.App.Services;

public sealed class CrearSalidaDraftService
{
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public ModalidadFecha Modalidad { get; private set; } = ModalidadFecha.Fija;
    public DateTime Fecha { get; set; } = DateTime.Today.AddDays(7);
    public TimeSpan Hora { get; set; } = new(20, 0, 0);
    public int? DiasParaPropuestas { get; set; }
    public int? DiasParaVotar { get; set; }
    public List<PropuestaSalidaDraft> Propuestas { get; } = [];
    public List<DateTimeOffset> OpcionesFecha { get; } = [];

    public bool PuedeCrear => Propuestas.Count is >= 1 and <= 3;

    public bool AlcanzoLimitePropuestas => Propuestas.Count >= 3;

    public void CambiarModalidad(ModalidadFecha modalidad)
    {
        Modalidad = modalidad;
        if (modalidad == ModalidadFecha.Fija) OpcionesFecha.Clear();
    }

    public string? AgregarOpcionFecha(DateTime fecha, TimeSpan hora)
    {
        if (OpcionesFecha.Count >= 3) return "Podés proponer como máximo tres fechas.";

        var opcion = new DateTimeOffset(fecha.Date + hora);
        if (OpcionesFecha.Any(x => x == opcion)) return "Esa fecha y hora ya fueron agregadas.";

        OpcionesFecha.Add(opcion);
        OpcionesFecha.Sort();
        return null;
    }

    public void QuitarOpcionFecha(int indice)
    {
        if (indice >= 0 && indice < OpcionesFecha.Count) OpcionesFecha.RemoveAt(indice);
    }

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
        DateTimeOffset? fechaEncuentro = Modalidad == ModalidadFecha.Fija
            ? new DateTimeOffset(Fecha.Date + Hora)
            : null;
        return new CrearSalidaRequest(
            Nombre.Trim(),
            Normalizar(Descripcion),
            Modalidad,
            fechaEncuentro,
            OpcionesFecha.ToArray(),
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
        Modalidad = ModalidadFecha.Fija;
        Fecha = DateTime.Today.AddDays(7);
        Hora = new TimeSpan(20, 0, 0);
        DiasParaPropuestas = null;
        DiasParaVotar = null;
        OpcionesFecha.Clear();
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
