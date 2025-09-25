using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM_Homestay.App.Migrations
{
    /// <inheritdoc />
    public partial class AddNormalizeFullInfoToEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Users_UserId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_UserId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Bookings");

            migrationBuilder.AddColumn<string>(
                name: "Brand",
                table: "ServiceItems",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Model",
                table: "ServiceItems",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NormalizeFullInfo",
                table: "ServiceItems",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "ServiceItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "NormalizeFullInfo",
                table: "Rules",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizeFullInfo",
                table: "RoomTypes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizeFullInfo",
                table: "Products",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizeFullInfo",
                table: "ProductCategories",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizeFullInfo",
                table: "HomestayServices",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizeFullInfo",
                table: "HomestayMaintenances",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizeFullInfo",
                table: "FAQs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizeFullInfo",
                table: "Amenities",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b480e58f-14a5-414c-b54b-89d168f833b2"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "bf383414-5509-4a62-8c8f-ef49612efb5a", "AQAAAAIAAYagAAAAEDTlMC/hRn0yyogsmA5vT2nJANOBml3H6TFRMjzsHGoM8+O94LfD0wItbzG0APmEzQ==", "1ac3e1d0-2d07-4e3c-8104-1a1409e6571a" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Brand",
                table: "ServiceItems");

            migrationBuilder.DropColumn(
                name: "Model",
                table: "ServiceItems");

            migrationBuilder.DropColumn(
                name: "NormalizeFullInfo",
                table: "ServiceItems");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "ServiceItems");

            migrationBuilder.DropColumn(
                name: "NormalizeFullInfo",
                table: "Rules");

            migrationBuilder.DropColumn(
                name: "NormalizeFullInfo",
                table: "RoomTypes");

            migrationBuilder.DropColumn(
                name: "NormalizeFullInfo",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "NormalizeFullInfo",
                table: "ProductCategories");

            migrationBuilder.DropColumn(
                name: "NormalizeFullInfo",
                table: "HomestayServices");

            migrationBuilder.DropColumn(
                name: "NormalizeFullInfo",
                table: "HomestayMaintenances");

            migrationBuilder.DropColumn(
                name: "NormalizeFullInfo",
                table: "FAQs");

            migrationBuilder.DropColumn(
                name: "NormalizeFullInfo",
                table: "Amenities");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Bookings",
                type: "uuid",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b480e58f-14a5-414c-b54b-89d168f833b2"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "9ba55b85-6318-4f19-82fe-4bc74cfd9c8b", "AQAAAAIAAYagAAAAEIKOKvUY5Yc+QQm2DPL3SIY++iyIU+fDEb183zMkUQwP+aDFbt/juZclRhKl+z+YHA==", "c7fd881c-cf62-436b-98ff-02614021266c" });

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
        }
    }
}
