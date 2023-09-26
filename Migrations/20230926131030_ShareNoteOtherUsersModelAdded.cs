using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NoteKeeper.Migrations
{
    public partial class ShareNoteOtherUsersModelAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShareNoteOtherUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    NoteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShareNoteOtherUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShareNoteOtherUsers_Notes_NoteId",
                        column: x => x.NoteId,
                        principalTable: "Notes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShareNoteOtherUsers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShareNoteOtherUsers_NoteId",
                table: "ShareNoteOtherUsers",
                column: "NoteId");

            migrationBuilder.CreateIndex(
                name: "IX_ShareNoteOtherUsers_UserId",
                table: "ShareNoteOtherUsers",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShareNoteOtherUsers");
        }
    }
}
