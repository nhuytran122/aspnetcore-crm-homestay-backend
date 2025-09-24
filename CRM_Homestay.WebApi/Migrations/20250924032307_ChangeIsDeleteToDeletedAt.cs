using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM_Homestay.App.Migrations
{
    /// <inheritdoc />
    public partial class ChangeIsDeleteToDeletedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingServices_Users_AssignedStaffId",
                table: "BookingServices");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "Customers");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Users",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Customers",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Bookings",
                type: "uuid",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b480e58f-14a5-414c-b54b-89d168f833b2"),
                columns: new[] { "ConcurrencyStamp", "DeletedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "9ba55b85-6318-4f19-82fe-4bc74cfd9c8b", null, "AQAAAAIAAYagAAAAEIKOKvUY5Yc+QQm2DPL3SIY++iyIU+fDEb183zMkUQwP+aDFbt/juZclRhKl+z+YHA==", "c7fd881c-cf62-436b-98ff-02614021266c" });

            migrationBuilder.CreateIndex(
                name: "IX_BookingServices_CreatorId",
                table: "BookingServices",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_UserId",
                table: "Bookings",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Users_UserId",
                table: "Bookings",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingServices_Users_AssignedStaffId",
                table: "BookingServices",
                column: "AssignedStaffId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingServices_Users_CreatorId",
                table: "BookingServices",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Users_UserId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_BookingServices_Users_AssignedStaffId",
                table: "BookingServices");

            migrationBuilder.DropForeignKey(
                name: "FK_BookingServices_Users_CreatorId",
                table: "BookingServices");

            migrationBuilder.DropIndex(
                name: "IX_BookingServices_CreatorId",
                table: "BookingServices");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_UserId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Bookings");

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "Customers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b480e58f-14a5-414c-b54b-89d168f833b2"),
                columns: new[] { "ConcurrencyStamp", "IsDelete", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c5e3979d-4e1d-44f4-a69f-4eea17da5481", false, "AQAAAAIAAYagAAAAEEZnSqVtF5KEkeWvoDkHidzrEvGt1uO9UOB/aZpUVHR6sO1pPpagDMyLU1PZdgfVsQ==", "f5969972-4b2c-405f-8b25-6c1e15a06d12" });

            migrationBuilder.AddForeignKey(
                name: "FK_BookingServices_Users_AssignedStaffId",
                table: "BookingServices",
                column: "AssignedStaffId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
