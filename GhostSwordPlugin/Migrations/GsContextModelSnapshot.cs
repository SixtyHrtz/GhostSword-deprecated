﻿// <auto-generated />
using System;
using GhostSwordPlugin;
using GhostSwordPlugin.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GhostSwordPlugin.Migrations
{
    [DbContext(typeof(GsContext))]
    partial class GsContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.0-rtm-30799")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("GhostSwordPlugin.Models.Dialogue", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int>("NpcInfoId");

                    b.Property<string>("Text")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue("");

                    b.HasKey("Id");

                    b.HasIndex("NpcInfoId");

                    b.ToTable("Dialogues");
                });

            modelBuilder.Entity("GhostSwordPlugin.Models.Item", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description");

                    b.Property<string>("Emoji");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Items");
                });

            modelBuilder.Entity("GhostSwordPlugin.Models.ItemDiscovery", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ItemId");

                    b.Property<long>("MaxAmount");

                    b.Property<long>("MinAmount");

                    b.Property<int>("PlaceId");

                    b.Property<float>("Rate");

                    b.Property<string>("Text")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("ItemId");

                    b.HasIndex("PlaceId");

                    b.ToTable("ItemDiscoveries");
                });

            modelBuilder.Entity("GhostSwordPlugin.Models.Journey", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("Duration");

                    b.Property<int>("PlaceAdjacencyId");

                    b.Property<int>("PlayerId");

                    b.Property<DateTime>("StartTime");

                    b.HasKey("Guid");

                    b.HasIndex("PlaceAdjacencyId");

                    b.HasIndex("PlayerId");

                    b.ToTable("Journeys");
                });

            modelBuilder.Entity("GhostSwordPlugin.Models.NpcInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Greetings")
                        .IsRequired();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int>("PlaceId");

                    b.HasKey("Id");

                    b.HasIndex("PlaceId");

                    b.ToTable("NpcInfos");
                });

            modelBuilder.Entity("GhostSwordPlugin.Models.Place", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<long>("Phase");

                    b.Property<int>("PlaceLinkId");

                    b.HasKey("Id");

                    b.HasIndex("PlaceLinkId");

                    b.ToTable("Places");
                });

            modelBuilder.Entity("GhostSwordPlugin.Models.PlaceAdjacency", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("BeginText")
                        .IsRequired();

                    b.Property<string>("EndText")
                        .IsRequired();

                    b.Property<long>("JourneyDuration");

                    b.Property<int>("Place1Id");

                    b.Property<int>("Place2Id");

                    b.HasKey("Id");

                    b.HasIndex("Place1Id");

                    b.HasIndex("Place2Id");

                    b.ToTable("PlaceAdjacencies");
                });

            modelBuilder.Entity("GhostSwordPlugin.Models.PlaceLink", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("PlaceLinks");
                });

            modelBuilder.Entity("GhostSwordPlugin.Models.Player", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<Guid?>("ChestItemGuid");

                    b.Property<Guid?>("FeetsItemGuid");

                    b.Property<Guid?>("HandsItemGuid");

                    b.Property<Guid?>("HeadItemGuid");

                    b.Property<bool>("IsBusy")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(false);

                    b.Property<Guid?>("LegsItemGuid");

                    b.Property<long>("MenuId")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(0L);

                    b.Property<int>("PlaceId")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(1);

                    b.Property<long>("UserId");

                    b.Property<string>("Username")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("ChestItemGuid");

                    b.HasIndex("FeetsItemGuid");

                    b.HasIndex("HandsItemGuid");

                    b.HasIndex("HeadItemGuid");

                    b.HasIndex("LegsItemGuid");

                    b.HasIndex("PlaceId");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("GhostSwordPlugin.Models.PlayerItem", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("newsequentialid()");

                    b.Property<long>("Amount");

                    b.Property<int>("ItemId");

                    b.Property<int>("PlayerId");

                    b.HasKey("Guid");

                    b.HasIndex("ItemId");

                    b.HasIndex("PlayerId");

                    b.ToTable("PlayerItems");
                });

            modelBuilder.Entity("GhostSwordPlugin.Models.PlayerPlace", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("Phase")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(1L);

                    b.Property<int>("PlaceLinkId");

                    b.Property<int>("PlayerId");

                    b.HasKey("Id");

                    b.HasIndex("PlaceLinkId");

                    b.HasIndex("PlayerId");

                    b.ToTable("PlayerPlaces");
                });

            modelBuilder.Entity("GhostSwordPlugin.Models.Dialogue", b =>
                {
                    b.HasOne("GhostSwordPlugin.Models.NpcInfo", "NpcInfo")
                        .WithMany("Dialogues")
                        .HasForeignKey("NpcInfoId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GhostSwordPlugin.Models.ItemDiscovery", b =>
                {
                    b.HasOne("GhostSwordPlugin.Models.Item", "Item")
                        .WithMany()
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GhostSwordPlugin.Models.Place", "Place")
                        .WithMany()
                        .HasForeignKey("PlaceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GhostSwordPlugin.Models.Journey", b =>
                {
                    b.HasOne("GhostSwordPlugin.Models.PlaceAdjacency", "PlaceAdjacency")
                        .WithMany()
                        .HasForeignKey("PlaceAdjacencyId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GhostSwordPlugin.Models.Player", "Player")
                        .WithMany()
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GhostSwordPlugin.Models.NpcInfo", b =>
                {
                    b.HasOne("GhostSwordPlugin.Models.Place", "Place")
                        .WithMany()
                        .HasForeignKey("PlaceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GhostSwordPlugin.Models.Place", b =>
                {
                    b.HasOne("GhostSwordPlugin.Models.PlaceLink", "PlaceLink")
                        .WithMany()
                        .HasForeignKey("PlaceLinkId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("GhostSwordPlugin.Models.PlaceAdjacency", b =>
                {
                    b.HasOne("GhostSwordPlugin.Models.Place", "Place1")
                        .WithMany()
                        .HasForeignKey("Place1Id")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("GhostSwordPlugin.Models.Place", "Place2")
                        .WithMany()
                        .HasForeignKey("Place2Id")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("GhostSwordPlugin.Models.Player", b =>
                {
                    b.HasOne("GhostSwordPlugin.Models.PlayerItem", "ChestItem")
                        .WithMany()
                        .HasForeignKey("ChestItemGuid")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("GhostSwordPlugin.Models.PlayerItem", "FeetsItem")
                        .WithMany()
                        .HasForeignKey("FeetsItemGuid")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("GhostSwordPlugin.Models.PlayerItem", "HandsItem")
                        .WithMany()
                        .HasForeignKey("HandsItemGuid")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("GhostSwordPlugin.Models.PlayerItem", "HeadItem")
                        .WithMany()
                        .HasForeignKey("HeadItemGuid")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("GhostSwordPlugin.Models.PlayerItem", "LegsItem")
                        .WithMany()
                        .HasForeignKey("LegsItemGuid")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("GhostSwordPlugin.Models.Place", "Place")
                        .WithMany()
                        .HasForeignKey("PlaceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GhostSwordPlugin.Models.PlayerItem", b =>
                {
                    b.HasOne("GhostSwordPlugin.Models.Item", "Item")
                        .WithMany()
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GhostSwordPlugin.Models.Player", "Player")
                        .WithMany()
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GhostSwordPlugin.Models.PlayerPlace", b =>
                {
                    b.HasOne("GhostSwordPlugin.Models.PlaceLink", "PlaceLink")
                        .WithMany()
                        .HasForeignKey("PlaceLinkId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GhostSwordPlugin.Models.Player", "Player")
                        .WithMany()
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
