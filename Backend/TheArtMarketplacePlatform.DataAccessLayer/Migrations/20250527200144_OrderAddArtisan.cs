using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheArtMarketplacePlatform.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class OrderAddArtisan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ArtisanId",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ArtisanName",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArtisanId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ArtisanName",
                table: "Orders");
        }
    }
}
