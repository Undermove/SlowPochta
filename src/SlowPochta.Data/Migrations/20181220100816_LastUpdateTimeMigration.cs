using Microsoft.EntityFrameworkCore.Migrations;

namespace SlowPochta.Data.Migrations
{
    public partial class LastUpdateTimeMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DeliveryDate",
                table: "Messages",
                newName: "LastUpdateTime");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastUpdateTime",
                table: "Messages",
                newName: "DeliveryDate");
        }
    }
}
