using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieBookingBackend.Migrations
{
    public partial class UpdatedDbForEmail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "EmailVerifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    VerificationCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailVerifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailVerifications_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 101,
                columns: new[] { "PasswordHash", "PasswordHashKey", "Status" },
                values: new object[] { new byte[] { 178, 110, 9, 32, 247, 109, 98, 90, 100, 42, 134, 0, 250, 160, 242, 113, 65, 177, 38, 103, 5, 242, 19, 255, 128, 226, 86, 183, 65, 176, 59, 58, 182, 192, 29, 58, 236, 208, 14, 93, 9, 147, 206, 48, 45, 167, 249, 24, 60, 11, 82, 253, 122, 40, 221, 27, 159, 207, 84, 168, 74, 98, 13, 147 }, new byte[] { 34, 164, 80, 109, 63, 187, 166, 177, 159, 3, 4, 57, 102, 242, 35, 181, 27, 191, 13, 228, 50, 70, 195, 80, 38, 188, 124, 186, 68, 107, 5, 109, 209, 176, 222, 30, 122, 105, 64, 249, 6, 149, 200, 250, 175, 98, 73, 158, 224, 47, 111, 117, 29, 176, 189, 6, 163, 40, 26, 75, 70, 85, 201, 130, 131, 250, 45, 160, 110, 99, 182, 15, 75, 113, 55, 192, 51, 182, 215, 19, 117, 194, 192, 203, 131, 39, 187, 123, 34, 26, 204, 87, 83, 62, 19, 206, 80, 223, 227, 146, 179, 5, 143, 111, 145, 207, 220, 89, 99, 72, 112, 179, 147, 46, 84, 222, 188, 71, 44, 80, 153, 159, 143, 63, 120, 52, 101, 234 }, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_EmailVerifications_UserId",
                table: "EmailVerifications",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailVerifications");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 101,
                columns: new[] { "PasswordHash", "PasswordHashKey" },
                values: new object[] { new byte[] { 248, 22, 17, 28, 23, 168, 250, 197, 206, 1, 132, 39, 156, 225, 103, 48, 217, 17, 209, 24, 3, 30, 175, 8, 238, 217, 43, 11, 188, 120, 244, 134, 106, 77, 152, 177, 160, 65, 91, 222, 56, 126, 33, 185, 93, 43, 61, 69, 20, 193, 119, 89, 110, 148, 229, 159, 248, 174, 250, 164, 194, 102, 173, 44 }, new byte[] { 217, 142, 50, 126, 253, 200, 168, 116, 121, 163, 244, 231, 205, 78, 122, 27, 133, 244, 105, 55, 59, 216, 226, 64, 140, 141, 239, 32, 30, 117, 114, 213, 192, 224, 190, 0, 126, 140, 115, 245, 211, 188, 173, 121, 125, 179, 157, 26, 93, 104, 188, 235, 64, 190, 180, 148, 3, 143, 27, 160, 195, 19, 40, 195, 115, 26, 33, 220, 25, 221, 205, 251, 135, 177, 224, 25, 213, 90, 163, 130, 42, 190, 135, 157, 65, 30, 226, 138, 122, 86, 125, 39, 188, 145, 133, 139, 133, 0, 229, 66, 83, 10, 72, 136, 232, 196, 13, 73, 177, 39, 80, 36, 31, 109, 56, 195, 204, 252, 58, 23, 186, 177, 110, 91, 171, 72, 52, 197 } });
        }
    }
}
