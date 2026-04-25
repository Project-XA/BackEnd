using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project_X.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSectionCodeAndMembershipFlow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sections_SectionCode",
                table: "Sections");

            migrationBuilder.DropColumn(
                name: "SectionCode",
                table: "Sections");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SectionCode",
                table: "Sections",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Sections_SectionCode",
                table: "Sections",
                column: "SectionCode",
                unique: true);
        }
    }
}
