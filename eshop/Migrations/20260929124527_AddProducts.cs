using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace eshop.Migrations
{
    /// <inheritdoc />
    public partial class AddProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "CartItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OldPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Section = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Category", "ImageUrl", "IsActive", "Name", "OldPrice", "Price", "Section" },
                values: new object[,]
                {
                    { 1, "Fashion", "https://www.shutterstock.com/image-photo/asian-beautiful-young-women-look-260nw-2508426961.jpg", true, "Shirts", 3332m, 1999m, "Flash" },
                    { 2, "Fashion", "https://thumbs.dreamstime.com/b/near-clothes-rack-showcase-girls-buy-something-summer-wardrobe-choose-some-items-discount-season-promotional-offers-client-465644853.jpg", true, "Shirts", 2141m, 1499m, "Flash" },
                    { 3, "Fashion", "https://www.livemint.com/lm-img/img/2024/10/03/original/pe_1727956111669.jpg", true, "Fashions", 1332m, 999m, "Flash" },
                    { 4, "Shoes", "/imgshop/boot.webp", true, "Shoes", 2665m, 1999m, "Featured" },
                    { 5, "Shoes", "/imgshop/boot-2.webp", true, "Shoes", 2665m, 1999m, "Featured" },
                    { 6, "Shoes", "/imgshop/boot-3.jpg", true, "Shoes", 2665m, 1999m, "Featured" },
                    { 7, "Shoes", "/imgshop/boot4.jpg", true, "Shoes", 2665m, 1999m, "Featured" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "CartItems");
        }
    }
}
