using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PartyCreatorWebApi.Migrations
{
    /// <inheritdoc />
    public partial class PowiadomieniaEdit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EventId",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsRead",
                table: "Notifications",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "IsRead",
                table: "Notifications");
        }
    }
}
