using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PartyCreatorWebApi.Migrations
{
    /// <inheritdoc />
    public partial class NewShoppingList : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "ShoppingListItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Price",
                table: "ShoppingListItems",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
