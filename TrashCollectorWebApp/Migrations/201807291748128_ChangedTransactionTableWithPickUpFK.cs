namespace TrashCollectorWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedTransactionTableWithPickUpFK : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Transactions", "CustomerID", "dbo.Customers");
            DropIndex("dbo.Transactions", new[] { "CustomerID" });
            AddColumn("dbo.Transactions", "PickUpID", c => c.Int(nullable: false));
            CreateIndex("dbo.Transactions", "PickUpID");
            AddForeignKey("dbo.Transactions", "PickUpID", "dbo.PickUpDirectories", "ID", cascadeDelete: true);
            DropColumn("dbo.Transactions", "CustomerID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Transactions", "CustomerID", c => c.Int(nullable: false));
            DropForeignKey("dbo.Transactions", "PickUpID", "dbo.PickUpDirectories");
            DropIndex("dbo.Transactions", new[] { "PickUpID" });
            DropColumn("dbo.Transactions", "PickUpID");
            CreateIndex("dbo.Transactions", "CustomerID");
            AddForeignKey("dbo.Transactions", "CustomerID", "dbo.Customers", "ID", cascadeDelete: true);
        }
    }
}
