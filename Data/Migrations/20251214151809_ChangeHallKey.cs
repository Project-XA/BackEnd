using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Project_X.Migrations
{
    /// <inheritdoc />
    public partial class ChangeHallKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceSessions_Halls_HallId1_HallName",
                table: "AttendanceSessions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Halls",
                table: "Halls");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceSessions_HallId1_HallName",
                table: "AttendanceSessions");

            migrationBuilder.DropColumn(
                name: "HallId1",
                table: "AttendanceSessions");

            migrationBuilder.DropColumn(
                name: "HallName",
                table: "AttendanceSessions");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Halls",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Halls",
                table: "Halls",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceSessions_HallId",
                table: "AttendanceSessions",
                column: "HallId");

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceSessions_Halls_HallId",
                table: "AttendanceSessions",
                column: "HallId",
                principalTable: "Halls",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceSessions_Halls_HallId",
                table: "AttendanceSessions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Halls",
                table: "Halls");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceSessions_HallId",
                table: "AttendanceSessions");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Halls",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "HallId1",
                table: "AttendanceSessions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "HallName",
                table: "AttendanceSessions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Halls",
                table: "Halls",
                columns: new[] { "Id", "HallName" });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceSessions_HallId1_HallName",
                table: "AttendanceSessions",
                columns: new[] { "HallId1", "HallName" });

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceSessions_Halls_HallId1_HallName",
                table: "AttendanceSessions",
                columns: new[] { "HallId1", "HallName" },
                principalTable: "Halls",
                principalColumns: new[] { "Id", "HallName" },
                onDelete: ReferentialAction.Cascade);
        }
    }
}
