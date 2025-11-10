using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UAE_Pass_Poc.Migrations
{
    /// <inheritdoc />
    public partial class ReceiveVisualizationEntityMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReceiveVisualization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProofOfPresentationId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VCId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VisualizationInfo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EvidenceInfo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IssuerSignature = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceiveVisualization", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReceiveVisualizationResponse",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReceiveVisualizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EvidenceVisualizationReceiptID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReceivePresentationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProofOfPresentationId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceiveVisualizationResponse", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReceiveVisualization");

            migrationBuilder.DropTable(
                name: "ReceiveVisualizationResponse");
        }
    }
}
