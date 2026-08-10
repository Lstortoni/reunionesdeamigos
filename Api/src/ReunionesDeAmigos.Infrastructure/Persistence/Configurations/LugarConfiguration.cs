using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReunionesDeAmigos.Domain.Entities;

namespace ReunionesDeAmigos.Infrastructure.Persistence.Configurations;

internal sealed class LugarConfiguration : IEntityTypeConfiguration<Lugar>
{
    public void Configure(EntityTypeBuilder<Lugar> builder)
    {
        builder.ToTable("lugares", table => table.HasCheckConstraint(
            "CK_lugares_coordenadas",
            "(\"Latitud\" IS NULL AND \"Longitud\" IS NULL) OR " +
            "(\"Latitud\" BETWEEN -90 AND 90 AND \"Longitud\" BETWEEN -180 AND 180)"));
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Nombre).HasMaxLength(150).IsRequired();
        builder.Property(x => x.Descripcion).HasMaxLength(1000);
        builder.Property(x => x.Direccion).HasMaxLength(250).IsRequired();
        builder.Property(x => x.Barrio).HasMaxLength(100);
        builder.Property(x => x.Latitud).HasPrecision(9, 6);
        builder.Property(x => x.Longitud).HasPrecision(9, 6);
        builder.HasOne(x => x.Ciudad)
            .WithMany()
            .HasForeignKey(x => x.CiudadId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.CiudadId, x.Tipo, x.Activo });
    }
}
