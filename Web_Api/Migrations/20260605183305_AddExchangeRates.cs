using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web_Api.Migrations
{
    /// <inheritdoc />
    public partial class AddExchangeRates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExchangeRates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromCurrency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ToCurrency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rate = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeRates", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExchangeRates");
        }
    }
}
