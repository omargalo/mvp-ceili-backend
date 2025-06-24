using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CeiliApi.Migrations
{
    /// <inheritdoc />
    public partial class AddDocentePassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Docentes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PasswordSalt",
                table: "Docentes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Docentes");

            migrationBuilder.DropColumn(
                name: "PasswordSalt",
                table: "Docentes");
        }
    }
}
