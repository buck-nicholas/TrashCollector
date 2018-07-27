namespace TrashCollectorWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedAssignedZipCodeToEmployee : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Employees", "AssignedZip", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Employees", "AssignedZip");
        }
    }
}
