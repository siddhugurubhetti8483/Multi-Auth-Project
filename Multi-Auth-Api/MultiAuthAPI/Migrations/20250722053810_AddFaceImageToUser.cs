using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultiAuthAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddFaceImageToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FaceImageBase64",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FaceImageBase64",
                table: "Users");
        }
    }
}
