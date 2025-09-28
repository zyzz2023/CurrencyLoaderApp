using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CurrencyRates",
                columns: table => new
                {
                    CurrencyCode = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CurrencyName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Nominal = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyRates", x => new { x.CurrencyCode, x.Date });
                });

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyRates_CurrencyCode_Date",
                table: "CurrencyRates",
                columns: new[] { "CurrencyCode", "Date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyRates_Date",
                table: "CurrencyRates",
                column: "Date");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CurrencyRates");
        }
    }
}
