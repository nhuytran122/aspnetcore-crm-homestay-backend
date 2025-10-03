using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CRM_Homestay.App.Migrations
{
    /// <inheritdoc />
    public partial class AddTablesRelatedToOtpModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OtpCodes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RecipientTypes = table.Column<string>(type: "text", nullable: false),
                    Recipient = table.Column<string>(type: "text", nullable: false),
                    Purpose = table.Column<string>(type: "text", nullable: false),
                    ReferenceTypes = table.Column<string>(type: "text", nullable: false),
                    ReferenceId = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Minutes = table.Column<int>(type: "integer", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Attempts = table.Column<byte>(type: "smallint", nullable: false),
                    MaxAttempts = table.Column<byte>(type: "smallint", nullable: false),
                    LockedUntil = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OtpCodes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OtpProviderLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OtpCodeId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProviderName = table.Column<string>(type: "text", nullable: false),
                    RequestPayload = table.Column<string>(type: "text", nullable: true),
                    ResponsePayload = table.Column<string>(type: "text", nullable: true),
                    StatusCode = table.Column<int>(type: "integer", nullable: true),
                    ErrorMessage = table.Column<string>(type: "text", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OtpProviderLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OtpProviderLogs_OtpCodes_OtpCodeId",
                        column: x => x.OtpCodeId,
                        principalTable: "OtpCodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OtpRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OtpCodeId = table.Column<Guid>(type: "uuid", nullable: true),
                    ReferenceTypes = table.Column<string>(type: "text", nullable: true),
                    ReferenceId = table.Column<string>(type: "text", nullable: true),
                    Recipient = table.Column<string>(type: "text", nullable: false),
                    ClientIp = table.Column<string>(type: "text", nullable: true),
                    UserAgent = table.Column<string>(type: "text", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OtpRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OtpRequests_OtpCodes_OtpCodeId",
                        column: x => x.OtpCodeId,
                        principalTable: "OtpCodes",
                        principalColumn: "Id");
                });

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("1977ace4-af1b-4344-8ec8-8e7c43a3ca2b"),
                column: "CreationTime",
                value: new DateTime(2025, 10, 3, 8, 57, 11, 719, DateTimeKind.Unspecified).AddTicks(3481));

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
                keyValue: new Guid("d10dac54-b88e-4105-940b-34ad1b2bf4fd"),
                column: "CreationTime",
                value: new DateTime(2025, 10, 3, 8, 57, 11, 719, DateTimeKind.Unspecified).AddTicks(3448));

            migrationBuilder.InsertData(
                table: "SystemSettings",
                columns: new[] { "Id", "ConfigKey", "ConfigValue", "CreationTime", "CreatorId", "Description", "LastModificationTime", "LastModifierId", "Sort", "SystemName" },
                values: new object[,]
                {
                    { new Guid("03a1f3c3-baf7-41f1-bc4b-36f7a6e8a008"), "EXPIRES_IN", "2025-07-31T10:41:12.8294958Z", new DateTime(2025, 10, 3, 8, 57, 11, 719, DateTimeKind.Unspecified).AddTicks(3577), null, null, null, null, 0, "Zalo_OA" },
                    { new Guid("2a1d4c5e-21f0-4a3e-9c92-58f5b8fbb002"), "TEMPLATE_ID", "956856", new DateTime(2025, 10, 3, 8, 57, 11, 719, DateTimeKind.Unspecified).AddTicks(3541), null, null, null, null, 0, "Zalo_OA" },
                    { new Guid("4b7e5f5a-55fd-46ab-8576-739b7c1da005"), "SECRET_KEY", "jhtitiytmjhjhkjkh****", new DateTime(2025, 10, 3, 8, 57, 11, 719, DateTimeKind.Unspecified).AddTicks(3552), null, null, null, null, 0, "Zalo_OA" },
                    { new Guid("8c17c3f1-d59e-45f9-9f63-81d85c8da009"), "COUNT_TO_LOCK", "4", new DateTime(2025, 10, 3, 8, 57, 11, 719, DateTimeKind.Unspecified).AddTicks(3579), null, null, null, null, 0, "Zalo_OA" },
                    { new Guid("9a5c8a90-0a04-45a3-bef0-cc66d8b1a007"), "ACCESS_TOKEN", "O_w29KxEtbnY_jklkl6Nv******", new DateTime(2025, 10, 3, 8, 57, 11, 719, DateTimeKind.Unspecified).AddTicks(3574), null, null, null, null, 0, "Zalo_OA" },
                    { new Guid("b39c2ff4-41e8-4663-96e1-bf9d6b3fa003"), "ADMIN_PHONE", "0987456321", new DateTime(2025, 10, 3, 8, 57, 11, 719, DateTimeKind.Unspecified).AddTicks(3544), null, null, null, null, 0, "Zalo_OA" },
                    { new Guid("c6e1d902-7dcb-4f37-8c14-944feebca004"), "APP_ID", "9652566698963****", new DateTime(2025, 10, 3, 8, 57, 11, 719, DateTimeKind.Unspecified).AddTicks(3549), null, null, null, null, 0, "Zalo_OA" },
                    { new Guid("d19f45f8-0f47-4d65-8f8d-0b64e1dda006"), "REFRESH_TOKEN", "wxKYU4QW_aRgj2LwGljktytyutyu********", new DateTime(2025, 10, 3, 8, 57, 11, 719, DateTimeKind.Unspecified).AddTicks(3555), null, null, null, null, 0, "Zalo_OA" },
                    { new Guid("f6c8d07a-5a64-4f17-b36c-7a42b9c8a001"), "DEVELOPMENT_MODE", "development", new DateTime(2025, 10, 3, 8, 57, 11, 719, DateTimeKind.Unspecified).AddTicks(3537), null, null, null, null, 0, "Zalo_OA" }
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b480e58f-14a5-414c-b54b-89d168f833b2"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "9c2f79e1-f907-406a-9c35-d0710d1bb1f8", "AQAAAAIAAYagAAAAEJp67uijzY3qqqOyKGdP5BpDr9SJSeXjpKHtDLNVbCBLrXP5QIpWAijI/TK9S4nVIg==", "bf10be19-d21b-4a42-ab29-bb7eeae074ea" });

            migrationBuilder.CreateIndex(
                name: "IX_OtpProviderLogs_OtpCodeId",
                table: "OtpProviderLogs",
                column: "OtpCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_OtpRequests_OtpCodeId",
                table: "OtpRequests",
                column: "OtpCodeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OtpProviderLogs");

            migrationBuilder.DropTable(
                name: "OtpRequests");

            migrationBuilder.DropTable(
                name: "OtpCodes");

            migrationBuilder.DeleteData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("03a1f3c3-baf7-41f1-bc4b-36f7a6e8a008"));

            migrationBuilder.DeleteData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("2a1d4c5e-21f0-4a3e-9c92-58f5b8fbb002"));

            migrationBuilder.DeleteData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("4b7e5f5a-55fd-46ab-8576-739b7c1da005"));

            migrationBuilder.DeleteData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("8c17c3f1-d59e-45f9-9f63-81d85c8da009"));

            migrationBuilder.DeleteData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("9a5c8a90-0a04-45a3-bef0-cc66d8b1a007"));

            migrationBuilder.DeleteData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("b39c2ff4-41e8-4663-96e1-bf9d6b3fa003"));

            migrationBuilder.DeleteData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("c6e1d902-7dcb-4f37-8c14-944feebca004"));

            migrationBuilder.DeleteData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("d19f45f8-0f47-4d65-8f8d-0b64e1dda006"));

            migrationBuilder.DeleteData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("f6c8d07a-5a64-4f17-b36c-7a42b9c8a001"));

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
    }
}
