using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PartyCreatorWebApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedSongs2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Songs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Songs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
