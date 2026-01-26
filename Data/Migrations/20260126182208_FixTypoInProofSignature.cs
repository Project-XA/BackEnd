using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project_X.Migrations
{
    /// <inheritdoc />
    public partial class FixTypoInProofSignature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProofSignture",
                table: "VerificationSessions",
                newName: "ProofSignature");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProofSignature",
                table: "VerificationSessions",
                newName: "ProofSignture");
        }
    }
}
