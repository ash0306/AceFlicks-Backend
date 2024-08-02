using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieBookingBackend.Migrations
{
    public partial class UpdatedQR : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QRCode_Bookings_BookingId",
                table: "QRCode");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QRCode",
                table: "QRCode");

            migrationBuilder.RenameTable(
                name: "QRCode",
                newName: "QRCodes");

            migrationBuilder.RenameIndex(
                name: "IX_QRCode_BookingId",
                table: "QRCodes",
                newName: "IX_QRCodes_BookingId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QRCodes",
                table: "QRCodes",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 101,
                columns: new[] { "PasswordHash", "PasswordHashKey" },
                values: new object[] { new byte[] { 24, 225, 184, 105, 150, 77, 98, 220, 218, 247, 41, 225, 111, 143, 136, 231, 46, 19, 134, 24, 175, 55, 26, 135, 80, 213, 70, 135, 77, 112, 139, 97, 215, 242, 80, 155, 67, 165, 245, 177, 67, 116, 109, 223, 156, 204, 12, 37, 124, 182, 35, 146, 151, 221, 88, 196, 6, 51, 48, 238, 62, 185, 62, 50 }, new byte[] { 126, 48, 210, 231, 52, 171, 145, 160, 184, 159, 21, 111, 214, 198, 32, 197, 227, 73, 241, 105, 74, 124, 124, 240, 202, 60, 79, 234, 29, 93, 83, 149, 233, 149, 20, 55, 215, 241, 230, 4, 95, 157, 211, 133, 26, 128, 181, 102, 116, 164, 231, 22, 88, 127, 129, 142, 108, 3, 254, 131, 83, 191, 184, 67, 255, 123, 10, 162, 82, 238, 251, 224, 45, 178, 97, 166, 109, 240, 221, 228, 156, 85, 133, 198, 252, 94, 121, 217, 220, 26, 53, 120, 194, 211, 218, 251, 246, 156, 179, 169, 64, 38, 50, 140, 103, 166, 59, 197, 130, 206, 134, 244, 17, 84, 200, 93, 208, 159, 18, 108, 237, 83, 238, 128, 15, 37, 161, 113 } });

            migrationBuilder.AddForeignKey(
                name: "FK_QRCodes_Bookings_BookingId",
                table: "QRCodes",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QRCodes_Bookings_BookingId",
                table: "QRCodes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QRCodes",
                table: "QRCodes");

            migrationBuilder.RenameTable(
                name: "QRCodes",
                newName: "QRCode");

            migrationBuilder.RenameIndex(
                name: "IX_QRCodes_BookingId",
                table: "QRCode",
                newName: "IX_QRCode_BookingId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QRCode",
                table: "QRCode",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 101,
                columns: new[] { "PasswordHash", "PasswordHashKey" },
                values: new object[] { new byte[] { 209, 90, 88, 211, 138, 184, 250, 181, 184, 120, 200, 216, 249, 54, 45, 139, 149, 95, 51, 54, 148, 95, 207, 0, 121, 19, 229, 204, 116, 143, 61, 43, 79, 16, 236, 219, 210, 146, 170, 15, 253, 174, 187, 42, 146, 48, 138, 114, 193, 136, 91, 236, 41, 140, 216, 247, 117, 65, 48, 204, 188, 102, 6, 134 }, new byte[] { 139, 197, 114, 138, 46, 115, 86, 30, 51, 1, 69, 65, 163, 182, 22, 165, 198, 69, 252, 85, 24, 57, 138, 242, 17, 177, 238, 151, 22, 34, 211, 85, 186, 250, 119, 222, 14, 70, 223, 170, 119, 23, 249, 69, 111, 159, 167, 213, 179, 56, 11, 82, 51, 49, 170, 181, 20, 239, 23, 139, 50, 66, 44, 47, 147, 250, 187, 87, 235, 176, 181, 72, 42, 73, 45, 47, 52, 84, 254, 35, 58, 223, 67, 112, 47, 91, 242, 33, 151, 22, 206, 176, 241, 180, 110, 249, 28, 41, 113, 61, 165, 220, 182, 3, 184, 141, 151, 23, 185, 1, 37, 80, 213, 77, 241, 100, 69, 123, 7, 176, 192, 2, 126, 189, 158, 104, 66, 143 } });

            migrationBuilder.AddForeignKey(
                name: "FK_QRCode_Bookings_BookingId",
                table: "QRCode",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
