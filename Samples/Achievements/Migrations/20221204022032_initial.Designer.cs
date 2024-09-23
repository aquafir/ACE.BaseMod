﻿// <auto-generated />
using System;
using Tower.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Tower.Migrations
{
    [DbContext(typeof(AchievementDbContext))]
    [Migration("20221204022032_initial")]
    partial class initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Achievements.Domain.Adventurer", b =>
                {
                    b.Property<uint>("AdventurerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int unsigned");

                    b.HasKey("AdventurerId");

                    b.ToTable("Adventurers");
                });

            modelBuilder.Entity("Achievements.Domain.Dungeon", b =>
                {
                    b.Property<uint>("DungeonId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int unsigned");

                    b.Property<uint>("AdventurerId")
                        .HasColumnType("int unsigned");

                    b.Property<byte[]>("Snapshot")
                        .IsRequired()
                        .HasColumnType("longblob");

                    b.HasKey("DungeonId");

                    b.HasIndex("AdventurerId");

                    b.ToTable("Dungeons");
                });

            modelBuilder.Entity("Achievements.Domain.Kill", b =>
                {
                    b.Property<ulong>("KillId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint unsigned");

                    b.Property<uint>("AdventurerId")
                        .HasColumnType("int unsigned");

                    b.Property<ulong>("Count")
                        .HasColumnType("bigint unsigned");

                    b.Property<ulong>("Wcid")
                        .HasColumnType("bigint unsigned");

                    b.HasKey("KillId");

                    b.HasIndex("AdventurerId");

                    b.ToTable("Kills");
                });

            modelBuilder.Entity("Achievements.Domain.Land", b =>
                {
                    b.Property<ulong>("LandId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint unsigned");

                    b.Property<uint>("AdventurerId")
                        .HasColumnType("int unsigned");

                    b.Property<bool>("Explored")
                        .HasColumnType("bit(1)");

                    b.HasKey("LandId");

                    b.ToTable("Lands");
                });

            modelBuilder.Entity("AdventurerLand", b =>
                {
                    b.Property<uint>("AdventurersAdventurerId")
                        .HasColumnType("int unsigned");

                    b.Property<ulong>("LandsLandId")
                        .HasColumnType("bigint unsigned");

                    b.HasKey("AdventurersAdventurerId", "LandsLandId");

                    b.HasIndex("LandsLandId");

                    b.ToTable("AdventurerLand");
                });

            modelBuilder.Entity("Achievements.Domain.Dungeon", b =>
                {
                    b.HasOne("Achievements.Domain.Adventurer", "Adventurer")
                        .WithMany("Dungeons")
                        .HasForeignKey("AdventurerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Adventurer");
                });

            modelBuilder.Entity("Achievements.Domain.Kill", b =>
                {
                    b.HasOne("Achievements.Domain.Adventurer", "Adventurer")
                        .WithMany("Kills")
                        .HasForeignKey("AdventurerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Adventurer");
                });

            modelBuilder.Entity("AdventurerLand", b =>
                {
                    b.HasOne("Achievements.Domain.Adventurer", null)
                        .WithMany()
                        .HasForeignKey("AdventurersAdventurerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Achievements.Domain.Land", null)
                        .WithMany()
                        .HasForeignKey("LandsLandId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Achievements.Domain.Adventurer", b =>
                {
                    b.Navigation("Dungeons");

                    b.Navigation("Kills");
                });
#pragma warning restore 612, 618
        }
    }
}
