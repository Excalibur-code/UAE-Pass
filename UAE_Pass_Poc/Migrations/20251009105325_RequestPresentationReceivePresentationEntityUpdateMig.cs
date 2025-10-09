using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UAE_Pass_Poc.Migrations
{
    /// <inheritdoc />
    public partial class RequestPresentationReceivePresentationEntityUpdateMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ReceivePresentation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProofOfPresentationId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProofOfPresentationRequestId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QrId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SignedPresentation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CitizenSignature = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceivePresentation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReceivePresentationResponse",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestPresentationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProofOfPresentationId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReceivePresentationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PresentationReceiptId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceivePresentationResponse", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RequestPresentation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PurposeEN = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PurposeAR = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Request = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Mobile = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Origin = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestedVerifiedAttributes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestPresentation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RequestPresentationResponseMapping",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestPresentationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProofOfPresentationId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestPresentationResponseMapping", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Document",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CustomDocumentTypeEN = table.Column<string>(type: "nvarchar(35)", maxLength: 35, nullable: true),
                    CustomDocumentTypeAR = table.Column<string>(type: "nvarchar(35)", maxLength: 35, nullable: true),
                    Required = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    RequiredAttested = table.Column<bool>(type: "bit", nullable: true),
                    AllowExpired = table.Column<bool>(type: "bit", nullable: true),
                    SelfSignedAccepted = table.Column<bool>(type: "bit", nullable: true),
                    Emirate = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SingleInstanceRequested = table.Column<bool>(type: "bit", nullable: true),
                    RequestPresentationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Document", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Document_RequestPresentation_RequestPresentationId",
                        column: x => x.RequestPresentationId,
                        principalTable: "RequestPresentation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentInstance",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Value = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentInstance", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentInstance_Document_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Document",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Document_RequestPresentationId",
                table: "Document",
                column: "RequestPresentationId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentInstance_DocumentId",
                table: "DocumentInstance",
                column: "DocumentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentInstance");

            migrationBuilder.DropTable(
                name: "ReceivePresentation");

            migrationBuilder.DropTable(
                name: "ReceivePresentationResponse");

            migrationBuilder.DropTable(
                name: "RequestPresentationResponseMapping");

            migrationBuilder.DropTable(
                name: "Document");

            migrationBuilder.DropTable(
                name: "RequestPresentation");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Users");
        }
    }
}
