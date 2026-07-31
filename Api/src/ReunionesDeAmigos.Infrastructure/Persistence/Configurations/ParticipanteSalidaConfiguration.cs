using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReunionesDeAmigos.Domain.Entities;

namespace ReunionesDeAmigos.Infrastructure.Persistence.Configurations;

internal sealed class ParticipanteSalidaConfiguration
    : IEntityTypeConfiguration<ParticipanteSalida>
{
    public void Configure(EntityTypeBuilder<ParticipanteSalida> builder)
    {
        builder.ToTable("participantes_salida", table => table.HasCheckConstraint(
            "CK_participantes_salida_identidad",
            "(\"UsuarioId\" IS NOT NULL AND \"CredencialInvitadoHash\" IS NULL) OR " +
            "(\"UsuarioId\" IS NULL AND \"CredencialInvitadoHash\" IS NOT NULL)"));
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.NombreVisible).HasMaxLength(100).IsRequired();
        builder.Property(x => x.CredencialInvitadoHash).HasMaxLength(256);
        builder.Ignore(x => x.EsInvitado);

        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(x => x.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.SalidaId, x.UsuarioId }).IsUnique();
        builder.HasIndex(x => x.CredencialInvitadoHash).IsUnique();
    }
}
