using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CeiliApi.Migrations
{
    /// <inheritdoc />
    public partial class InitFullCeili : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Alumno",
                table: "Evaluaciones");

            migrationBuilder.DropColumn(
                name: "Grupo",
                table: "Evaluaciones");

            migrationBuilder.AddColumn<int>(
                name: "AlumnoId",
                table: "Evaluaciones",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DocenteId",
                table: "Evaluaciones",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Docentes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreCompleto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Grupo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Docentes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Alumnos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreCompleto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Edad = table.Column<int>(type: "int", nullable: false),
                    Sexo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DocenteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alumnos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Alumnos_Docentes_DocenteId",
                        column: x => x.DocenteId,
                        principalTable: "Docentes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Evaluaciones_AlumnoId",
                table: "Evaluaciones",
                column: "AlumnoId");

            migrationBuilder.CreateIndex(
                name: "IX_Evaluaciones_DocenteId",
                table: "Evaluaciones",
                column: "DocenteId");

            migrationBuilder.CreateIndex(
                name: "IX_Alumnos_DocenteId",
                table: "Alumnos",
                column: "DocenteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Evaluaciones_Alumnos_AlumnoId",
                table: "Evaluaciones",
                column: "AlumnoId",
                principalTable: "Alumnos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Evaluaciones_Docentes_DocenteId",
                table: "Evaluaciones",
                column: "DocenteId",
                principalTable: "Docentes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Evaluaciones_Alumnos_AlumnoId",
                table: "Evaluaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_Evaluaciones_Docentes_DocenteId",
                table: "Evaluaciones");

            migrationBuilder.DropTable(
                name: "Alumnos");

            migrationBuilder.DropTable(
                name: "Docentes");

            migrationBuilder.DropIndex(
                name: "IX_Evaluaciones_AlumnoId",
                table: "Evaluaciones");

            migrationBuilder.DropIndex(
                name: "IX_Evaluaciones_DocenteId",
                table: "Evaluaciones");

            migrationBuilder.DropColumn(
                name: "AlumnoId",
                table: "Evaluaciones");

            migrationBuilder.DropColumn(
                name: "DocenteId",
                table: "Evaluaciones");

            migrationBuilder.AddColumn<string>(
                name: "Alumno",
                table: "Evaluaciones",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Grupo",
                table: "Evaluaciones",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
