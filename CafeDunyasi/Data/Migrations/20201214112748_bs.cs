using Microsoft.EntityFrameworkCore.Migrations;

namespace CafeDunyasi.Data.Migrations
{
    public partial class bs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvatarImgURL",
                table: "BusinessInfo");

            migrationBuilder.DropColumn(
                name: "EMail",
                table: "BusinessInfo");

            migrationBuilder.DropColumn(
                name: "MenuImgURL",
                table: "BusinessInfo");

            migrationBuilder.AddColumn<string>(
                name: "AvatarImg",
                table: "BusinessInfo",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MenuImg",
                table: "BusinessInfo",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvatarImg",
                table: "BusinessInfo");

            migrationBuilder.DropColumn(
                name: "MenuImg",
                table: "BusinessInfo");

            migrationBuilder.AddColumn<string>(
                name: "AvatarImgURL",
                table: "BusinessInfo",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EMail",
                table: "BusinessInfo",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MenuImgURL",
                table: "BusinessInfo",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
