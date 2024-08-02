using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieBookingBackend.Migrations
{
    public partial class AddedQR : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QRId",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "QRCode",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingId = table.Column<int>(type: "int", nullable: false),
                    BookingQR = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QRCode", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QRCode_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 101,
                columns: new[] { "PasswordHash", "PasswordHashKey" },
                values: new object[] { new byte[] { 209, 90, 88, 211, 138, 184, 250, 181, 184, 120, 200, 216, 249, 54, 45, 139, 149, 95, 51, 54, 148, 95, 207, 0, 121, 19, 229, 204, 116, 143, 61, 43, 79, 16, 236, 219, 210, 146, 170, 15, 253, 174, 187, 42, 146, 48, 138, 114, 193, 136, 91, 236, 41, 140, 216, 247, 117, 65, 48, 204, 188, 102, 6, 134 }, new byte[] { 139, 197, 114, 138, 46, 115, 86, 30, 51, 1, 69, 65, 163, 182, 22, 165, 198, 69, 252, 85, 24, 57, 138, 242, 17, 177, 238, 151, 22, 34, 211, 85, 186, 250, 119, 222, 14, 70, 223, 170, 119, 23, 249, 69, 111, 159, 167, 213, 179, 56, 11, 82, 51, 49, 170, 181, 20, 239, 23, 139, 50, 66, 44, 47, 147, 250, 187, 87, 235, 176, 181, 72, 42, 73, 45, 47, 52, 84, 254, 35, 58, 223, 67, 112, 47, 91, 242, 33, 151, 22, 206, 176, 241, 180, 110, 249, 28, 41, 113, 61, 165, 220, 182, 3, 184, 141, 151, 23, 185, 1, 37, 80, 213, 77, 241, 100, 69, 123, 7, 176, 192, 2, 126, 189, 158, 104, 66, 143 } });

            migrationBuilder.CreateIndex(
                name: "IX_QRCode_BookingId",
                table: "QRCode",
                column: "BookingId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QRCode");

            migrationBuilder.DropColumn(
                name: "QRId",
                table: "Bookings");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 101,
                columns: new[] { "PasswordHash", "PasswordHashKey" },
                values: new object[] { new byte[] { 178, 110, 9, 32, 247, 109, 98, 90, 100, 42, 134, 0, 250, 160, 242, 113, 65, 177, 38, 103, 5, 242, 19, 255, 128, 226, 86, 183, 65, 176, 59, 58, 182, 192, 29, 58, 236, 208, 14, 93, 9, 147, 206, 48, 45, 167, 249, 24, 60, 11, 82, 253, 122, 40, 221, 27, 159, 207, 84, 168, 74, 98, 13, 147 }, new byte[] { 34, 164, 80, 109, 63, 187, 166, 177, 159, 3, 4, 57, 102, 242, 35, 181, 27, 191, 13, 228, 50, 70, 195, 80, 38, 188, 124, 186, 68, 107, 5, 109, 209, 176, 222, 30, 122, 105, 64, 249, 6, 149, 200, 250, 175, 98, 73, 158, 224, 47, 111, 117, 29, 176, 189, 6, 163, 40, 26, 75, 70, 85, 201, 130, 131, 250, 45, 160, 110, 99, 182, 15, 75, 113, 55, 192, 51, 182, 215, 19, 117, 194, 192, 203, 131, 39, 187, 123, 34, 26, 204, 87, 83, 62, 19, 206, 80, 223, 227, 146, 179, 5, 143, 111, 145, 207, 220, 89, 99, 72, 112, 179, 147, 46, 84, 222, 188, 71, 44, 80, 153, 159, 143, 63, 120, 52, 101, 234 } });
        }
    }
}
