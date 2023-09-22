using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NoteKeeper.Migrations
{
    public partial class NoteSharePermissionsAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Permission",
                table: "Notes",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Permission",
                table: "Notes");
        }
    }
}
