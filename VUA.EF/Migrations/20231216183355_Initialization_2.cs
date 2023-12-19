using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VUA.EF.Migrations
{
    /// <inheritdoc />
    public partial class Initialization_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "fileUrls",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UrlString = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fileUrls", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "weekFileUrls",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false),
                    WeekId = table.Column<int>(type: "int", nullable: false),
                    FileUrlid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_weekFileUrls", x => new { x.WeekId, x.id });
                    table.ForeignKey(
                        name: "FK_weekFileUrls_Weeks_WeekId",
                        column: x => x.WeekId,
                        principalTable: "Weeks",
                        principalColumn: "WeekId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_weekFileUrls_fileUrls_FileUrlid",
                        column: x => x.FileUrlid,
                        principalTable: "fileUrls",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_weekFileUrls_fileUrls_id",
                        column: x => x.id,
                        principalTable: "fileUrls",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_weekFileUrls_FileUrlid",
                table: "weekFileUrls",
                column: "FileUrlid");

            migrationBuilder.CreateIndex(
                name: "IX_weekFileUrls_id",
                table: "weekFileUrls",
                column: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "weekFileUrls");

            migrationBuilder.DropTable(
                name: "fileUrls");
        }
    }
}
