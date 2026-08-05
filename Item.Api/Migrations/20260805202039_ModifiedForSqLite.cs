using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Item.Api.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedForSqLite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Stocks");

            migrationBuilder.AddColumn<Guid>(
                name: "Version",
                table: "Stocks",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Version",
                table: "Stocks");

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Stocks",
                type: "BLOB",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);
        }
    }
}
