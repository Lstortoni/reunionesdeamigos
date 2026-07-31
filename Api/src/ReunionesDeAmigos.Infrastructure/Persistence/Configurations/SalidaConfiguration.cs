using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReunionesDeAmigos.Domain.Entities;

namespace ReunionesDeAmigos.Infrastructure.Persistence.Configurations;

internal sealed class SalidaConfiguration : IEntityTypeConfiguration<Salida>
{
    public void Configure(EntityTypeBuilder<Salida> builder)
    {
        builder.ToTable("salidas", table => table.HasCheckConstraint(
            "CK_salidas_fechas",
            "\"FechaFinPropuestas\" < \"FechaFinVotacion\" AND \"FechaFinVotacion\" < \"FechaEncuentro\""));
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Nombre).HasMaxLength(150).IsRequired();
        builder.Property(x => x.Descripcion).HasMaxLength(1000);
        builder.Property(x => x.CodigoAcceso).HasMaxLength(30).IsRequired();
        builder.HasIndex(x => x.CodigoAcceso).IsUnique();

        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(x => x.CreadorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Participantes)
            .WithOne()
            .HasForeignKey(x => x.SalidaId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Propuestas)
            .WithOne()
            .HasForeignKey(x => x.SalidaId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Votos)
            .WithOne()
            .HasForeignKey(x => x.SalidaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
