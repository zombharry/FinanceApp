using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Item.Api.Migrations
{
    /// <inheritdoc />
    public partial class ExtendedDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "Products",
                newName: "CreatedAt");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Products",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Products",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Categories",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Categories",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Categories",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Deliveries",
                columns: table => new
                {
                    DeliveryId = table.Column<Guid>(type: "TEXT", nullable: false),
                    DeliveredItemId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Amount = table.Column<int>(type: "INTEGER", nullable: false),
                    Cost = table.Column<double>(type: "REAL", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deliveries", x => x.DeliveryId);
                    table.ForeignKey(
                        name: "FK_Deliveries_Products_DeliveredItemId",
                        column: x => x.DeliveredItemId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PurchasedProducts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    PriceAtPurchase = table.Column<double>(type: "REAL", nullable: true),
                    OwnerId = table.Column<string>(type: "TEXT", nullable: false),
                    CategoryId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProductId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchasedProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchasedProducts_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchasedProducts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Stocks",
                columns: table => new
                {
                    StockId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProductId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "BLOB", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stocks", x => x.StockId);
                    table.ForeignKey(
                        name: "FK_Stocks_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_DeliveredItemId",
                table: "Deliveries",
                column: "DeliveredItemId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchasedProducts_CategoryId",
                table: "PurchasedProducts",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchasedProducts_ProductId",
                table: "PurchasedProducts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_ProductId",
                table: "Stocks",
                column: "ProductId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Deliveries");

            migrationBuilder.DropTable(
                name: "PurchasedProducts");

            migrationBuilder.DropTable(
                name: "Stocks");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Categories");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Products",
                newName: "OwnerId");
        }
    }
}
