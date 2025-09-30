using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM_Homestay.App.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTblCustomerGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "DiscountValue",
                table: "CustomerGroups",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<int>(
                name: "DiscountType",
                table: "CustomerGroups",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b480e58f-14a5-414c-b54b-89d168f833b2"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f1963e98-18e2-4592-8fec-bbd21b66ca74", "AQAAAAIAAYagAAAAEByAoCYTwjDPjO9+27MAkXrNQPEhfC7eAc9cs3PYxcwkLFYDBcvKb6ELTnT7BZWRqA==", "d054afb0-f2d3-4d3f-b4ce-f2cadf6eb5d5" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "DiscountValue",
                table: "CustomerGroups",
                type: "numeric",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DiscountType",
                table: "CustomerGroups",
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
                values: new object[] { "55383b9c-2ac8-4fb8-b68c-2ea30e85562f", "AQAAAAIAAYagAAAAEAZccybxJPUfamegw1GpkV1j9HdOj5MDbsk4SRQ/EVCsgBFVjWp0CDA8zMwQmERO5Q==", "5b0dbf98-ac88-47f4-b549-ec77c5fb3ce5" });
        }
    }
}
