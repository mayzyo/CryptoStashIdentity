using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoStashIdentity.Migrations
{
    public partial class AddApnCol : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Apn",
                table: "AspNetUsers",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Apn",
                table: "AspNetUsers");
        }
    }
}
