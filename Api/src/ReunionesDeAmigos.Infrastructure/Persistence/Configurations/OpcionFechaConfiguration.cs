using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReunionesDeAmigos.Domain.Entities;

namespace ReunionesDeAmigos.Infrastructure.Persistence.Configurations;

internal sealed class OpcionFechaConfiguration
    : IEntityTypeConfiguration<OpcionFecha>
{
    public void Configure(EntityTypeBuilder<OpcionFecha> builder)
    {
        builder.ToTable("opciones_fecha");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.HasOne<ParticipanteSalida>()
            .WithMany()
            .HasForeignKey(x => x.ParticipanteSalidaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Disponibilidades)
            .WithOne()
            .HasForeignKey(x => x.OpcionFechaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.SalidaId, x.FechaHora })
            .IsUnique();
    }
}
