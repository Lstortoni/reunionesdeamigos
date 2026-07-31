using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReunionesDeAmigos.Domain.Entities;

namespace ReunionesDeAmigos.Infrastructure.Persistence.Configurations;

internal sealed class VotoConfiguration : IEntityTypeConfiguration<Voto>
{
    public void Configure(EntityTypeBuilder<Voto> builder)
    {
        builder.ToTable("votos");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.HasOne<ParticipanteSalida>()
            .WithMany()
            .HasForeignKey(x => x.ParticipanteSalidaId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Propuesta>()
            .WithMany()
            .HasForeignKey(x => x.PropuestaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.SalidaId, x.ParticipanteSalidaId }).IsUnique();
    }
}
