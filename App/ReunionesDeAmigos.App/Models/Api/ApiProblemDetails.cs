namespace ReunionesDeAmigos.App.Models.Api;

public sealed record ApiProblemDetails(
    string? Title,
    string? Detail,
    int? Status);
