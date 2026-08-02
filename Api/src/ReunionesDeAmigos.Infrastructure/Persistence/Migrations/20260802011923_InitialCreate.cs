using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReunionesDeAmigos.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "lugares",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Direccion = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Barrio = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Ciudad = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    Latitud = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: true),
                    Longitud = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lugares", x => x.Id);
                    table.CheckConstraint("CK_lugares_coordenadas", "(\"Latitud\" IS NULL AND \"Longitud\" IS NULL) OR (\"Latitud\" BETWEEN -90 AND 90 AND \"Longitud\" BETWEEN -180 AND 180)");
                });

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: false),
                    FechaCreacion = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "salidas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    FechaEncuentro = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    FechaFinPropuestas = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    FechaFinVotacion = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CodigoAcceso = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    CreadorId = table.Column<Guid>(type: "uuid", nullable: false),
                    FechaCreacion = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    FechaCancelacion = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_salidas", x => x.Id);
                    table.CheckConstraint("CK_salidas_fechas", "\"FechaFinPropuestas\" < \"FechaFinVotacion\" AND \"FechaFinVotacion\" < \"FechaEncuentro\"");
                    table.ForeignKey(
                        name: "FK_salidas_usuarios_CreadorId",
                        column: x => x.CreadorId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "participantes_salida",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SalidaId = table.Column<Guid>(type: "uuid", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: true),
                    NombreVisible = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FechaIngreso = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CredencialInvitadoHash = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_participantes_salida", x => x.Id);
                    table.CheckConstraint("CK_participantes_salida_identidad", "(\"UsuarioId\" IS NOT NULL AND \"CredencialInvitadoHash\" IS NULL) OR (\"UsuarioId\" IS NULL AND \"CredencialInvitadoHash\" IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_participantes_salida_salidas_SalidaId",
                        column: x => x.SalidaId,
                        principalTable: "salidas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_participantes_salida_usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "propuestas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SalidaId = table.Column<Guid>(type: "uuid", nullable: false),
                    ParticipanteSalidaId = table.Column<Guid>(type: "uuid", nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    LugarId = table.Column<Guid>(type: "uuid", nullable: true),
                    NombreManual = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    DescripcionManual = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    DireccionManual = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    FechaCreacion = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_propuestas", x => x.Id);
                    table.CheckConstraint("CK_propuestas_tipo", "(\"Tipo\" = 1 AND \"LugarId\" IS NOT NULL AND \"NombreManual\" IS NULL) OR (\"Tipo\" = 2 AND \"LugarId\" IS NULL AND \"NombreManual\" IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_propuestas_lugares_LugarId",
                        column: x => x.LugarId,
                        principalTable: "lugares",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_propuestas_participantes_salida_ParticipanteSalidaId",
                        column: x => x.ParticipanteSalidaId,
                        principalTable: "participantes_salida",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_propuestas_salidas_SalidaId",
                        column: x => x.SalidaId,
                        principalTable: "salidas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "votos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SalidaId = table.Column<Guid>(type: "uuid", nullable: false),
                    ParticipanteSalidaId = table.Column<Guid>(type: "uuid", nullable: false),
                    PropuestaId = table.Column<Guid>(type: "uuid", nullable: false),
                    FechaCreacion = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    FechaUltimaModificacion = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_votos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_votos_participantes_salida_ParticipanteSalidaId",
                        column: x => x.ParticipanteSalidaId,
                        principalTable: "participantes_salida",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_votos_propuestas_PropuestaId",
                        column: x => x.PropuestaId,
                        principalTable: "propuestas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_votos_salidas_SalidaId",
                        column: x => x.SalidaId,
                        principalTable: "salidas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_lugares_Ciudad_Tipo_Activo",
                table: "lugares",
                columns: new[] { "Ciudad", "Tipo", "Activo" });

            migrationBuilder.CreateIndex(
                name: "IX_participantes_salida_CredencialInvitadoHash",
                table: "participantes_salida",
                column: "CredencialInvitadoHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_participantes_salida_SalidaId_UsuarioId",
                table: "participantes_salida",
                columns: new[] { "SalidaId", "UsuarioId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_participantes_salida_UsuarioId",
                table: "participantes_salida",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_propuestas_LugarId",
                table: "propuestas",
                column: "LugarId");

            migrationBuilder.CreateIndex(
                name: "IX_propuestas_ParticipanteSalidaId",
                table: "propuestas",
                column: "ParticipanteSalidaId");

            migrationBuilder.CreateIndex(
                name: "IX_propuestas_SalidaId_LugarId",
                table: "propuestas",
                columns: new[] { "SalidaId", "LugarId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_salidas_CodigoAcceso",
                table: "salidas",
                column: "CodigoAcceso",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_salidas_CreadorId",
                table: "salidas",
                column: "CreadorId");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_Email",
                table: "usuarios",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_votos_ParticipanteSalidaId",
                table: "votos",
                column: "ParticipanteSalidaId");

            migrationBuilder.CreateIndex(
                name: "IX_votos_PropuestaId",
                table: "votos",
                column: "PropuestaId");

            migrationBuilder.CreateIndex(
                name: "IX_votos_SalidaId_ParticipanteSalidaId",
                table: "votos",
                columns: new[] { "SalidaId", "ParticipanteSalidaId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "votos");

            migrationBuilder.DropTable(
                name: "propuestas");

            migrationBuilder.DropTable(
                name: "lugares");

            migrationBuilder.DropTable(
                name: "participantes_salida");

            migrationBuilder.DropTable(
                name: "salidas");

            migrationBuilder.DropTable(
                name: "usuarios");
        }
    }
}
