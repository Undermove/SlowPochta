using Microsoft.EntityFrameworkCore.Migrations;

namespace SlowPochta.Data.Migrations
{
    public partial class AddMaxLengthToStatusDescription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DeliveryStatusDescription",
                table: "MessageDeliveryStatusVariants",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeliveryStatusHeader",
                table: "MessageDeliveryStatusVariants",
                maxLength: 256,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryStatusHeader",
                table: "MessageDeliveryStatusVariants");

            migrationBuilder.AlterColumn<string>(
                name: "DeliveryStatusDescription",
                table: "MessageDeliveryStatusVariants",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 2000,
                oldNullable: true);
        }
    }
}
