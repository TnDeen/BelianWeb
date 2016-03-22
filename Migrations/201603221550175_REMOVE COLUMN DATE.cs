namespace ContosoUniversity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class REMOVECOLUMNDATE : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Person", "HireDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Person", "HireDate", c => c.DateTime());
        }
    }
}
