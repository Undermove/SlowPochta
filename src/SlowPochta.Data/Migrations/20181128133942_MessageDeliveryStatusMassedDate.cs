using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SlowPochta.Data.Migrations
{
    public partial class MessageDeliveryStatusMassedDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "TransitionDateTime",
                table: "MessagePassedDeliveryStatuses",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TransitionDateTime",
                table: "MessagePassedDeliveryStatuses");
        }
    }
}
