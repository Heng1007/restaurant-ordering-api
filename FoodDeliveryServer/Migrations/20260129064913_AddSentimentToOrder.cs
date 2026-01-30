using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodDeliveryServer.Migrations
{
    /// <inheritdoc />
    public partial class AddSentimentToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerNote",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sentiment",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerNote",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Sentiment",
                table: "Orders");
        }
    }
}
