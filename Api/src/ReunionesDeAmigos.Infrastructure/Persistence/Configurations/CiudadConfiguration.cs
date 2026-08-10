using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReunionesDeAmigos.Domain.Entities;

namespace ReunionesDeAmigos.Infrastructure.Persistence.Configurations;

internal sealed class CiudadConfiguration : IEntityTypeConfiguration<Ciudad>
{
    public void Configure(EntityTypeBuilder<Ciudad> builder)
    {
        builder.ToTable("ciudades");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Nombre).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Provincia).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Pais).HasMaxLength(100).IsRequired();
        builder.HasIndex(x => new { x.Pais, x.Provincia, x.Nombre })
            .IsUnique();
    }
}
