using Microsoft.EntityFrameworkCore.Migrations;

namespace asagiv.datapush.ui.Migrations
{
    public partial class WinDataMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConnectionSettingsSet",
                columns: table => new
                {
                    Id = table.Column<uint>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ConnectionName = table.Column<string>(type: "TEXT", nullable: true),
                    ConnectionString = table.Column<string>(type: "TEXT", nullable: true),
                    IsPullNode = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConnectionSettingsSet", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConnectionSettingsSet");
        }
    }
}
