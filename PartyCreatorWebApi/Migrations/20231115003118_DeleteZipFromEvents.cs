using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PartyCreatorWebApi.Migrations
{
    /// <inheritdoc />
    public partial class DeleteZipFromEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Zip",
                table: "Events");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Zip",
                table: "Events",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
