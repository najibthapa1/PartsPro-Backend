using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PartsPro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCostPriceToPart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CostPrice",
                table: "Parts",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CostPrice",
                table: "Parts");
        }
    }
}
