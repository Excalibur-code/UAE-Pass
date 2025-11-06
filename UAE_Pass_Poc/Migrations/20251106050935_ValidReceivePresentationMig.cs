using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UAE_Pass_Poc.Migrations
{
    /// <inheritdoc />
    public partial class ValidReceivePresentationMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPresentationValid",
                table: "ReceivePresentation",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPresentationValid",
                table: "ReceivePresentation");
        }
    }
}
