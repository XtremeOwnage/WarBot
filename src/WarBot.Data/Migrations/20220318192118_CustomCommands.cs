using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarBot.Data.Migrations
{
    public partial class CustomCommands : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomSlashCommand",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ParentID = table.Column<long>(type: "bigint", nullable: false),
                    PublishSlashCommand = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MinimumRoleLevel = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomSlashCommand", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CustomSlashCommand_GuildSettings_ParentID",
                        column: x => x.ParentID,
                        principalTable: "GuildSettings",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CustomCommandAction",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ParentID = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TargetChannelID = table.Column<long>(type: "bigint", nullable: false),
                    TargetRoleID = table.Column<long>(type: "bigint", nullable: false),
                    ItemId = table.Column<ulong>(type: "bigint unsigned", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomCommandAction", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CustomCommandAction_CustomSlashCommand_ParentID",
                        column: x => x.ParentID,
                        principalTable: "CustomSlashCommand",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomCommandAction_GuildChannel_TargetChannelID",
                        column: x => x.TargetChannelID,
                        principalTable: "GuildChannel",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomCommandAction_GuildRole_TargetRoleID",
                        column: x => x.TargetRoleID,
                        principalTable: "GuildRole",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_CustomCommandAction_ParentID",
                table: "CustomCommandAction",
                column: "ParentID");

            migrationBuilder.CreateIndex(
                name: "IX_CustomCommandAction_TargetChannelID",
                table: "CustomCommandAction",
                column: "TargetChannelID");

            migrationBuilder.CreateIndex(
                name: "IX_CustomCommandAction_TargetRoleID",
                table: "CustomCommandAction",
                column: "TargetRoleID");

            migrationBuilder.CreateIndex(
                name: "IX_CustomSlashCommand_ParentID",
                table: "CustomSlashCommand",
                column: "ParentID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomCommandAction");

            migrationBuilder.DropTable(
                name: "CustomSlashCommand");
        }
    }
}
