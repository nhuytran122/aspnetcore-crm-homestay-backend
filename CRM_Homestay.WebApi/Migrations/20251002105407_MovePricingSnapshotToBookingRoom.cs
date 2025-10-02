using System;
using CRM_Homestay.Core.Models;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM_Homestay.App.Migrations
{
    /// <inheritdoc />
    public partial class MovePricingSnapshotToBookingRoom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PricingSnapshot",
                table: "Bookings");

            migrationBuilder.AddColumn<BookingPricingSnapshot>(
                name: "PricingSnapshot",
                table: "BookingRooms",
                type: "json",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("1977ace4-af1b-4344-8ec8-8e7c43a3ca2b"),
                column: "CreationTime",
                value: new DateTime(2025, 10, 2, 10, 54, 4, 439, DateTimeKind.Unspecified).AddTicks(3971));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("a34726e0-fe3f-442e-a337-48ae997becf0"),
                column: "CreationTime",
                value: new DateTime(2025, 10, 2, 10, 54, 4, 439, DateTimeKind.Unspecified).AddTicks(3964));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("a5511daa-0ef1-40c7-82bc-ca05ac3dc89a"),
                column: "CreationTime",
                value: new DateTime(2025, 10, 2, 10, 54, 4, 439, DateTimeKind.Unspecified).AddTicks(3973));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("b21c690b-4042-4407-9d78-e693fbf7ae46"),
                column: "CreationTime",
                value: new DateTime(2025, 10, 2, 10, 54, 4, 439, DateTimeKind.Unspecified).AddTicks(3969));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("d10dac54-b88e-4105-940b-34ad1b2bf4fd"),
                column: "CreationTime",
                value: new DateTime(2025, 10, 2, 10, 54, 4, 439, DateTimeKind.Unspecified).AddTicks(3912));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b480e58f-14a5-414c-b54b-89d168f833b2"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3ce6c1e2-7d86-4ec2-98e7-65b6dda69d69", "AQAAAAIAAYagAAAAEDlUyIlJJGyIsrm5hvW5rezBHTtWzUpMtb7o6EH158NmGyJoASqodwg4GMo7GES1UA==", "157525ff-75d3-4636-9c6e-5d234ad9343b" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PricingSnapshot",
                table: "BookingRooms");

            migrationBuilder.AddColumn<BookingPricingSnapshot>(
                name: "PricingSnapshot",
                table: "Bookings",
                type: "json",
                nullable: true);

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
        }
    }
}
