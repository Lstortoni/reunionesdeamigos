using Microsoft.EntityFrameworkCore;
using ReunionesDeAmigos.Domain.Entities;

namespace ReunionesDeAmigos.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Salida> Salidas => Set<Salida>();
    public DbSet<ParticipanteSalida> ParticipantesSalida => Set<ParticipanteSalida>();
    public DbSet<Lugar> Lugares => Set<Lugar>();
    public DbSet<Ciudad> Ciudades => Set<Ciudad>();
    public DbSet<Propuesta> Propuestas => Set<Propuesta>();
    public DbSet<Voto> Votos => Set<Voto>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
