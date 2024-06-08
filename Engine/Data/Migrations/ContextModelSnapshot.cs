﻿// <auto-generated />
using System;
using Engine.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Engine.Data.Migrations
{
    [DbContext(typeof(Context))]
    partial class ContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Engine.Entities.Card", b =>
                {
                    b.Property<string>("Name")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("Card1")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Card10")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Card11")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Card12")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Card13")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Card2")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Card3")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Card4")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Card5")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Card6")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Card7")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Card8")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Card9")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Name");

                    b.ToTable("Card");
                });

            modelBuilder.Entity("Engine.Entities.ConnectionTracker", b =>
                {
                    b.Property<string>("Name")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("CurrentId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OldId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PlayerName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Name");

                    b.ToTable("ConnectionTracker");
                });

            modelBuilder.Entity("Engine.Entities.Game", b =>
                {
                    b.Property<string>("Name")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("Blue1")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Blue1Card")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Blue1CardsName")
                        .HasColumnType("nvarchar(20)");

                    b.Property<int>("Blue1Claimed")
                        .HasColumnType("int");

                    b.Property<string>("Blue1Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Blue2")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Blue2Card")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Blue2CardsName")
                        .HasColumnType("nvarchar(20)");

                    b.Property<int>("Blue2Claimed")
                        .HasColumnType("int");

                    b.Property<string>("Blue2Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("BlueRoundScore")
                        .HasColumnType("int");

                    b.Property<int>("BlueTotalScore")
                        .HasColumnType("int");

                    b.Property<int>("ClaimStartsByIndex")
                        .HasColumnType("int");

                    b.Property<int>("GS")
                        .HasColumnType("int");

                    b.Property<string>("GameType")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("HakemCardsName")
                        .HasColumnType("nvarchar(20)");

                    b.Property<int>("HakemIndex")
                        .HasColumnType("int");

                    b.Property<string>("HokmSuit")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Red1")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Red1Card")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Red1CardsName")
                        .HasColumnType("nvarchar(20)");

                    b.Property<int>("Red1Claimed")
                        .HasColumnType("int");

                    b.Property<string>("Red1Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Red2")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Red2Card")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Red2CardsName")
                        .HasColumnType("nvarchar(20)");

                    b.Property<int>("Red2Claimed")
                        .HasColumnType("int");

                    b.Property<string>("Red2Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RedRoundScore")
                        .HasColumnType("int");

                    b.Property<int>("RedTotalScore")
                        .HasColumnType("int");

                    b.Property<int>("RoundStartsByIndex")
                        .HasColumnType("int");

                    b.Property<string>("RoundSuit")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RoundTargetScore")
                        .HasColumnType("int");

                    b.Property<int>("TargetScore")
                        .HasColumnType("int");

                    b.Property<int>("WhosTurnIndex")
                        .HasColumnType("int");

                    b.HasKey("Name");

                    b.HasIndex("Blue1CardsName");

                    b.HasIndex("Blue2CardsName");

                    b.HasIndex("HakemCardsName");

                    b.HasIndex("Red1CardsName");

                    b.HasIndex("Red2CardsName");

                    b.ToTable("Game");
                });

            modelBuilder.Entity("Engine.Entities.Lobby", b =>
                {
                    b.Property<string>("Name")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.HasKey("Name");

                    b.ToTable("Lobby");
                });

            modelBuilder.Entity("Engine.Entities.Player", b =>
                {
                    b.Property<string>("Name")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("Badge")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ConnectionId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Country")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("GameName")
                        .HasColumnType("nvarchar(20)");

                    b.Property<int>("GamesLeft")
                        .HasColumnType("int");

                    b.Property<int>("GamesLost")
                        .HasColumnType("int");

                    b.Property<int>("GamesWon")
                        .HasColumnType("int");

                    b.Property<int>("HokmScore")
                        .HasColumnType("int");

                    b.Property<string>("LobbyName")
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("PhotoUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Rate")
                        .HasColumnType("int");

                    b.Property<string>("RoomName")
                        .HasColumnType("nvarchar(20)");

                    b.Property<int>("ShelemScore")
                        .HasColumnType("int");

                    b.HasKey("Name");

                    b.HasIndex("GameName");

                    b.HasIndex("LobbyName");

                    b.HasIndex("RoomName");

                    b.ToTable("Player");
                });

            modelBuilder.Entity("Engine.Entities.Room", b =>
                {
                    b.Property<string>("Name")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("Blue1")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Blue1Ready")
                        .HasColumnType("bit");

                    b.Property<string>("Blue2")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Blue2Ready")
                        .HasColumnType("bit");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("GameType")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("HostName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Red1")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Red1Ready")
                        .HasColumnType("bit");

                    b.Property<string>("Red2")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Red2Ready")
                        .HasColumnType("bit");

                    b.Property<int>("TargetScore")
                        .HasColumnType("int");

                    b.HasKey("Name");

                    b.ToTable("Room");
                });

            modelBuilder.Entity("Engine.Entities.ConnectionTracker", b =>
                {
                    b.HasOne("Engine.Entities.Player", "Player")
                        .WithOne("ConnectionTracker")
                        .HasForeignKey("Engine.Entities.ConnectionTracker", "Name")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Player");
                });

            modelBuilder.Entity("Engine.Entities.Game", b =>
                {
                    b.HasOne("Engine.Entities.Card", "Blue1Cards")
                        .WithMany()
                        .HasForeignKey("Blue1CardsName");

                    b.HasOne("Engine.Entities.Card", "Blue2Cards")
                        .WithMany()
                        .HasForeignKey("Blue2CardsName");

                    b.HasOne("Engine.Entities.Card", "HakemCards")
                        .WithMany()
                        .HasForeignKey("HakemCardsName");

                    b.HasOne("Engine.Entities.Card", "Red1Cards")
                        .WithMany()
                        .HasForeignKey("Red1CardsName");

                    b.HasOne("Engine.Entities.Card", "Red2Cards")
                        .WithMany()
                        .HasForeignKey("Red2CardsName");

                    b.Navigation("Blue1Cards");

                    b.Navigation("Blue2Cards");

                    b.Navigation("HakemCards");

                    b.Navigation("Red1Cards");

                    b.Navigation("Red2Cards");
                });

            modelBuilder.Entity("Engine.Entities.Player", b =>
                {
                    b.HasOne("Engine.Entities.Game", "Game")
                        .WithMany("Players")
                        .HasForeignKey("GameName")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Engine.Entities.Lobby", "Lobby")
                        .WithMany("Players")
                        .HasForeignKey("LobbyName")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Engine.Entities.Room", "Room")
                        .WithMany("Players")
                        .HasForeignKey("RoomName")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Game");

                    b.Navigation("Lobby");

                    b.Navigation("Room");
                });

            modelBuilder.Entity("Engine.Entities.Game", b =>
                {
                    b.Navigation("Players");
                });

            modelBuilder.Entity("Engine.Entities.Lobby", b =>
                {
                    b.Navigation("Players");
                });

            modelBuilder.Entity("Engine.Entities.Player", b =>
                {
                    b.Navigation("ConnectionTracker");
                });

            modelBuilder.Entity("Engine.Entities.Room", b =>
                {
                    b.Navigation("Players");
                });
#pragma warning restore 612, 618
        }
    }
}
