using Microsoft.EntityFrameworkCore.Migrations;

namespace CafeDunyasi.Data.Migrations
{
    public partial class post : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BusinessID",
                table: "Posts");

            migrationBuilder.AddColumn<string>(
                name: "UserID",
                table: "Posts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserID",
                table: "Posts");

            migrationBuilder.AddColumn<int>(
                name: "BusinessID",
                table: "Posts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
