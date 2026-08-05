using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Item.Api.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedPurchasedProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Amount",
                table: "PurchasedProducts",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "PurchasedProducts");
        }
    }
}
