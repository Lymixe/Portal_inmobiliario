using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Portal_inmobiliario.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialDomainSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Inmuebles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Codigo = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Titulo = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Imagen = table.Column<string>(type: "TEXT", nullable: true),
                    Tipo = table.Column<int>(type: "INTEGER", nullable: false),
                    Ciudad = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Direccion = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Dormitorios = table.Column<int>(type: "INTEGER", nullable: false),
                    Banos = table.Column<int>(type: "INTEGER", nullable: false),
                    MetrosCuadrados = table.Column<double>(type: "REAL", nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    Activo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inmuebles", x => x.Id);
                    table.CheckConstraint("CK_Inmueble_MetrosCuadrados", "MetrosCuadrados > 0");
                    table.CheckConstraint("CK_Inmueble_Precio", "Precio > 0");
                });

            migrationBuilder.CreateTable(
                name: "Reservas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InmuebleId = table.Column<int>(type: "INTEGER", nullable: false),
                    UsuarioId = table.Column<string>(type: "TEXT", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FechaExpiracion = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reservas_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reservas_Inmuebles_InmuebleId",
                        column: x => x.InmuebleId,
                        principalTable: "Inmuebles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Visitas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InmuebleId = table.Column<int>(type: "INTEGER", nullable: false),
                    UsuarioId = table.Column<string>(type: "TEXT", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Estado = table.Column<int>(type: "INTEGER", nullable: false),
                    Notas = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Visitas", x => x.Id);
                    table.CheckConstraint("CK_Visita_Fechas", "FechaFin > FechaInicio");
                    table.ForeignKey(
                        name: "FK_Visitas_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Visitas_Inmuebles_InmuebleId",
                        column: x => x.InmuebleId,
                        principalTable: "Inmuebles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Inmuebles",
                columns: new[] { "Id", "Activo", "Banos", "Ciudad", "Codigo", "Direccion", "Dormitorios", "Imagen", "MetrosCuadrados", "Precio", "Tipo", "Titulo" },
                values: new object[,]
                {
                    { 1, true, 2, "Madrid", "PISO-MAD-001", "Calle de Goya, 75", 3, "/images/piso_salamanca.jpg", 110.5, 650000m, 0, "Luminoso Piso en el Barrio de Salamanca" },
                    { 2, true, 4, "Barcelona", "CHALET-BCN-001", "Avinguda del Port, 22, Sitges", 5, "/images/chalet_sitges.jpg", 320.0, 1250000m, 1, "Espectacular Chalet con Vistas al Mar en Sitges" },
                    { 3, true, 2, "Valencia", "OFI-VLC-001", "Avenida de las Cortes Valencianas, 58", 0, "/images/oficina_valencia.jpg", 150.0, 850000m, 2, "Moderna Oficina en el Centro Financiero" },
                    { 4, false, 1, "Sevilla", "LOFT-SEV-001", "Calle Betis, 41", 1, "/images/loft_triana.jpg", 65.0, 280000m, 0, "Loft con Encanto en el Barrio de Triana" },
                    { 5, true, 1, "Madrid", "LOCAL-MAD-001", "Calle del Pez, 15", 0, "/images/local_malasana.jpg", 80.0, 450000m, 3, "Local Comercial en Zona Peatonal de Malasaña" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Inmuebles_Codigo",
                table: "Inmuebles",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_InmuebleId",
                table: "Reservas",
                column: "InmuebleId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_UsuarioId",
                table: "Reservas",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Visitas_InmuebleId",
                table: "Visitas",
                column: "InmuebleId");

            migrationBuilder.CreateIndex(
                name: "IX_Visitas_UsuarioId",
                table: "Visitas",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reservas");

            migrationBuilder.DropTable(
                name: "Visitas");

            migrationBuilder.DropTable(
                name: "Inmuebles");
        }
    }
}
