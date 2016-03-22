namespace ContosoUniversity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class adddeprtment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Department", "KebenaranBertulis", c => c.String());
            AddColumn("dbo.Department", "ResitRasmi", c => c.String());
            AddColumn("dbo.Department", "Skrap", c => c.Boolean(nullable: false));
            AddColumn("dbo.Department", "Lateks", c => c.Boolean(nullable: false));
            AddColumn("dbo.Department", "Lain", c => c.Boolean(nullable: false));
            AlterStoredProcedure(
                "dbo.Department_Insert",
                p => new
                    {
                        Name = p.String(maxLength: 50),
                        KebenaranBertulis = p.String(),
                        ResitRasmi = p.String(),
                        Skrap = p.Boolean(),
                        Lateks = p.Boolean(),
                        Lain = p.Boolean(),
                        Kg = p.Decimal(precision: 18, scale: 2),
                        Multiplier = p.Decimal(precision: 18, scale: 2),
                        Budget = p.Decimal(precision: 19, scale: 4, storeType: "money"),
                        StartDate = p.DateTime(),
                        InstructorID = p.Int(),
                    },
                body:
                    @"INSERT [dbo].[Department]([Name], [KebenaranBertulis], [ResitRasmi], [Skrap], [Lateks], [Lain], [Kg], [Multiplier], [Budget], [StartDate], [InstructorID])
                      VALUES (@Name, @KebenaranBertulis, @ResitRasmi, @Skrap, @Lateks, @Lain, @Kg, @Multiplier, @Budget, @StartDate, @InstructorID)
                      
                      DECLARE @DepartmentID int
                      SELECT @DepartmentID = [DepartmentID]
                      FROM [dbo].[Department]
                      WHERE @@ROWCOUNT > 0 AND [DepartmentID] = scope_identity()
                      
                      SELECT t0.[DepartmentID], t0.[RowVersion]
                      FROM [dbo].[Department] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[DepartmentID] = @DepartmentID"
            );
            
            AlterStoredProcedure(
                "dbo.Department_Update",
                p => new
                    {
                        DepartmentID = p.Int(),
                        Name = p.String(maxLength: 50),
                        KebenaranBertulis = p.String(),
                        ResitRasmi = p.String(),
                        Skrap = p.Boolean(),
                        Lateks = p.Boolean(),
                        Lain = p.Boolean(),
                        Kg = p.Decimal(precision: 18, scale: 2),
                        Multiplier = p.Decimal(precision: 18, scale: 2),
                        Budget = p.Decimal(precision: 19, scale: 4, storeType: "money"),
                        StartDate = p.DateTime(),
                        InstructorID = p.Int(),
                        RowVersion_Original = p.Binary(maxLength: 8, fixedLength: true, storeType: "rowversion"),
                    },
                body:
                    @"UPDATE [dbo].[Department]
                      SET [Name] = @Name, [KebenaranBertulis] = @KebenaranBertulis, [ResitRasmi] = @ResitRasmi, [Skrap] = @Skrap, [Lateks] = @Lateks, [Lain] = @Lain, [Kg] = @Kg, [Multiplier] = @Multiplier, [Budget] = @Budget, [StartDate] = @StartDate, [InstructorID] = @InstructorID
                      WHERE (([DepartmentID] = @DepartmentID) AND (([RowVersion] = @RowVersion_Original) OR ([RowVersion] IS NULL AND @RowVersion_Original IS NULL)))
                      
                      SELECT t0.[RowVersion]
                      FROM [dbo].[Department] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[DepartmentID] = @DepartmentID"
            );
            
        }
        
        public override void Down()
        {
            DropColumn("dbo.Department", "Lain");
            DropColumn("dbo.Department", "Lateks");
            DropColumn("dbo.Department", "Skrap");
            DropColumn("dbo.Department", "ResitRasmi");
            DropColumn("dbo.Department", "KebenaranBertulis");
            throw new NotSupportedException("Scaffolding create or alter procedure operations is not supported in down methods.");
        }
    }
}
