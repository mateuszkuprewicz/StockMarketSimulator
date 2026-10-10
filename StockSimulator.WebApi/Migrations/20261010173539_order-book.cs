using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StockSimulator.Migrations
{
    /// <inheritdoc />
    public partial class orderbook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CompanyBudget",
                table: "Stocks",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "FreeFloat",
                table: "Stocks",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalShares",
                table: "Stocks",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "MarketOffers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Price = table.Column<decimal>(type: "TEXT", nullable: false),
                    Amount = table.Column<int>(type: "INTEGER", nullable: false),
                    StockId = table.Column<int>(type: "INTEGER", nullable: false),
                    Discriminator = table.Column<string>(type: "TEXT", maxLength: 21, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketOffers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarketOffers_Stocks_StockId",
                        column: x => x.StockId,
                        principalTable: "Stocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "MarketOffers",
                columns: new[] { "Id", "Amount", "Discriminator", "Price", "StockId" },
                values: new object[,]
                {
                    { 1, 10, "CompanyMarketOffer", 2.23m, 1 },
                    { 2, 8, "CompanyMarketOffer", 1024m, 2 },
                    { 3, 12, "CompanyMarketOffer", 0.5m, 3 },
                    { 4, 15, "CompanyMarketOffer", 25.13m, 4 },
                    { 5, 3, "CompanyMarketOffer", 314m, 5 }
                });

            migrationBuilder.UpdateData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CompanyBudget", "FreeFloat", "TotalShares" },
                values: new object[] { 0m, 10, 1000 });

            migrationBuilder.UpdateData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CompanyBudget", "FreeFloat", "TotalShares" },
                values: new object[] { 0m, 8, 500 });

            migrationBuilder.UpdateData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CompanyBudget", "FreeFloat", "TotalShares" },
                values: new object[] { 0m, 12, 100 });

            migrationBuilder.UpdateData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CompanyBudget", "FreeFloat", "TotalShares" },
                values: new object[] { 0m, 15, 200 });

            migrationBuilder.UpdateData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CompanyBudget", "FreeFloat", "TotalShares" },
                values: new object[] { 0m, 3, 50 });

            migrationBuilder.CreateIndex(
                name: "IX_MarketOffers_StockId",
                table: "MarketOffers",
                column: "StockId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MarketOffers");

            migrationBuilder.DropColumn(
                name: "CompanyBudget",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "FreeFloat",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "TotalShares",
                table: "Stocks");
        }
    }
}
