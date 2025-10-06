using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM_Homestay.App.Migrations
{
    /// <inheritdoc />
    public partial class AddTableCustomerPoint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomerPoints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    TotalAccumulated = table.Column<int>(type: "integer", nullable: false),
                    TotalUsed = table.Column<int>(type: "integer", nullable: false),
                    CurrentBalance = table.Column<int>(type: "integer", nullable: false),
                    HoldPoint = table.Column<int>(type: "integer", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerPoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerPoints_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("03a1f3c3-baf7-41f1-bc4b-36f7a6e8a008"),
                column: "CreationTime",
                value: new DateTime(2025, 10, 6, 2, 47, 24, 497, DateTimeKind.Unspecified).AddTicks(8955));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("1977ace4-af1b-4344-8ec8-8e7c43a3ca2b"),
                column: "CreationTime",
                value: new DateTime(2025, 10, 6, 2, 47, 24, 497, DateTimeKind.Unspecified).AddTicks(8875));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("2a1d4c5e-21f0-4a3e-9c92-58f5b8fbb002"),
                column: "CreationTime",
                value: new DateTime(2025, 10, 6, 2, 47, 24, 497, DateTimeKind.Unspecified).AddTicks(8926));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("4b7e5f5a-55fd-46ab-8576-739b7c1da005"),
                column: "CreationTime",
                value: new DateTime(2025, 10, 6, 2, 47, 24, 497, DateTimeKind.Unspecified).AddTicks(8932));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("8c17c3f1-d59e-45f9-9f63-81d85c8da009"),
                column: "CreationTime",
                value: new DateTime(2025, 10, 6, 2, 47, 24, 497, DateTimeKind.Unspecified).AddTicks(8957));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("9a5c8a90-0a04-45a3-bef0-cc66d8b1a007"),
                column: "CreationTime",
                value: new DateTime(2025, 10, 6, 2, 47, 24, 497, DateTimeKind.Unspecified).AddTicks(8951));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("a34726e0-fe3f-442e-a337-48ae997becf0"),
                column: "CreationTime",
                value: new DateTime(2025, 10, 6, 2, 47, 24, 497, DateTimeKind.Unspecified).AddTicks(8869));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("a5511daa-0ef1-40c7-82bc-ca05ac3dc89a"),
                column: "CreationTime",
                value: new DateTime(2025, 10, 6, 2, 47, 24, 497, DateTimeKind.Unspecified).AddTicks(8885));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("b21c690b-4042-4407-9d78-e693fbf7ae46"),
                column: "CreationTime",
                value: new DateTime(2025, 10, 6, 2, 47, 24, 497, DateTimeKind.Unspecified).AddTicks(8872));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("b39c2ff4-41e8-4663-96e1-bf9d6b3fa003"),
                column: "CreationTime",
                value: new DateTime(2025, 10, 6, 2, 47, 24, 497, DateTimeKind.Unspecified).AddTicks(8928));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("c6e1d902-7dcb-4f37-8c14-944feebca004"),
                column: "CreationTime",
                value: new DateTime(2025, 10, 6, 2, 47, 24, 497, DateTimeKind.Unspecified).AddTicks(8930));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("d10dac54-b88e-4105-940b-34ad1b2bf4fd"),
                column: "CreationTime",
                value: new DateTime(2025, 10, 6, 2, 47, 24, 497, DateTimeKind.Unspecified).AddTicks(8841));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("d19f45f8-0f47-4d65-8f8d-0b64e1dda006"),
                column: "CreationTime",
                value: new DateTime(2025, 10, 6, 2, 47, 24, 497, DateTimeKind.Unspecified).AddTicks(8933));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("f6c8d07a-5a64-4f17-b36c-7a42b9c8a001"),
                column: "CreationTime",
                value: new DateTime(2025, 10, 6, 2, 47, 24, 497, DateTimeKind.Unspecified).AddTicks(8924));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b480e58f-14a5-414c-b54b-89d168f833b2"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "628e77ab-ab9f-4f5d-9837-0121df18abab", "AQAAAAIAAYagAAAAEHJn9lAPSQbFX96It3Rj9ZGGi9O+gtaQmYf5o9xNLTVtJNStf/72hsmElwuNknYQ8g==", "b1bc941a-1c7b-4b4d-a968-4e9115efcf95" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerPoints_CustomerId",
                table: "CustomerPoints",
                column: "CustomerId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerPoints");

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("03a1f3c3-baf7-41f1-bc4b-36f7a6e8a008"),
                column: "CreationTime",
                value: new DateTime(2025, 10, 3, 8, 57, 11, 719, DateTimeKind.Unspecified).AddTicks(3577));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("1977ace4-af1b-4344-8ec8-8e7c43a3ca2b"),
                column: "CreationTime",
                value: new DateTime(2025, 10, 3, 8, 57, 11, 719, DateTimeKind.Unspecified).AddTicks(3481));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("2a1d4c5e-21f0-4a3e-9c92-58f5b8fbb002"),
                column: "CreationTime",
                value: new DateTime(2025, 10, 3, 8, 57, 11, 719, DateTimeKind.Unspecified).AddTicks(3541));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("4b7e5f5a-55fd-46ab-8576-739b7c1da005"),
                column: "CreationTime",
                value: new DateTime(2025, 10, 3, 8, 57, 11, 719, DateTimeKind.Unspecified).AddTicks(3552));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("8c17c3f1-d59e-45f9-9f63-81d85c8da009"),
                column: "CreationTime",
                value: new DateTime(2025, 10, 3, 8, 57, 11, 719, DateTimeKind.Unspecified).AddTicks(3579));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("9a5c8a90-0a04-45a3-bef0-cc66d8b1a007"),
                column: "CreationTime",
                value: new DateTime(2025, 10, 3, 8, 57, 11, 719, DateTimeKind.Unspecified).AddTicks(3574));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("a34726e0-fe3f-442e-a337-48ae997becf0"),
                column: "CreationTime",
                value: new DateTime(2025, 10, 3, 8, 57, 11, 719, DateTimeKind.Unspecified).AddTicks(3475));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("a5511daa-0ef1-40c7-82bc-ca05ac3dc89a"),
                column: "CreationTime",
                value: new DateTime(2025, 10, 3, 8, 57, 11, 719, DateTimeKind.Unspecified).AddTicks(3485));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("b21c690b-4042-4407-9d78-e693fbf7ae46"),
                column: "CreationTime",
                value: new DateTime(2025, 10, 3, 8, 57, 11, 719, DateTimeKind.Unspecified).AddTicks(3479));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("b39c2ff4-41e8-4663-96e1-bf9d6b3fa003"),
                column: "CreationTime",
                value: new DateTime(2025, 10, 3, 8, 57, 11, 719, DateTimeKind.Unspecified).AddTicks(3544));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("c6e1d902-7dcb-4f37-8c14-944feebca004"),
                column: "CreationTime",
                value: new DateTime(2025, 10, 3, 8, 57, 11, 719, DateTimeKind.Unspecified).AddTicks(3549));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("d10dac54-b88e-4105-940b-34ad1b2bf4fd"),
                column: "CreationTime",
                value: new DateTime(2025, 10, 3, 8, 57, 11, 719, DateTimeKind.Unspecified).AddTicks(3448));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("d19f45f8-0f47-4d65-8f8d-0b64e1dda006"),
                column: "CreationTime",
                value: new DateTime(2025, 10, 3, 8, 57, 11, 719, DateTimeKind.Unspecified).AddTicks(3555));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("f6c8d07a-5a64-4f17-b36c-7a42b9c8a001"),
                column: "CreationTime",
                value: new DateTime(2025, 10, 3, 8, 57, 11, 719, DateTimeKind.Unspecified).AddTicks(3537));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b480e58f-14a5-414c-b54b-89d168f833b2"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "9c2f79e1-f907-406a-9c35-d0710d1bb1f8", "AQAAAAIAAYagAAAAEJp67uijzY3qqqOyKGdP5BpDr9SJSeXjpKHtDLNVbCBLrXP5QIpWAijI/TK9S4nVIg==", "bf10be19-d21b-4a42-ab29-bb7eeae074ea" });
        }
    }
}
