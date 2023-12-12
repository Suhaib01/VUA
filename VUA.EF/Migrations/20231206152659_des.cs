using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VUA.EF.Migrations
{
    /// <inheritdoc />
    public partial class des : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WelcomeViduoUrl",
                table: "Weeks",
                newName: "Description");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Weeks",
                newName: "WelcomeViduoUrl");
        }
    }
}
