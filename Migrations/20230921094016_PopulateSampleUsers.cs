using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NoteKeeper.Migrations
{
    public partial class PopulateSampleUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.Sql("INSERT INTO [dbo].[Users] ([Name], [Email], [Password]) VALUES ('Omkar', 'omkar@note.com', 'omkar123');");
            migrationBuilder.Sql("INSERT INTO [dbo].[Users] ([Name], [Email], [Password]) VALUES ('OmkarU', 'omkar@note.com', 'omkar123');");
            migrationBuilder.Sql("INSERT INTO [dbo].[Users] ([Name], [Email], [Password]) VALUES ('OmkarDU', 'omkar@note.com', 'omkar123');");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
