﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WarBot.Data;

#nullable disable

namespace WarBot.Data.Migrations
{
    [DbContext(typeof(WarDB))]
    [Migration("20211231015115_2")]
    partial class _2
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("WarBOT.Data.Models.GuildChannel", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<ulong?>("DiscordID")
                        .HasColumnType("bigint unsigned");

                    b.Property<string>("DiscordName")
                        .HasColumnType("longtext");

                    b.HasKey("ID");

                    b.ToTable("GuildChannel");
                });

            modelBuilder.Entity("WarBOT.Data.Models.GuildChannelEvent", b =>
                {
                    b.Property<ulong>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint unsigned");

                    b.Property<long>("ChannelID")
                        .HasColumnType("bigint");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<bool>("Enabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Message")
                        .HasColumnType("longtext");

                    b.HasKey("ID");

                    b.HasIndex("ChannelID");

                    b.ToTable("GuildChannelEvent");

                    b.HasDiscriminator<string>("Discriminator").HasValue("GuildChannelEvent");
                });

            modelBuilder.Entity("WarBOT.Data.Models.GuildRole", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<string>("CustomName")
                        .HasColumnType("longtext");

                    b.Property<ulong?>("DiscordID")
                        .HasColumnType("bigint unsigned");

                    b.Property<string>("DiscordName")
                        .HasColumnType("longtext");

                    b.HasKey("ID");

                    b.ToTable("GuildRole");
                });

            modelBuilder.Entity("WarBOT.Data.Models.GuildRoles", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<long>("GuestID")
                        .HasColumnType("bigint");

                    b.Property<long>("LeaderID")
                        .HasColumnType("bigint");

                    b.Property<long>("MemberID")
                        .HasColumnType("bigint");

                    b.Property<long>("OfficerID")
                        .HasColumnType("bigint");

                    b.Property<long>("ServerAdminID")
                        .HasColumnType("bigint");

                    b.Property<long>("SuperMemberID")
                        .HasColumnType("bigint");

                    b.HasKey("ID");

                    b.HasIndex("GuestID");

                    b.HasIndex("LeaderID");

                    b.HasIndex("MemberID");

                    b.HasIndex("OfficerID");

                    b.HasIndex("ServerAdminID");

                    b.HasIndex("SuperMemberID");

                    b.ToTable("GuildRoles");
                });

            modelBuilder.Entity("WarBOT.Data.Models.GuildSettings", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<string>("BotPrefix")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<double>("BotVersion")
                        .HasColumnType("double");

                    b.Property<long>("Channel_AdminsID")
                        .HasColumnType("bigint");

                    b.Property<ulong>("DiscordID")
                        .HasColumnType("bigint unsigned");

                    b.Property<string>("DiscordName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<ulong>("Event_UpdatesID")
                        .HasColumnType("bigint unsigned");

                    b.Property<ulong>("Event_UserJoinID")
                        .HasColumnType("bigint unsigned");

                    b.Property<ulong>("Event_UserLeftID")
                        .HasColumnType("bigint unsigned");

                    b.Property<long>("HustleCastleSettingsID")
                        .HasColumnType("bigint");

                    b.Property<long>("RolesID")
                        .HasColumnType("bigint");

                    b.Property<string>("Website")
                        .HasColumnType("longtext");

                    b.HasKey("ID");

                    b.HasIndex("Channel_AdminsID");

                    b.HasIndex("Event_UpdatesID");

                    b.HasIndex("Event_UserJoinID");

                    b.HasIndex("Event_UserLeftID");

                    b.HasIndex("HustleCastleSettingsID");

                    b.HasIndex("RolesID");

                    b.ToTable("GuildSettings");
                });

            modelBuilder.Entity("WarBOT.Data.Models.HustleCastleSettings", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<string>("LootMessage")
                        .HasColumnType("longtext");

                    b.Property<ulong>("PortalID")
                        .HasColumnType("bigint unsigned");

                    b.Property<ulong>("War_1ID")
                        .HasColumnType("bigint unsigned");

                    b.Property<ulong>("War_2ID")
                        .HasColumnType("bigint unsigned");

                    b.Property<ulong>("War_3ID")
                        .HasColumnType("bigint unsigned");

                    b.Property<ulong>("War_4ID")
                        .HasColumnType("bigint unsigned");

                    b.HasKey("ID");

                    b.HasIndex("PortalID");

                    b.HasIndex("War_1ID");

                    b.HasIndex("War_2ID");

                    b.HasIndex("War_3ID");

                    b.HasIndex("War_4ID");

                    b.ToTable("HustleCastleSettings");
                });

            modelBuilder.Entity("WarBOT.Data.Models.HustleGuildChannelEvent", b =>
                {
                    b.HasBaseType("WarBOT.Data.Models.GuildChannelEvent");

                    b.Property<string>("Event_Finished_Message")
                        .HasColumnType("longtext");

                    b.Property<string>("Event_Started_Message")
                        .HasColumnType("longtext");

                    b.Property<string>("Prep_Ending_Message")
                        .HasColumnType("longtext");

                    b.Property<string>("Prep_Started_Message")
                        .HasColumnType("longtext");

                    b.HasDiscriminator().HasValue("HustleGuildChannelEvent");
                });

            modelBuilder.Entity("WarBOT.Data.Models.GuildChannelEvent", b =>
                {
                    b.HasOne("WarBOT.Data.Models.GuildChannel", "Channel")
                        .WithMany()
                        .HasForeignKey("ChannelID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Channel");
                });

            modelBuilder.Entity("WarBOT.Data.Models.GuildRoles", b =>
                {
                    b.HasOne("WarBOT.Data.Models.GuildRole", "Guest")
                        .WithMany()
                        .HasForeignKey("GuestID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WarBOT.Data.Models.GuildRole", "Leader")
                        .WithMany()
                        .HasForeignKey("LeaderID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WarBOT.Data.Models.GuildRole", "Member")
                        .WithMany()
                        .HasForeignKey("MemberID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WarBOT.Data.Models.GuildRole", "Officer")
                        .WithMany()
                        .HasForeignKey("OfficerID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WarBOT.Data.Models.GuildRole", "ServerAdmin")
                        .WithMany()
                        .HasForeignKey("ServerAdminID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WarBOT.Data.Models.GuildRole", "SuperMember")
                        .WithMany()
                        .HasForeignKey("SuperMemberID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Guest");

                    b.Navigation("Leader");

                    b.Navigation("Member");

                    b.Navigation("Officer");

                    b.Navigation("ServerAdmin");

                    b.Navigation("SuperMember");
                });

            modelBuilder.Entity("WarBOT.Data.Models.GuildSettings", b =>
                {
                    b.HasOne("WarBOT.Data.Models.GuildChannel", "Channel_Admins")
                        .WithMany()
                        .HasForeignKey("Channel_AdminsID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WarBOT.Data.Models.GuildChannelEvent", "Event_Updates")
                        .WithMany()
                        .HasForeignKey("Event_UpdatesID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WarBOT.Data.Models.GuildChannelEvent", "Event_UserJoin")
                        .WithMany()
                        .HasForeignKey("Event_UserJoinID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WarBOT.Data.Models.GuildChannelEvent", "Event_UserLeft")
                        .WithMany()
                        .HasForeignKey("Event_UserLeftID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WarBOT.Data.Models.HustleCastleSettings", "HustleCastleSettings")
                        .WithMany()
                        .HasForeignKey("HustleCastleSettingsID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WarBOT.Data.Models.GuildRoles", "Roles")
                        .WithMany()
                        .HasForeignKey("RolesID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Channel_Admins");

                    b.Navigation("Event_Updates");

                    b.Navigation("Event_UserJoin");

                    b.Navigation("Event_UserLeft");

                    b.Navigation("HustleCastleSettings");

                    b.Navigation("Roles");
                });

            modelBuilder.Entity("WarBOT.Data.Models.HustleCastleSettings", b =>
                {
                    b.HasOne("WarBOT.Data.Models.GuildChannelEvent", "Portal")
                        .WithMany()
                        .HasForeignKey("PortalID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WarBOT.Data.Models.HustleGuildChannelEvent", "War_1")
                        .WithMany()
                        .HasForeignKey("War_1ID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WarBOT.Data.Models.HustleGuildChannelEvent", "War_2")
                        .WithMany()
                        .HasForeignKey("War_2ID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WarBOT.Data.Models.HustleGuildChannelEvent", "War_3")
                        .WithMany()
                        .HasForeignKey("War_3ID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WarBOT.Data.Models.HustleGuildChannelEvent", "War_4")
                        .WithMany()
                        .HasForeignKey("War_4ID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Portal");

                    b.Navigation("War_1");

                    b.Navigation("War_2");

                    b.Navigation("War_3");

                    b.Navigation("War_4");
                });
#pragma warning restore 612, 618
        }
    }
}