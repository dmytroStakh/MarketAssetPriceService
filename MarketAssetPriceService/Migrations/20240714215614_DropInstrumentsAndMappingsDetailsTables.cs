﻿using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MarketAssetPriceService.Migrations
{
    public partial class DropInstrumentsAndMappingsDetailsTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropTable(
                name: "Instruments");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Instruments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Symbol = table.Column<string>(type: "text", nullable: false),
                    Kind = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    TickSize = table.Column<decimal>(type: "numeric", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    BaseCurrency = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instruments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MappingsDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Provider = table.Column<string>(type: "text", nullable: true),
                    Symbol = table.Column<string>(type: "text", nullable: true),
                    Exchange = table.Column<string>(type: "text", nullable: true),
                    DefaultOrderSize = table.Column<int>(type: "integer", nullable: false),
                    InstrumentId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MappingsDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MappingsDetails_Instruments_InstrumentId",
                        column: x => x.InstrumentId,
                        principalTable: "Instruments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
