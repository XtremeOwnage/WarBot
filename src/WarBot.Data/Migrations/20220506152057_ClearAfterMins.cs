using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarBot.Data.Migrations
{
    public partial class ClearAfterMins : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AlterColumn<long>(
            //    name: "War_4ID",
            //    table: "HustleCastleSettings",
            //    type: "bigint",
            //    nullable: false,
            //    oldClrType: typeof(ulong),
            //    oldType: "bigint unsigned");

            //migrationBuilder.AlterColumn<long>(
            //    name: "War_3ID",
            //    table: "HustleCastleSettings",
            //    type: "bigint",
            //    nullable: false,
            //    oldClrType: typeof(ulong),
            //    oldType: "bigint unsigned");

            //migrationBuilder.AlterColumn<long>(
            //    name: "War_2ID",
            //    table: "HustleCastleSettings",
            //    type: "bigint",
            //    nullable: false,
            //    oldClrType: typeof(ulong),
            //    oldType: "bigint unsigned");

            //migrationBuilder.AlterColumn<long>(
            //    name: "War_1ID",
            //    table: "HustleCastleSettings",
            //    type: "bigint",
            //    nullable: false,
            //    oldClrType: typeof(ulong),
            //    oldType: "bigint unsigned");

            //migrationBuilder.AlterColumn<long>(
            //    name: "PortalID",
            //    table: "HustleCastleSettings",
            //    type: "bigint",
            //    nullable: false,
            //    oldClrType: typeof(ulong),
            //    oldType: "bigint unsigned");

            //migrationBuilder.AlterColumn<long>(
            //    name: "Expedition_4ID",
            //    table: "HustleCastleSettings",
            //    type: "bigint",
            //    nullable: true,
            //    oldClrType: typeof(ulong),
            //    oldType: "bigint unsigned",
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<long>(
            //    name: "Expedition_3ID",
            //    table: "HustleCastleSettings",
            //    type: "bigint",
            //    nullable: true,
            //    oldClrType: typeof(ulong),
            //    oldType: "bigint unsigned",
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<long>(
            //    name: "Expedition_2ID",
            //    table: "HustleCastleSettings",
            //    type: "bigint",
            //    nullable: true,
            //    oldClrType: typeof(ulong),
            //    oldType: "bigint unsigned",
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<long>(
            //    name: "Expedition_1ID",
            //    table: "HustleCastleSettings",
            //    type: "bigint",
            //    nullable: true,
            //    oldClrType: typeof(ulong),
            //    oldType: "bigint unsigned",
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<long>(
            //    name: "Event_UserLeftID",
            //    table: "GuildSettings",
            //    type: "bigint",
            //    nullable: false,
            //    oldClrType: typeof(ulong),
            //    oldType: "bigint unsigned");

            //migrationBuilder.AlterColumn<long>(
            //    name: "Event_UserJoinID",
            //    table: "GuildSettings",
            //    type: "bigint",
            //    nullable: false,
            //    oldClrType: typeof(ulong),
            //    oldType: "bigint unsigned");

            //migrationBuilder.AlterColumn<long>(
            //    name: "Event_UpdatesID",
            //    table: "GuildSettings",
            //    type: "bigint",
            //    nullable: false,
            //    oldClrType: typeof(ulong),
            //    oldType: "bigint unsigned");

            //migrationBuilder.AlterColumn<long>(
            //    name: "ID",
            //    table: "GuildChannelEvent",
            //    type: "bigint",
            //    nullable: false,
            //    oldClrType: typeof(ulong),
            //    oldType: "bigint unsigned")
            //    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
            //    .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<int>(
                name: "ClearAfterMins",
                table: "GuildChannelEvent",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<uint>(
                name: "ClearMethod",
                table: "GuildChannelEvent",
                type: "int unsigned",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClearAfterMins",
                table: "GuildChannelEvent");

            migrationBuilder.DropColumn(
                name: "ClearMethod",
                table: "GuildChannelEvent");

            //migrationBuilder.AlterColumn<ulong>(
            //    name: "War_4ID",
            //    table: "HustleCastleSettings",
            //    type: "bigint unsigned",
            //    nullable: false,
            //    oldClrType: typeof(long),
            //    oldType: "bigint");

            //migrationBuilder.AlterColumn<ulong>(
            //    name: "War_3ID",
            //    table: "HustleCastleSettings",
            //    type: "bigint unsigned",
            //    nullable: false,
            //    oldClrType: typeof(long),
            //    oldType: "bigint");

            //migrationBuilder.AlterColumn<ulong>(
            //    name: "War_2ID",
            //    table: "HustleCastleSettings",
            //    type: "bigint unsigned",
            //    nullable: false,
            //    oldClrType: typeof(long),
            //    oldType: "bigint");

            //migrationBuilder.AlterColumn<ulong>(
            //    name: "War_1ID",
            //    table: "HustleCastleSettings",
            //    type: "bigint unsigned",
            //    nullable: false,
            //    oldClrType: typeof(long),
            //    oldType: "bigint");

            //migrationBuilder.AlterColumn<ulong>(
            //    name: "PortalID",
            //    table: "HustleCastleSettings",
            //    type: "bigint unsigned",
            //    nullable: false,
            //    oldClrType: typeof(long),
            //    oldType: "bigint");

            //migrationBuilder.AlterColumn<ulong>(
            //    name: "Expedition_4ID",
            //    table: "HustleCastleSettings",
            //    type: "bigint unsigned",
            //    nullable: true,
            //    oldClrType: typeof(long),
            //    oldType: "bigint",
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<ulong>(
            //    name: "Expedition_3ID",
            //    table: "HustleCastleSettings",
            //    type: "bigint unsigned",
            //    nullable: true,
            //    oldClrType: typeof(long),
            //    oldType: "bigint",
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<ulong>(
            //    name: "Expedition_2ID",
            //    table: "HustleCastleSettings",
            //    type: "bigint unsigned",
            //    nullable: true,
            //    oldClrType: typeof(long),
            //    oldType: "bigint",
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<ulong>(
            //    name: "Expedition_1ID",
            //    table: "HustleCastleSettings",
            //    type: "bigint unsigned",
            //    nullable: true,
            //    oldClrType: typeof(long),
            //    oldType: "bigint",
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<ulong>(
            //    name: "Event_UserLeftID",
            //    table: "GuildSettings",
            //    type: "bigint unsigned",
            //    nullable: false,
            //    oldClrType: typeof(long),
            //    oldType: "bigint");

            //migrationBuilder.AlterColumn<ulong>(
            //    name: "Event_UserJoinID",
            //    table: "GuildSettings",
            //    type: "bigint unsigned",
            //    nullable: false,
            //    oldClrType: typeof(long),
            //    oldType: "bigint");

            //migrationBuilder.AlterColumn<ulong>(
            //    name: "Event_UpdatesID",
            //    table: "GuildSettings",
            //    type: "bigint unsigned",
            //    nullable: false,
            //    oldClrType: typeof(long),
            //    oldType: "bigint");

            //migrationBuilder.AlterColumn<ulong>(
            //    name: "ID",
            //    table: "GuildChannelEvent",
            //    type: "bigint unsigned",
            //    nullable: false,
            //    oldClrType: typeof(long),
            //    oldType: "bigint")
            //    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
            //    .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);
        }
    }
}
