using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ScrapBot.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Notifiers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DiscordId = table.Column<decimal>(nullable: false),
                    NextTrigger = table.Column<DateTimeOffset>(nullable: false),
                    Interval = table.Column<TimeSpan>(nullable: false),
                    TriggerCount = table.Column<int>(nullable: false),
                    CompactDisplay = table.Column<bool>(nullable: false),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifiers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notifiers_DiscordId",
                table: "Notifiers",
                column: "DiscordId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifiers");
        }
    }
}
