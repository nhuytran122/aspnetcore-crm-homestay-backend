using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM_Homestay.App.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTableServiceItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceItems_HomestayServices_HomestayServiceId",
                table: "ServiceItems");

            migrationBuilder.DropColumn(
                name: "ServiceId",
                table: "ServiceItems");

            migrationBuilder.AlterColumn<Guid>(
                name: "HomestayServiceId",
                table: "ServiceItems",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b480e58f-14a5-414c-b54b-89d168f833b2"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "55383b9c-2ac8-4fb8-b68c-2ea30e85562f", "AQAAAAIAAYagAAAAEAZccybxJPUfamegw1GpkV1j9HdOj5MDbsk4SRQ/EVCsgBFVjWp0CDA8zMwQmERO5Q==", "5b0dbf98-ac88-47f4-b549-ec77c5fb3ce5" });

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceItems_HomestayServices_HomestayServiceId",
                table: "ServiceItems",
                column: "HomestayServiceId",
                principalTable: "HomestayServices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceItems_HomestayServices_HomestayServiceId",
                table: "ServiceItems");

            migrationBuilder.AlterColumn<Guid>(
                name: "HomestayServiceId",
                table: "ServiceItems",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "ServiceId",
                table: "ServiceItems",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b480e58f-14a5-414c-b54b-89d168f833b2"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "49c90969-dcf6-4c02-8a31-610477bafa56", "AQAAAAIAAYagAAAAEPPe5YXUV1V/89h0Z5BLSKloz0+CXTdixotHCg5/M9iheqtJ6aOB8mjyqeJ8puWMRg==", "c5b44011-9965-4908-adce-4dad993e8f90" });

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceItems_HomestayServices_HomestayServiceId",
                table: "ServiceItems",
                column: "HomestayServiceId",
                principalTable: "HomestayServices",
                principalColumn: "Id");
        }
    }
}
