namespace NoteKeeper.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NotePropertyUpdated : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Notes", "Title", c => c.String(nullable: false));
            AlterColumn("dbo.Notes", "Description", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Notes", "Description", c => c.String());
            AlterColumn("dbo.Notes", "Title", c => c.String());
        }
    }
}
