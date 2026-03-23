using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project_X.Migrations
{
    /// <inheritdoc />
    public partial class AddIsUniversityToOrganization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsUniversity",
                table: "Organizations",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsUniversity",
                table: "Organizations");
        }
    }
}
