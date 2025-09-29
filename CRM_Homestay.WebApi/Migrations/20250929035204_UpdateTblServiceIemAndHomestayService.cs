using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM_Homestay.App.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTblServiceIemAndHomestayService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Model",
                table: "ServiceItems",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Brand",
                table: "ServiceItems",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "HomestayServices",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b480e58f-14a5-414c-b54b-89d168f833b2"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "49c90969-dcf6-4c02-8a31-610477bafa56", "AQAAAAIAAYagAAAAEPPe5YXUV1V/89h0Z5BLSKloz0+CXTdixotHCg5/M9iheqtJ6aOB8mjyqeJ8puWMRg==", "c5b44011-9965-4908-adce-4dad993e8f90" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "HomestayServices");

            migrationBuilder.AlterColumn<string>(
                name: "Model",
                table: "ServiceItems",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Brand",
                table: "ServiceItems",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b480e58f-14a5-414c-b54b-89d168f833b2"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "476cc704-1af5-4a15-b110-6d58980c7225", "AQAAAAIAAYagAAAAENmFvd+9qAuJqDZEnPKw+t/8EhuKprya5sWZRJ344YcXmNvBNJmUh26G2RzxQ9ph7w==", "8aa9dbd7-2600-43c4-91c4-c688d0d72a24" });
        }
    }
}
