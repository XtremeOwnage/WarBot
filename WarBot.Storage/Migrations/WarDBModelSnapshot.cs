﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WarBot.Storage;

namespace WarBot.Storage.Migrations
{
    [DbContext(typeof(WarDB))]
    partial class WarDBModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.0-preview3-35497");

            modelBuilder.Entity("WarBot.Core.Voting.Poll", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<ulong>("ChannelId");

                    b.Property<DateTimeOffset>("EndTime");

                    b.Property<ulong>("MessageId");

                    b.HasKey("ID");

                    b.ToTable("Polls");
                });

            modelBuilder.Entity("WarBot.Core.Voting.PollOption", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int>("PollID");

                    b.Property<string>("Value")
                        .IsRequired();

                    b.HasKey("ID");

                    b.HasIndex("PollID");

                    b.ToTable("PollOption");
                });

            modelBuilder.Entity("WarBot.Core.Voting.UserVote", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("OptionID");

                    b.Property<int>("PollID");

                    b.HasKey("ID");

                    b.HasIndex("PollID");

                    b.ToTable("UserVote");
                });

            modelBuilder.Entity("WarBot.Storage.Models.DiscordGuild", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("BotVersion");

                    b.Property<ulong>("EntityId");

                    b.Property<string>("Loot");

                    b.Property<string>("Name");

                    b.Property<int?>("NotificationSettingsID");

                    b.Property<string>("WarBOT_Prefix");

                    b.Property<string>("Website");

                    b.HasKey("ID");

                    b.HasIndex("NotificationSettingsID");

                    b.ToTable("Guilds");
                });

            modelBuilder.Entity("WarBot.Storage.Models.DiscordUser", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<ulong>("EntityId");

                    b.Property<DateTimeOffset?>("LastActivity");

                    b.Property<DateTimeOffset?>("LastOnline");

                    b.Property<string>("Name");

                    b.HasKey("ID");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("WarBot.Storage.Models.GuildChannel", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ChannelType");

                    b.Property<ulong>("EntityId");

                    b.Property<int?>("GuildID");

                    b.Property<string>("Name");

                    b.HasKey("ID");

                    b.HasIndex("GuildID");

                    b.ToTable("GuildChannel");
                });

            modelBuilder.Entity("WarBot.Storage.Models.GuildNotificationsSettings", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("GreetingMessage");

                    b.Property<bool>("SendUpdateMessage");

                    b.Property<bool>("War1Enabled");

                    b.Property<bool>("War2Enabled");

                    b.Property<bool>("War3Enabled");

                    b.Property<bool>("War4Enabled");

                    b.Property<bool>("WarPrepEnding");

                    b.Property<string>("WarPrepEndingMessage");

                    b.Property<bool>("WarPrepStarted");

                    b.Property<string>("WarPrepStartedMessage");

                    b.Property<bool>("WarStarted");

                    b.Property<string>("WarStartedMessage");

                    b.HasKey("ID");

                    b.ToTable("GuildNotificationsSettings");
                });

            modelBuilder.Entity("WarBot.Storage.Models.GuildRole", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<ulong>("EntityId");

                    b.Property<int?>("GuildID");

                    b.Property<int>("Level");

                    b.Property<string>("Name");

                    b.HasKey("ID");

                    b.HasIndex("GuildID");

                    b.ToTable("GuildRole");
                });

            modelBuilder.Entity("WarBot.Storage.Models.HustleCastle.HustleGuild", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("DiscordGuildId");

                    b.Property<string>("Name");

                    b.HasKey("ID");

                    b.HasIndex("DiscordGuildId")
                        .IsUnique();

                    b.ToTable("HustleGuild");
                });

            modelBuilder.Entity("WarBot.Storage.Models.HustleCastle.HustleGuildSeason", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Active");

                    b.Property<DateTime>("DateEnd");

                    b.Property<int?>("HustleGuildID");

                    b.Property<int?>("HustleUserID");

                    b.Property<uint>("MinimumGameGlory");

                    b.Property<uint>("MinimumGuildGlory");

                    b.HasKey("ID");

                    b.HasIndex("HustleGuildID");

                    b.HasIndex("HustleUserID");

                    b.ToTable("HustleGuildSeason");
                });

            modelBuilder.Entity("WarBot.Storage.Models.HustleCastle.HustleUser", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CharacterName")
                        .IsRequired()
                        .HasMaxLength(20);

                    b.Property<int?>("HustleGuildID");

                    b.Property<bool>("IsActiveCharacter");

                    b.Property<DateTimeOffset>("JoinDate");

                    b.Property<uint>("SquadPower");

                    b.Property<byte>("ThroneRoomLevel");

                    b.Property<int>("UserID");

                    b.HasKey("ID");

                    b.HasIndex("HustleGuildID");

                    b.HasIndex("UserID");

                    b.ToTable("HustleUser");
                });

            modelBuilder.Entity("WarBot.Storage.Models.HustleCastle.HustleUserSeason", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<uint>("Glory");

                    b.Property<int?>("SeasonID");

                    b.Property<int?>("UserID");

                    b.HasKey("ID");

                    b.HasIndex("SeasonID");

                    b.HasIndex("UserID");

                    b.ToTable("HustleUserSeason");
                });

            modelBuilder.Entity("WarBot.Storage.Models.HustleCastle.LootItem", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<int?>("HustleGuildSeasonID");

                    b.Property<int?>("HustleUserSeasonID");

                    b.Property<int?>("HustleUserSeasonID1");

                    b.Property<string>("Name");

                    b.Property<int?>("WinningUserID");

                    b.HasKey("ID");

                    b.HasIndex("HustleGuildSeasonID");

                    b.HasIndex("HustleUserSeasonID");

                    b.HasIndex("HustleUserSeasonID1");

                    b.HasIndex("WinningUserID");

                    b.ToTable("LootItem");
                });

            modelBuilder.Entity("WarBot.Core.Voting.PollOption", b =>
                {
                    b.HasOne("WarBot.Core.Voting.Poll")
                        .WithMany("Options")
                        .HasForeignKey("PollID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("WarBot.Core.Voting.UserVote", b =>
                {
                    b.HasOne("WarBot.Core.Voting.Poll")
                        .WithMany("Votes")
                        .HasForeignKey("PollID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("WarBot.Storage.Models.DiscordGuild", b =>
                {
                    b.HasOne("WarBot.Storage.Models.GuildNotificationsSettings", "NotificationSettings")
                        .WithMany()
                        .HasForeignKey("NotificationSettingsID");
                });

            modelBuilder.Entity("WarBot.Storage.Models.GuildChannel", b =>
                {
                    b.HasOne("WarBot.Storage.Models.DiscordGuild", "Guild")
                        .WithMany("Channels")
                        .HasForeignKey("GuildID");
                });

            modelBuilder.Entity("WarBot.Storage.Models.GuildRole", b =>
                {
                    b.HasOne("WarBot.Storage.Models.DiscordGuild", "Guild")
                        .WithMany("Roles")
                        .HasForeignKey("GuildID");
                });

            modelBuilder.Entity("WarBot.Storage.Models.HustleCastle.HustleGuild", b =>
                {
                    b.HasOne("WarBot.Storage.Models.DiscordGuild", "DiscordGuild")
                        .WithOne("HustleClan")
                        .HasForeignKey("WarBot.Storage.Models.HustleCastle.HustleGuild", "DiscordGuildId");
                });

            modelBuilder.Entity("WarBot.Storage.Models.HustleCastle.HustleGuildSeason", b =>
                {
                    b.HasOne("WarBot.Storage.Models.HustleCastle.HustleGuild")
                        .WithMany("Seasons")
                        .HasForeignKey("HustleGuildID");

                    b.HasOne("WarBot.Storage.Models.HustleCastle.HustleUser")
                        .WithMany("Seasons")
                        .HasForeignKey("HustleUserID");
                });

            modelBuilder.Entity("WarBot.Storage.Models.HustleCastle.HustleUser", b =>
                {
                    b.HasOne("WarBot.Storage.Models.HustleCastle.HustleGuild")
                        .WithMany("Users")
                        .HasForeignKey("HustleGuildID");

                    b.HasOne("WarBot.Storage.Models.DiscordUser", "User")
                        .WithMany("Characters")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("WarBot.Storage.Models.HustleCastle.HustleUserSeason", b =>
                {
                    b.HasOne("WarBot.Storage.Models.HustleCastle.HustleGuildSeason", "Season")
                        .WithMany("UserData")
                        .HasForeignKey("SeasonID");

                    b.HasOne("WarBot.Storage.Models.HustleCastle.HustleUser", "User")
                        .WithMany()
                        .HasForeignKey("UserID");
                });

            modelBuilder.Entity("WarBot.Storage.Models.HustleCastle.LootItem", b =>
                {
                    b.HasOne("WarBot.Storage.Models.HustleCastle.HustleGuildSeason")
                        .WithMany("AvailableLoot")
                        .HasForeignKey("HustleGuildSeasonID");

                    b.HasOne("WarBot.Storage.Models.HustleCastle.HustleUserSeason")
                        .WithMany("GreedItems")
                        .HasForeignKey("HustleUserSeasonID");

                    b.HasOne("WarBot.Storage.Models.HustleCastle.HustleUserSeason")
                        .WithMany("NeedItems")
                        .HasForeignKey("HustleUserSeasonID1");

                    b.HasOne("WarBot.Storage.Models.HustleCastle.HustleUser", "WinningUser")
                        .WithMany()
                        .HasForeignKey("WinningUserID");
                });
#pragma warning restore 612, 618
        }
    }
}
