namespace NoteKeeper.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PopulateSampleUsers : DbMigration
    {
        public override void Up()
        {
            Sql("INSERT INTO [dbo].[Users] ([Name], [Email], [Password]) VALUES ('Omkar', 'omkar@note.com', 'omkar123');");
            Sql("INSERT INTO [dbo].[Users] ([Name], [Email], [Password]) VALUES ('OmkarU', 'omkar@note.com', 'omkar123');");
            Sql("INSERT INTO [dbo].[Users] ([Name], [Email], [Password]) VALUES ('OmkarDU', 'omkar@note.com', 'omkar123');");
            
        }
        
        public override void Down()
        {
        }
    }
}
