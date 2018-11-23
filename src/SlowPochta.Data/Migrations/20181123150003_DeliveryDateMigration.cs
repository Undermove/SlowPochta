using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SlowPochta.Data.Migrations
{
    public partial class DeliveryDateMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveryDate",
                table: "Messages",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryDate",
                table: "Messages");
        }
    }
}
