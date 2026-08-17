using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReunionesDeAmigos.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ReemplazarCatalogoPorLugaresExternos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_propuestas_lugares_LugarId",
                table: "propuestas");

            migrationBuilder.DropIndex(
                name: "IX_propuestas_LugarId",
                table: "propuestas");

            migrationBuilder.DropIndex(
                name: "IX_propuestas_SalidaId_LugarId",
                table: "propuestas");

            migrationBuilder.DropCheckConstraint(
                name: "CK_propuestas_tipo",
                table: "propuestas");

            migrationBuilder.Sql(
                """
                UPDATE propuestas AS p
                SET "Tipo" = 2,
                    "NombreManual" = l."Nombre",
                    "DescripcionManual" = COALESCE(p."DescripcionManual", l."Descripcion"),
                    "DireccionManual" = COALESCE(p."DireccionManual", l."Direccion")
                FROM lugares AS l
                WHERE p."LugarId" = l."Id" AND p."Tipo" = 1;
                """);

            migrationBuilder.DropColumn(
                name: "LugarId",
                table: "propuestas");

            migrationBuilder.AddColumn<string>(
                name: "GooglePlaceId",
                table: "propuestas",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_propuestas_SalidaId_GooglePlaceId",
                table: "propuestas",
                columns: new[] { "SalidaId", "GooglePlaceId" },
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_propuestas_tipo",
                table: "propuestas",
                sql: "(\"Tipo\" = 1 AND \"GooglePlaceId\" IS NOT NULL AND \"NombreManual\" IS NULL) OR (\"Tipo\" = 2 AND \"GooglePlaceId\" IS NULL AND \"NombreManual\" IS NOT NULL)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_propuestas_SalidaId_GooglePlaceId",
                table: "propuestas");

            migrationBuilder.DropCheckConstraint(
                name: "CK_propuestas_tipo",
                table: "propuestas");

            migrationBuilder.Sql(
                """
                UPDATE propuestas
                SET "Tipo" = 2,
                    "NombreManual" = COALESCE("NombreManual", 'Lugar de Google')
                WHERE "Tipo" = 1;
                """);

            migrationBuilder.DropColumn(
                name: "GooglePlaceId",
                table: "propuestas");

            migrationBuilder.AddColumn<Guid>(
                name: "LugarId",
                table: "propuestas",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_propuestas_LugarId",
                table: "propuestas",
                column: "LugarId");

            migrationBuilder.CreateIndex(
                name: "IX_propuestas_SalidaId_LugarId",
                table: "propuestas",
                columns: new[] { "SalidaId", "LugarId" },
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_propuestas_tipo",
                table: "propuestas",
                sql: "(\"Tipo\" = 1 AND \"LugarId\" IS NOT NULL AND \"NombreManual\" IS NULL) OR (\"Tipo\" = 2 AND \"LugarId\" IS NULL AND \"NombreManual\" IS NOT NULL)");

            migrationBuilder.AddForeignKey(
                name: "FK_propuestas_lugares_LugarId",
                table: "propuestas",
                column: "LugarId",
                principalTable: "lugares",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
