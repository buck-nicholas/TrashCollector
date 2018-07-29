namespace TrashCollectorWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedTransactionsTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Transactions",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Amount = c.Double(nullable: false),
                        TransactionCompleted = c.Boolean(nullable: false),
                        CustomerID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Customers", t => t.CustomerID, cascadeDelete: true)
                .Index(t => t.CustomerID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Transactions", "CustomerID", "dbo.Customers");
            DropIndex("dbo.Transactions", new[] { "CustomerID" });
            DropTable("dbo.Transactions");
        }
    }
}
