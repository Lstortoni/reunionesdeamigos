using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReunionesDeAmigos.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCiudades : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_lugares_Ciudad_Tipo_Activo",
                table: "lugares");

            migrationBuilder.DropColumn(
                name: "Ciudad",
                table: "lugares");

            migrationBuilder.AddColumn<Guid>(
                name: "CiudadId",
                table: "lugares",
                type: "uuid",
                nullable: false);

            migrationBuilder.CreateTable(
                name: "ciudades",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Provincia = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Pais = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Activa = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ciudades", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_lugares_CiudadId_Tipo_Activo",
                table: "lugares",
                columns: new[] { "CiudadId", "Tipo", "Activo" });

            migrationBuilder.CreateIndex(
                name: "IX_ciudades_Pais_Provincia_Nombre",
                table: "ciudades",
                columns: new[] { "Pais", "Provincia", "Nombre" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_lugares_ciudades_CiudadId",
                table: "lugares",
                column: "CiudadId",
                principalTable: "ciudades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_lugares_ciudades_CiudadId",
                table: "lugares");

            migrationBuilder.DropTable(
                name: "ciudades");

            migrationBuilder.DropIndex(
                name: "IX_lugares_CiudadId_Tipo_Activo",
                table: "lugares");

            migrationBuilder.DropColumn(
                name: "CiudadId",
                table: "lugares");

            migrationBuilder.AddColumn<string>(
                name: "Ciudad",
                table: "lugares",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_lugares_Ciudad_Tipo_Activo",
                table: "lugares",
                columns: new[] { "Ciudad", "Tipo", "Activo" });
        }
    }
}
