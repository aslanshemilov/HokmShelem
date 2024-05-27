using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Data.ContextM.Migrations
{
    /// <inheritdoc />
    public partial class AddingLeftByToGameHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LeftBy",
                table: "GameHistory",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LeftBy",
                table: "GameHistory");
        }
    }
}
