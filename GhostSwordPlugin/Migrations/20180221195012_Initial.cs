using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GhostSwordPlugin.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(nullable: true),
                    Emoji = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Places",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Places", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ItemDiscoveries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ItemId = table.Column<int>(nullable: false),
                    MaxAmount = table.Column<int>(nullable: false),
                    MinAmount = table.Column<int>(nullable: false),
                    PlaceId = table.Column<int>(nullable: false),
                    Rate = table.Column<float>(nullable: false),
                    Text = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemDiscoveries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemDiscoveries_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemDiscoveries_Places_PlaceId",
                        column: x => x.PlaceId,
                        principalTable: "Places",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NPCs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Greetings = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    PlaceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NPCs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NPCs_Places_PlaceId",
                        column: x => x.PlaceId,
                        principalTable: "Places",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlaceAdjacencies",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BeginText = table.Column<string>(nullable: false),
                    EndText = table.Column<string>(nullable: false),
                    JourneyDuration = table.Column<int>(nullable: false),
                    Place1Id = table.Column<int>(nullable: false),
                    Place2Id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaceAdjacencies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlaceAdjacencies_Places_Place1Id",
                        column: x => x.Place1Id,
                        principalTable: "Places",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlaceAdjacencies_Places_Place2Id",
                        column: x => x.Place2Id,
                        principalTable: "Places",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsBusy = table.Column<bool>(nullable: false, defaultValue: false),
                    MenuId = table.Column<int>(nullable: false, defaultValue: 1),
                    PlaceId = table.Column<int>(nullable: false, defaultValue: 1),
                    UserId = table.Column<long>(nullable: false),
                    Username = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Players_Places_PlaceId",
                        column: x => x.PlaceId,
                        principalTable: "Places",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Dialogues",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    NPCId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Text = table.Column<string>(nullable: true, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dialogues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dialogues_NPCs_NPCId",
                        column: x => x.NPCId,
                        principalTable: "NPCs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Journeys",
                columns: table => new
                {
                    Guid = table.Column<Guid>(nullable: false),
                    Duration = table.Column<int>(nullable: false),
                    PlaceAdjacencyId = table.Column<int>(nullable: false),
                    PlayerId = table.Column<int>(nullable: false),
                    StartTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Journeys", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_Journeys_PlaceAdjacencies_PlaceAdjacencyId",
                        column: x => x.PlaceAdjacencyId,
                        principalTable: "PlaceAdjacencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Journeys_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerItems",
                columns: table => new
                {
                    Guid = table.Column<Guid>(nullable: false),
                    Amount = table.Column<int>(nullable: false),
                    ItemId = table.Column<int>(nullable: false),
                    PlayerId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerItems", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_PlayerItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerItems_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Dialogues_NPCId",
                table: "Dialogues",
                column: "NPCId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemDiscoveries_ItemId",
                table: "ItemDiscoveries",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemDiscoveries_PlaceId",
                table: "ItemDiscoveries",
                column: "PlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Journeys_PlaceAdjacencyId",
                table: "Journeys",
                column: "PlaceAdjacencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Journeys_PlayerId",
                table: "Journeys",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_NPCs_PlaceId",
                table: "NPCs",
                column: "PlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaceAdjacencies_Place1Id",
                table: "PlaceAdjacencies",
                column: "Place1Id");

            migrationBuilder.CreateIndex(
                name: "IX_PlaceAdjacencies_Place2Id",
                table: "PlaceAdjacencies",
                column: "Place2Id");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerItems_ItemId",
                table: "PlayerItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerItems_PlayerId",
                table: "PlayerItems",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_PlaceId",
                table: "Players",
                column: "PlaceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Dialogues");

            migrationBuilder.DropTable(
                name: "ItemDiscoveries");

            migrationBuilder.DropTable(
                name: "Journeys");

            migrationBuilder.DropTable(
                name: "PlayerItems");

            migrationBuilder.DropTable(
                name: "NPCs");

            migrationBuilder.DropTable(
                name: "PlaceAdjacencies");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "Places");
        }
    }
}
