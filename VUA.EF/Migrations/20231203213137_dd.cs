using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VUA.EF.Migrations
{
    /// <inheritdoc />
    public partial class dd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsExternalUser",
                table: "AspNetUsers",
                newName: "IsRejectedUser");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsRejectedUser",
                table: "AspNetUsers",
                newName: "IsExternalUser");
        }
    }
}
