using Microsoft.EntityFrameworkCore.Migrations;

namespace WarBot.Storage.Migrations
{
    public partial class Settings_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GuildSetting",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Key = table.Column<int>(nullable: false),
                    Enabled = table.Column<bool>(nullable: false),
                    Value = table.Column<string>(nullable: true),
                    GuildID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildSetting", x => x.ID);
                    table.ForeignKey(
                        name: "FK_GuildSetting_Guilds_GuildID",
                        column: x => x.GuildID,
                        principalTable: "Guilds",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GuildSetting_GuildID",
                table: "GuildSetting",
                column: "GuildID");
            
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GuildSetting");
        }
    }
}
