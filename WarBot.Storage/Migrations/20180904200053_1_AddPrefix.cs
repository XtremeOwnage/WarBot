using Microsoft.EntityFrameworkCore.Migrations;

namespace WarBot.Storage.Migrations
{
    public partial class _1_AddPrefix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WarBOT_NickName",
                table: "Guilds",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WarBOT_Prefix",
                table: "Guilds",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WarBOT_NickName",
                table: "Guilds");

            migrationBuilder.DropColumn(
                name: "WarBOT_Prefix",
                table: "Guilds");
        }
    }
}
