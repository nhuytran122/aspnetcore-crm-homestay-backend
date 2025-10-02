using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM_Homestay.App.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRelationshipInTblBookingRoom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RoomUsages_BookingRoomId",
                table: "RoomUsages");

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("1977ace4-af1b-4344-8ec8-8e7c43a3ca2b"),
                column: "CreationTime",
                value: new DateTime(2025, 10, 2, 10, 20, 19, 851, DateTimeKind.Unspecified).AddTicks(9150));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("a34726e0-fe3f-442e-a337-48ae997becf0"),
                column: "CreationTime",
                value: new DateTime(2025, 10, 2, 10, 20, 19, 851, DateTimeKind.Unspecified).AddTicks(9144));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("a5511daa-0ef1-40c7-82bc-ca05ac3dc89a"),
                column: "CreationTime",
                value: new DateTime(2025, 10, 2, 10, 20, 19, 851, DateTimeKind.Unspecified).AddTicks(9152));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("b21c690b-4042-4407-9d78-e693fbf7ae46"),
                column: "CreationTime",
                value: new DateTime(2025, 10, 2, 10, 20, 19, 851, DateTimeKind.Unspecified).AddTicks(9147));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("d10dac54-b88e-4105-940b-34ad1b2bf4fd"),
                column: "CreationTime",
                value: new DateTime(2025, 10, 2, 10, 20, 19, 851, DateTimeKind.Unspecified).AddTicks(9113));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b480e58f-14a5-414c-b54b-89d168f833b2"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ac70fabb-8e6f-4a20-8df6-2b630186a2b5", "AQAAAAIAAYagAAAAECx3pjNN8lutx/7LHjROLI5ms8qvf5J3xFCAtYpgZolMs2Th1XudLDz8++u491M6HA==", "f96fa495-c875-4e96-97a3-e3cdf6634635" });

            migrationBuilder.CreateIndex(
                name: "IX_RoomUsages_BookingRoomId",
                table: "RoomUsages",
                column: "BookingRoomId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RoomUsages_BookingRoomId",
                table: "RoomUsages");

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("1977ace4-af1b-4344-8ec8-8e7c43a3ca2b"),
                column: "CreationTime",
                value: new DateTime(2025, 10, 2, 2, 38, 9, 253, DateTimeKind.Unspecified).AddTicks(3614));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("a34726e0-fe3f-442e-a337-48ae997becf0"),
                column: "CreationTime",
                value: new DateTime(2025, 10, 2, 2, 38, 9, 253, DateTimeKind.Unspecified).AddTicks(3602));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("a5511daa-0ef1-40c7-82bc-ca05ac3dc89a"),
                column: "CreationTime",
                value: new DateTime(2025, 10, 2, 2, 38, 9, 253, DateTimeKind.Unspecified).AddTicks(3637));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("b21c690b-4042-4407-9d78-e693fbf7ae46"),
                column: "CreationTime",
                value: new DateTime(2025, 10, 2, 2, 38, 9, 253, DateTimeKind.Unspecified).AddTicks(3609));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("d10dac54-b88e-4105-940b-34ad1b2bf4fd"),
                column: "CreationTime",
                value: new DateTime(2025, 10, 2, 2, 38, 9, 253, DateTimeKind.Unspecified).AddTicks(3560));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b480e58f-14a5-414c-b54b-89d168f833b2"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f3f0cda9-5c44-4063-9147-1d413cb27add", "AQAAAAIAAYagAAAAEKDHiJzgsvGcbd9EDQb2hVvI/pVCSRN4MlGpXxl4jdjnorh6oyQpxuuZ0UZlcxfFPg==", "362d4fd2-515b-4a61-9678-a551e07877b4" });

            migrationBuilder.CreateIndex(
                name: "IX_RoomUsages_BookingRoomId",
                table: "RoomUsages",
                column: "BookingRoomId",
                unique: true);
        }
    }
}
