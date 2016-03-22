namespace ContosoUniversity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class adddetailscolumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Person", "details", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Person", "details");
        }
    }
}
