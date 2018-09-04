using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WarBot.Storage.Migrations
{
    public partial class _2_MoreFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastActivity",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastOnline",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "JoinDate",
                table: "HustleUser",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "GreetingMessage",
                table: "GuildNotificationsSettings",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastActivity",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastOnline",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "JoinDate",
                table: "HustleUser");

            migrationBuilder.DropColumn(
                name: "GreetingMessage",
                table: "GuildNotificationsSettings");
        }
    }
}
