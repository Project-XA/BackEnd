using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Project_X.Migrations
{
    /// <inheritdoc />
    public partial class AddHallModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HallId",
                table: "AttendanceSessions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Hall",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    HallName = table.Column<string>(type: "text", nullable: false),
                    Capacity = table.Column<int>(type: "integer", nullable: false),
                    HallArea = table.Column<double>(type: "double precision", nullable: false),
                    OrganizationId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hall", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Hall_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "OrganizationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceSessions_HallId",
                table: "AttendanceSessions",
                column: "HallId");

            migrationBuilder.CreateIndex(
                name: "IX_Hall_OrganizationId",
                table: "Hall",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceSessions_Hall_HallId",
                table: "AttendanceSessions",
                column: "HallId",
                principalTable: "Hall",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceSessions_Hall_HallId",
                table: "AttendanceSessions");

            migrationBuilder.DropTable(
                name: "Hall");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceSessions_HallId",
                table: "AttendanceSessions");

            migrationBuilder.DropColumn(
                name: "HallId",
                table: "AttendanceSessions");
        }
    }
}
