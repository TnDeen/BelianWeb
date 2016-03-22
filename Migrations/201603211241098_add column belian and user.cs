namespace ContosoUniversity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addcolumnbeliananduser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Course", "Kg", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Course", "Multiplier", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Course", "Rm", c => c.Decimal(nullable: false, storeType: "money"));
            AddColumn("dbo.Person", "NoLesen", c => c.String(maxLength: 50));
            DropColumn("dbo.Person", "Kg");
            DropColumn("dbo.Person", "Rm");
            DropColumn("dbo.Person", "details");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Person", "details", c => c.String());
            AddColumn("dbo.Person", "Rm", c => c.Double());
            AddColumn("dbo.Person", "Kg", c => c.Double());
            DropColumn("dbo.Person", "NoLesen");
            DropColumn("dbo.Course", "Rm");
            DropColumn("dbo.Course", "Multiplier");
            DropColumn("dbo.Course", "Kg");
        }
    }
}
