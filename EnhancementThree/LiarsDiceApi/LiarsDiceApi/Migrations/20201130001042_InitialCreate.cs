using Microsoft.EntityFrameworkCore.Migrations;

namespace LiarsDiceApi.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumberOfDiceGuess = table.Column<int>(type: "int", nullable: false),
                    ValueOfDieGuess = table.Column<int>(type: "int", nullable: false),
                    CalledLiar = table.Column<bool>(type: "bit", nullable: false),
                    GameId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiceValueOne = table.Column<int>(type: "int", nullable: false),
                    DiceValueTwo = table.Column<int>(type: "int", nullable: false),
                    DiceValueThree = table.Column<int>(type: "int", nullable: false),
                    DiceValueFour = table.Column<int>(type: "int", nullable: false),
                    DiceValueFive = table.Column<int>(type: "int", nullable: false),
                    DiceValueSix = table.Column<int>(type: "int", nullable: false),
                    WinnerId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Games_Players_WinnerId",
                        column: x => x.WinnerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Games_WinnerId",
                table: "Games",
                column: "WinnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_GameId1",
                table: "Players",
                column: "GameId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Games_GameId1",
                table: "Players",
                column: "GameId1",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_Players_WinnerId",
                table: "Games");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "Games");
        }
    }
}
