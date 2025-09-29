using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM_Homestay.App.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRoomAmenityAndRoomUsage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RoomUsages_BookingRoomId",
                table: "RoomUsages");

            migrationBuilder.AddColumn<Guid>(
                name: "RoomId",
                table: "RoomUsages",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "RoomAmenities",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b480e58f-14a5-414c-b54b-89d168f833b2"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "476cc704-1af5-4a15-b110-6d58980c7225", "AQAAAAIAAYagAAAAENmFvd+9qAuJqDZEnPKw+t/8EhuKprya5sWZRJ344YcXmNvBNJmUh26G2RzxQ9ph7w==", "8aa9dbd7-2600-43c4-91c4-c688d0d72a24" });

            migrationBuilder.CreateIndex(
                name: "IX_RoomUsages_BookingRoomId",
                table: "RoomUsages",
                column: "BookingRoomId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoomUsages_RoomId",
                table: "RoomUsages",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomUsages_Rooms_RoomId",
                table: "RoomUsages",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoomUsages_Rooms_RoomId",
                table: "RoomUsages");

            migrationBuilder.DropIndex(
                name: "IX_RoomUsages_BookingRoomId",
                table: "RoomUsages");

            migrationBuilder.DropIndex(
                name: "IX_RoomUsages_RoomId",
                table: "RoomUsages");

            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "RoomUsages");

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "RoomAmenities",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b480e58f-14a5-414c-b54b-89d168f833b2"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "bf383414-5509-4a62-8c8f-ef49612efb5a", "AQAAAAIAAYagAAAAEDTlMC/hRn0yyogsmA5vT2nJANOBml3H6TFRMjzsHGoM8+O94LfD0wItbzG0APmEzQ==", "1ac3e1d0-2d07-4e3c-8104-1a1409e6571a" });

            migrationBuilder.CreateIndex(
                name: "IX_RoomUsages_BookingRoomId",
                table: "RoomUsages",
                column: "BookingRoomId");
        }
    }
}
