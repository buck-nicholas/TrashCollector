namespace TrashCollectorWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TestMig : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PickUpDirectories",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        DayOfWeek = c.String(),
                        SpecialPickUp = c.Boolean(nullable: false),
                        SpecialDate = c.String(),
                        PickUpCompleted = c.Boolean(nullable: false),
                        CustomerID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Customers", t => t.CustomerID, cascadeDelete: true)
                .Index(t => t.CustomerID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PickUpDirectories", "CustomerID", "dbo.Customers");
            DropIndex("dbo.PickUpDirectories", new[] { "CustomerID" });
            DropTable("dbo.PickUpDirectories");
        }
    }
}
