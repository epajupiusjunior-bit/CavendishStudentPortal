using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CavendishACMISPortal.Migrations
{
    /// <inheritdoc />
    public partial class UpdateResultWithCATs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Score",
                table: "Results",
                newName: "FinalExam");

            migrationBuilder.RenameColumn(
                name: "Grade",
                table: "Results",
                newName: "CAT2");

            migrationBuilder.AddColumn<decimal>(
                name: "CAT1",
                table: "Results",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CAT1",
                table: "Results");

            migrationBuilder.RenameColumn(
                name: "FinalExam",
                table: "Results",
                newName: "Score");

            migrationBuilder.RenameColumn(
                name: "CAT2",
                table: "Results",
                newName: "Grade");
        }
    }
}
