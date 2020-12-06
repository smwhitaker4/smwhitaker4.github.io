using Microsoft.EntityFrameworkCore.Migrations;

namespace LiarsDiceApi.Migrations
{
    public partial class AddRulesText : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RulesText",
                table: "Rules",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RulesText",
                table: "Rules");
        }
    }
}
