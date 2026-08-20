using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReunionesDeAmigos.Domain.Entities;

namespace ReunionesDeAmigos.Infrastructure.Persistence.Configurations;

internal sealed class DisponibilidadFechaConfiguration
    : IEntityTypeConfiguration<DisponibilidadFecha>
{
    public void Configure(EntityTypeBuilder<DisponibilidadFecha> builder)
    {
        builder.ToTable("disponibilidades_fecha");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.HasOne<ParticipanteSalida>()
            .WithMany()
            .HasForeignKey(x => x.ParticipanteSalidaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new
        {
            x.OpcionFechaId,
            x.ParticipanteSalidaId
        }).IsUnique();
    }
}
