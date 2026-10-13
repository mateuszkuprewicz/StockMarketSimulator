using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StockSimulator.Migrations
{
    /// <inheritdoc />
    public partial class OrderBook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_AspNetUsers_UserId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Stocks_StockId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_UserShares_Stocks_StockId",
                table: "UserShares");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_StockId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_UserId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "StockId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Transactions");

            migrationBuilder.RenameColumn(
                name: "StockId",
                table: "UserShares",
                newName: "CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_UserShares_StockId",
                table: "UserShares",
                newName: "IX_UserShares_CompanyId");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Transactions",
                newName: "CompanyId");

            migrationBuilder.AddColumn<string>(
                name: "UserBuyerId",
                table: "Transactions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserSellerId",
                table: "Transactions",
                type: "TEXT",
                nullable: true);

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
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    CompanyId = table.Column<int>(type: "INTEGER", nullable: false),
                    Discriminator = table.Column<string>(type: "TEXT", maxLength: 21, nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketOffers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarketOffers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MarketOffers_Stocks_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Stocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "MarketOffers",
                columns: new[] { "Id", "CompanyId", "Discriminator", "Price", "Quantity" },
                values: new object[,]
                {
                    { 1, 1, "CompanyMarketOffer", 2.23m, 10 },
                    { 2, 2, "CompanyMarketOffer", 1024m, 8 },
                    { 3, 3, "CompanyMarketOffer", 0.5m, 12 },
                    { 4, 4, "CompanyMarketOffer", 25.13m, 15 },
                    { 5, 5, "CompanyMarketOffer", 314m, 3 }
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
                name: "IX_Transactions_CompanyId",
                table: "Transactions",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_UserBuyerId",
                table: "Transactions",
                column: "UserBuyerId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_UserSellerId",
                table: "Transactions",
                column: "UserSellerId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Transaction_No_Company_To_Company",
                table: "Transactions",
                sql: "UserBuyerId IS NOT NULL OR UserSellerId IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MarketOffers_CompanyId",
                table: "MarketOffers",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketOffers_UserId",
                table: "MarketOffers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_AspNetUsers_UserBuyerId",
                table: "Transactions",
                column: "UserBuyerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_AspNetUsers_UserSellerId",
                table: "Transactions",
                column: "UserSellerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Stocks_CompanyId",
                table: "Transactions",
                column: "CompanyId",
                principalTable: "Stocks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserShares_Stocks_CompanyId",
                table: "UserShares",
                column: "CompanyId",
                principalTable: "Stocks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_AspNetUsers_UserBuyerId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_AspNetUsers_UserSellerId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Stocks_CompanyId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_UserShares_Stocks_CompanyId",
                table: "UserShares");

            migrationBuilder.DropTable(
                name: "MarketOffers");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_CompanyId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_UserBuyerId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_UserSellerId",
                table: "Transactions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Transaction_No_Company_To_Company",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "UserBuyerId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "UserSellerId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "CompanyBudget",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "FreeFloat",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "TotalShares",
                table: "Stocks");

            migrationBuilder.RenameColumn(
                name: "CompanyId",
                table: "UserShares",
                newName: "StockId");

            migrationBuilder.RenameIndex(
                name: "IX_UserShares_CompanyId",
                table: "UserShares",
                newName: "IX_UserShares_StockId");

            migrationBuilder.RenameColumn(
                name: "CompanyId",
                table: "Transactions",
                newName: "Type");

            migrationBuilder.AddColumn<int>(
                name: "StockId",
                table: "Transactions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Transactions",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_StockId",
                table: "Transactions",
                column: "StockId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_UserId",
                table: "Transactions",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_AspNetUsers_UserId",
                table: "Transactions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Stocks_StockId",
                table: "Transactions",
                column: "StockId",
                principalTable: "Stocks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserShares_Stocks_StockId",
                table: "UserShares",
                column: "StockId",
                principalTable: "Stocks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
