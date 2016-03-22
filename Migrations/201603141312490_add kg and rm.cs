namespace ContosoUniversity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addkgandrm : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Person", "Kg", c => c.Double());
            AddColumn("dbo.Person", "Rm", c => c.Double());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Person", "Rm");
            DropColumn("dbo.Person", "Kg");
        }
    }
}
