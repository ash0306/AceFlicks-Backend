using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieBookingBackend.Migrations
{
    public partial class UpdatedModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "SeatStatus",
                table: "Seats",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Movies",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Movies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Movies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 101,
                columns: new[] { "PasswordHash", "PasswordHashKey" },
                values: new object[] { new byte[] { 2, 179, 118, 79, 79, 51, 63, 219, 211, 145, 178, 252, 202, 20, 51, 182, 246, 48, 131, 112, 107, 100, 158, 183, 128, 215, 148, 152, 249, 249, 71, 19, 146, 45, 233, 142, 168, 86, 18, 203, 34, 84, 159, 92, 109, 198, 100, 169, 50, 49, 2, 161, 55, 215, 17, 208, 174, 210, 76, 24, 10, 243, 137, 239 }, new byte[] { 16, 107, 110, 10, 125, 170, 59, 102, 77, 129, 225, 170, 191, 103, 236, 67, 160, 230, 213, 43, 53, 166, 81, 181, 33, 177, 222, 69, 230, 28, 196, 131, 134, 79, 106, 74, 213, 24, 17, 11, 185, 4, 162, 168, 204, 186, 107, 38, 64, 94, 0, 71, 153, 199, 28, 31, 146, 79, 6, 117, 28, 68, 16, 104, 191, 33, 224, 151, 162, 219, 210, 118, 1, 40, 32, 153, 124, 115, 245, 129, 201, 244, 82, 228, 132, 69, 202, 181, 71, 12, 220, 123, 214, 154, 76, 141, 189, 28, 181, 75, 213, 59, 156, 251, 162, 146, 69, 98, 231, 111, 15, 14, 107, 203, 136, 244, 39, 179, 233, 87, 233, 192, 15, 109, 240, 43, 111, 129 } });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Movies_Title",
                table: "Movies",
                column: "Title",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Movies_Title",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "SeatStatus",
                table: "Seats");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Movies");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Movies",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 101,
                columns: new[] { "PasswordHash", "PasswordHashKey" },
                values: new object[] { new byte[] { 182, 61, 0, 125, 1, 237, 233, 25, 12, 40, 109, 24, 110, 233, 237, 26, 215, 83, 149, 154, 95, 1, 101, 135, 116, 176, 104, 27, 23, 111, 145, 51, 250, 196, 122, 167, 20, 90, 221, 239, 124, 196, 247, 163, 214, 177, 79, 122, 196, 27, 216, 143, 107, 72, 64, 47, 128, 152, 124, 124, 202, 103, 213, 42 }, new byte[] { 173, 136, 73, 163, 225, 52, 227, 212, 38, 172, 170, 37, 150, 135, 252, 14, 150, 151, 157, 70, 60, 141, 3, 63, 111, 230, 243, 17, 202, 243, 224, 224, 95, 223, 57, 201, 150, 253, 6, 233, 187, 181, 188, 48, 150, 36, 220, 12, 147, 74, 205, 183, 235, 40, 223, 77, 221, 124, 167, 62, 210, 140, 239, 212, 142, 99, 102, 69, 5, 27, 54, 119, 188, 78, 234, 211, 121, 56, 148, 51, 1, 224, 146, 147, 37, 19, 136, 18, 130, 155, 16, 44, 193, 160, 212, 25, 117, 164, 217, 145, 184, 126, 153, 124, 152, 8, 184, 224, 232, 123, 134, 112, 229, 50, 27, 64, 224, 227, 198, 242, 125, 28, 142, 63, 3, 29, 156, 134 } });
        }
    }
}
