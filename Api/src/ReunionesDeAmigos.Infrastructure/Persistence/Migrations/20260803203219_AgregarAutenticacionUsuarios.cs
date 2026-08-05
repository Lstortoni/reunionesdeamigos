using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReunionesDeAmigos.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarAutenticacionUsuarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "usuarios",
                type: "character varying(512)",
                maxLength: 512,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "usuarios");
        }
    }
}
