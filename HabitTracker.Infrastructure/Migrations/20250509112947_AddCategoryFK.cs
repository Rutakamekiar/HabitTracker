using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HabitTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Records_CategoryId",
                table: "Records",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Records_Categories_CategoryId",
                table: "Records",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Records_Categories_CategoryId",
                table: "Records");

            migrationBuilder.DropIndex(
                name: "IX_Records_CategoryId",
                table: "Records");
        }
    }
}
