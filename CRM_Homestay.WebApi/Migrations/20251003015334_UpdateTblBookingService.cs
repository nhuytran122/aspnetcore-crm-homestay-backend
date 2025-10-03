using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM_Homestay.App.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTblBookingService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Quantity",
                table: "BookingServices",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("1977ace4-af1b-4344-8ec8-8e7c43a3ca2b"),
                column: "CreationTime",
                value: new DateTime(2025, 10, 3, 1, 53, 30, 994, DateTimeKind.Unspecified).AddTicks(8669));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("a34726e0-fe3f-442e-a337-48ae997becf0"),
                column: "CreationTime",
                value: new DateTime(2025, 10, 3, 1, 53, 30, 994, DateTimeKind.Unspecified).AddTicks(8664));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("a5511daa-0ef1-40c7-82bc-ca05ac3dc89a"),
                column: "CreationTime",
                value: new DateTime(2025, 10, 3, 1, 53, 30, 994, DateTimeKind.Unspecified).AddTicks(8676));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("b21c690b-4042-4407-9d78-e693fbf7ae46"),
                column: "CreationTime",
                value: new DateTime(2025, 10, 3, 1, 53, 30, 994, DateTimeKind.Unspecified).AddTicks(8667));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("d10dac54-b88e-4105-940b-34ad1b2bf4fd"),
                column: "CreationTime",
                value: new DateTime(2025, 10, 3, 1, 53, 30, 994, DateTimeKind.Unspecified).AddTicks(8640));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b480e58f-14a5-414c-b54b-89d168f833b2"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "02a469f3-8a74-4bbc-827b-6b9411159874", "AQAAAAIAAYagAAAAECN5LaOoq8W8oTqTjhu0ej6BF8mYpSV4ZAcpqvNxHquF1yYU7NTn0ybYEOQNJOR7oQ==", "3bb0b6dc-8edc-481d-a843-62ea22d5dae2" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Quantity",
                table: "BookingServices",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

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
    }
}
