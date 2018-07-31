namespace TrashCollectorWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedIsActivePropToPickUpDirectory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PickUpDirectories", "IsActive", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PickUpDirectories", "IsActive");
        }
    }
}
