using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockSimulator.Migrations
{
    /// <inheritdoc />
    public partial class ChangeStocksTableName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Stacks",
                table: "Stacks");

            migrationBuilder.RenameTable(
                name: "Stacks",
                newName: "Stocks");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Stocks",
                table: "Stocks",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Stocks",
                table: "Stocks");

            migrationBuilder.RenameTable(
                name: "Stocks",
                newName: "Stacks");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Stacks",
                table: "Stacks",
                column: "Id");
        }
    }
}
