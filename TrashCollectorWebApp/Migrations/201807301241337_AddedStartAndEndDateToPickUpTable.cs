namespace TrashCollectorWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedStartAndEndDateToPickUpTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PickUpDirectories", "StartDate", c => c.String());
            AddColumn("dbo.PickUpDirectories", "EndDate", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PickUpDirectories", "EndDate");
            DropColumn("dbo.PickUpDirectories", "StartDate");
        }
    }
}
