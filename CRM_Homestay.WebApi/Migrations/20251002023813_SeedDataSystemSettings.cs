using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CRM_Homestay.App.Migrations
{
    /// <inheritdoc />
    public partial class SeedDataSystemSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "SystemSettings",
                columns: new[] { "Id", "ConfigKey", "ConfigValue", "CreationTime", "CreatorId", "Description", "LastModificationTime", "LastModifierId", "Sort", "SystemName" },
                values: new object[,]
                {
                    { new Guid("1977ace4-af1b-4344-8ec8-8e7c43a3ca2b"), "DiscountType", "2", new DateTime(2025, 10, 2, 2, 38, 9, 253, DateTimeKind.Unspecified).AddTicks(3614), null, null, null, null, 0, "IncentiveCoupon" },
                    { new Guid("a34726e0-fe3f-442e-a337-48ae997becf0"), "OvernightStartTime", "22:00:00", new DateTime(2025, 10, 2, 2, 38, 9, 253, DateTimeKind.Unspecified).AddTicks(3602), null, "Thời gian bắt đầu tính giá qua đêm (giờ check-in được tính là giá qua đêm nếu >= thời gian này)", null, null, 0, "RoomPricing" },
                    { new Guid("a5511daa-0ef1-40c7-82bc-ca05ac3dc89a"), "DiscountValue", "10000", new DateTime(2025, 10, 2, 2, 38, 9, 253, DateTimeKind.Unspecified).AddTicks(3637), null, null, null, null, 0, "IncentiveCoupon" },
                    { new Guid("b21c690b-4042-4407-9d78-e693fbf7ae46"), "OvernightEndTime", "08:00:00", new DateTime(2025, 10, 2, 2, 38, 9, 253, DateTimeKind.Unspecified).AddTicks(3609), null, "Thời gian kết thúc tính giá qua đêm (giờ check-out được tính là giá qua đêm nếu <= thời gian này)", null, null, 0, "RoomPricing" },
                    { new Guid("d10dac54-b88e-4105-940b-34ad1b2bf4fd"), "CleaningMinutes", "60", new DateTime(2025, 10, 2, 2, 38, 9, 253, DateTimeKind.Unspecified).AddTicks(3560), null, "Thời gian (phút) dọn dẹp phòng", null, null, 0, "RoomUsage" }
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b480e58f-14a5-414c-b54b-89d168f833b2"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f3f0cda9-5c44-4063-9147-1d413cb27add", "AQAAAAIAAYagAAAAEKDHiJzgsvGcbd9EDQb2hVvI/pVCSRN4MlGpXxl4jdjnorh6oyQpxuuZ0UZlcxfFPg==", "362d4fd2-515b-4a61-9678-a551e07877b4" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("1977ace4-af1b-4344-8ec8-8e7c43a3ca2b"));

            migrationBuilder.DeleteData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("a34726e0-fe3f-442e-a337-48ae997becf0"));

            migrationBuilder.DeleteData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("a5511daa-0ef1-40c7-82bc-ca05ac3dc89a"));

            migrationBuilder.DeleteData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("b21c690b-4042-4407-9d78-e693fbf7ae46"));

            migrationBuilder.DeleteData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("d10dac54-b88e-4105-940b-34ad1b2bf4fd"));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b480e58f-14a5-414c-b54b-89d168f833b2"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f1963e98-18e2-4592-8fec-bbd21b66ca74", "AQAAAAIAAYagAAAAEByAoCYTwjDPjO9+27MAkXrNQPEhfC7eAc9cs3PYxcwkLFYDBcvKb6ELTnT7BZWRqA==", "d054afb0-f2d3-4d3f-b4ce-f2cadf6eb5d5" });
        }
    }
}
