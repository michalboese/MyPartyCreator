using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PartyCreatorWebApi.Migrations
{
    /// <inheritdoc />
    public partial class SurveyChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SurveyVoteId",
                table: "Choices");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SurveyVoteId",
                table: "Choices",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
