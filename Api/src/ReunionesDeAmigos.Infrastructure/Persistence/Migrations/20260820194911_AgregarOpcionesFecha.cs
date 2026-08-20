using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReunionesDeAmigos.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarOpcionesFecha : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_salidas_fechas",
                table: "salidas");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "FechaEncuentro",
                table: "salidas",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<int>(
                name: "Modalidad",
                table: "salidas",
                type: "integer",
                nullable: true);

            migrationBuilder.Sql(
                "UPDATE salidas SET \"Modalidad\" = 1 WHERE \"Modalidad\" IS NULL;");

            migrationBuilder.AlterColumn<int>(
                name: "Modalidad",
                table: "salidas",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "opciones_fecha",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SalidaId = table.Column<Guid>(type: "uuid", nullable: false),
                    ParticipanteSalidaId = table.Column<Guid>(type: "uuid", nullable: false),
                    FechaHora = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    FechaCreacion = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_opciones_fecha", x => x.Id);
                    table.ForeignKey(
                        name: "FK_opciones_fecha_participantes_salida_ParticipanteSalidaId",
                        column: x => x.ParticipanteSalidaId,
                        principalTable: "participantes_salida",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_opciones_fecha_salidas_SalidaId",
                        column: x => x.SalidaId,
                        principalTable: "salidas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "disponibilidades_fecha",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OpcionFechaId = table.Column<Guid>(type: "uuid", nullable: false),
                    ParticipanteSalidaId = table.Column<Guid>(type: "uuid", nullable: false),
                    Disponible = table.Column<bool>(type: "boolean", nullable: false),
                    FechaRespuesta = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_disponibilidades_fecha", x => x.Id);
                    table.ForeignKey(
                        name: "FK_disponibilidades_fecha_opciones_fecha_OpcionFechaId",
                        column: x => x.OpcionFechaId,
                        principalTable: "opciones_fecha",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_disponibilidades_fecha_participantes_salida_ParticipanteSal~",
                        column: x => x.ParticipanteSalidaId,
                        principalTable: "participantes_salida",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.AddCheckConstraint(
                name: "CK_salidas_fechas",
                table: "salidas",
                sql: "\"FechaFinPropuestas\" < \"FechaFinVotacion\" AND ((\"Modalidad\" = 1 AND \"FechaEncuentro\" IS NOT NULL AND \"FechaFinVotacion\" < \"FechaEncuentro\") OR (\"Modalidad\" = 2 AND \"FechaEncuentro\" IS NULL))");

            migrationBuilder.CreateIndex(
                name: "IX_disponibilidades_fecha_OpcionFechaId_ParticipanteSalidaId",
                table: "disponibilidades_fecha",
                columns: new[] { "OpcionFechaId", "ParticipanteSalidaId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_disponibilidades_fecha_ParticipanteSalidaId",
                table: "disponibilidades_fecha",
                column: "ParticipanteSalidaId");

            migrationBuilder.CreateIndex(
                name: "IX_opciones_fecha_ParticipanteSalidaId",
                table: "opciones_fecha",
                column: "ParticipanteSalidaId");

            migrationBuilder.CreateIndex(
                name: "IX_opciones_fecha_SalidaId_FechaHora",
                table: "opciones_fecha",
                columns: new[] { "SalidaId", "FechaHora" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "disponibilidades_fecha");

            migrationBuilder.DropTable(
                name: "opciones_fecha");

            migrationBuilder.DropCheckConstraint(
                name: "CK_salidas_fechas",
                table: "salidas");

            migrationBuilder.DropColumn(
                name: "Modalidad",
                table: "salidas");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "FechaEncuentro",
                table: "salidas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_salidas_fechas",
                table: "salidas",
                sql: "\"FechaFinPropuestas\" < \"FechaFinVotacion\" AND \"FechaFinVotacion\" < \"FechaEncuentro\"");
        }
    }
}
