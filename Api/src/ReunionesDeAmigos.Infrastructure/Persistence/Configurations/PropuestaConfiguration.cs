using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReunionesDeAmigos.Domain.Entities;

namespace ReunionesDeAmigos.Infrastructure.Persistence.Configurations;

internal sealed class PropuestaConfiguration : IEntityTypeConfiguration<Propuesta>
{
    public void Configure(EntityTypeBuilder<Propuesta> builder)
    {
        builder.ToTable("propuestas", table => table.HasCheckConstraint(
            "CK_propuestas_tipo",
            "(\"Tipo\" = 1 AND \"LugarId\" IS NOT NULL AND \"NombreManual\" IS NULL) OR " +
            "(\"Tipo\" = 2 AND \"LugarId\" IS NULL AND \"NombreManual\" IS NOT NULL)"));
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.NombreManual).HasMaxLength(150);
        builder.Property(x => x.DescripcionManual).HasMaxLength(1000);
        builder.Property(x => x.DireccionManual).HasMaxLength(250);

        builder.HasOne<ParticipanteSalida>()
            .WithMany()
            .HasForeignKey(x => x.ParticipanteSalidaId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Lugar>()
            .WithMany()
            .HasForeignKey(x => x.LugarId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.SalidaId, x.LugarId }).IsUnique();
    }
}
