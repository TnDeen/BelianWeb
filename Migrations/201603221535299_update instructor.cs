namespace ContosoUniversity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateinstructor : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Person", "NomborLesen", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Person", "NomborLesen");
        }
    }
}
