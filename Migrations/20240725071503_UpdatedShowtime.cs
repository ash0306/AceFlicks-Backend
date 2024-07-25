using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieBookingBackend.Migrations
{
    public partial class UpdatedShowtime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AvailableSeats",
                table: "Showtimes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 101,
                columns: new[] { "PasswordHash", "PasswordHashKey" },
                values: new object[] { new byte[] { 208, 65, 187, 185, 125, 52, 187, 203, 245, 49, 170, 87, 78, 143, 50, 222, 89, 165, 64, 174, 99, 103, 66, 91, 17, 94, 101, 222, 94, 187, 244, 196, 136, 59, 81, 34, 134, 29, 145, 206, 11, 70, 110, 79, 52, 62, 145, 162, 166, 15, 40, 203, 216, 92, 17, 47, 47, 82, 173, 29, 228, 211, 188, 225 }, new byte[] { 202, 105, 107, 23, 164, 99, 160, 3, 185, 48, 199, 210, 56, 81, 73, 203, 91, 23, 189, 147, 43, 91, 87, 198, 246, 132, 116, 25, 254, 196, 200, 238, 148, 223, 112, 94, 244, 108, 184, 177, 188, 245, 99, 240, 222, 69, 16, 234, 14, 120, 163, 243, 9, 59, 26, 76, 166, 180, 98, 109, 2, 42, 115, 100, 127, 154, 233, 36, 222, 71, 10, 206, 180, 92, 133, 214, 169, 1, 72, 15, 176, 65, 28, 221, 131, 232, 81, 8, 135, 19, 220, 165, 90, 125, 73, 19, 1, 244, 99, 76, 242, 138, 199, 92, 157, 181, 102, 60, 62, 176, 72, 178, 91, 210, 27, 181, 207, 88, 16, 156, 150, 74, 97, 136, 228, 209, 130, 224 } });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvailableSeats",
                table: "Showtimes");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 101,
                columns: new[] { "PasswordHash", "PasswordHashKey" },
                values: new object[] { new byte[] { 2, 179, 118, 79, 79, 51, 63, 219, 211, 145, 178, 252, 202, 20, 51, 182, 246, 48, 131, 112, 107, 100, 158, 183, 128, 215, 148, 152, 249, 249, 71, 19, 146, 45, 233, 142, 168, 86, 18, 203, 34, 84, 159, 92, 109, 198, 100, 169, 50, 49, 2, 161, 55, 215, 17, 208, 174, 210, 76, 24, 10, 243, 137, 239 }, new byte[] { 16, 107, 110, 10, 125, 170, 59, 102, 77, 129, 225, 170, 191, 103, 236, 67, 160, 230, 213, 43, 53, 166, 81, 181, 33, 177, 222, 69, 230, 28, 196, 131, 134, 79, 106, 74, 213, 24, 17, 11, 185, 4, 162, 168, 204, 186, 107, 38, 64, 94, 0, 71, 153, 199, 28, 31, 146, 79, 6, 117, 28, 68, 16, 104, 191, 33, 224, 151, 162, 219, 210, 118, 1, 40, 32, 153, 124, 115, 245, 129, 201, 244, 82, 228, 132, 69, 202, 181, 71, 12, 220, 123, 214, 154, 76, 141, 189, 28, 181, 75, 213, 59, 156, 251, 162, 146, 69, 98, 231, 111, 15, 14, 107, 203, 136, 244, 39, 179, 233, 87, 233, 192, 15, 109, 240, 43, 111, 129 } });
        }
    }
}
