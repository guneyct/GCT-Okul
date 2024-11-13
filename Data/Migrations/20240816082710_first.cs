using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class first : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Username",
                table: "Teacher",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "Student",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "Admin",
                newName: "Email");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Teacher",
                newName: "Username");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Student",
                newName: "Username");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Admin",
                newName: "Username");
        }
    }
}
