using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NoteKeeper.Migrations
{
    public partial class AddData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                SET IDENTITY_INSERT [dbo].[Users] ON
                INSERT INTO [dbo].[Users] ([Id], [Name], [Email], [Password]) VALUES (1, N'test1', N'test1@note.com', N'$2a$10$GSLqXdBsOnbNwlAKp8x/3Oq5d9cFelhzqCFpe6aNQjRkfU3D/RX3O')
                INSERT INTO [dbo].[Users] ([Id], [Name], [Email], [Password]) VALUES (2, N'test2', N'test2@note.com', N'$2a$10$Pdk8ko3uSwBPDwvmiSy8BOK0xYMLZO2gPeTaXYowMxHwpzoo7wAiq')
                INSERT INTO [dbo].[Users] ([Id], [Name], [Email], [Password]) VALUES (3, N'test3', N'test3@note.com', N'$2a$10$xE/5kfOs5GdW6n0zsoJpF.Pp6NyJGihIltjsjsvi/0GDaUdHQX5Iq')
                SET IDENTITY_INSERT [dbo].[Users] OFF
            ");
            migrationBuilder.Sql(@"
                INSERT INTO [dbo].[Notes] ( [Title], [Description], [DateAdded], [UserId]) VALUES ( N'Title 1', N'This is 1 description', N'2023-09-22 20:42:12', 1)
                INSERT INTO [dbo].[Notes] ( [Title], [Description], [DateAdded], [UserId]) VALUES ( N'Title 2', N'This is 2 description', N'2023-09-22 20:42:22', 1)
                INSERT INTO [dbo].[Notes] ( [Title], [Description], [DateAdded], [UserId]) VALUES ( N'Title 3', N'This is 3 description', N'2023-09-22 20:42:31', 1)
                INSERT INTO [dbo].[Notes] ( [Title], [Description], [DateAdded], [UserId]) VALUES ( N'Title 3', N'This is 3 description', N'2023-09-22 20:42:35', 2)
            ");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
