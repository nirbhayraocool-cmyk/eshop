using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eshop.Migrations
{
    /// <inheritdoc />
    public partial class AddSizeAndGallery : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GalleryImages",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sizes",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Size",
                table: "OrderItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Size",
                table: "CartItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "GalleryImages", "Sizes" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "GalleryImages", "Sizes" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "GalleryImages", "Sizes" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "GalleryImages", "Sizes" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "GalleryImages", "Sizes" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "GalleryImages", "Sizes" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "GalleryImages", "Sizes" },
                values: new object[] { null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GalleryImages",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Sizes",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Size",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "Size",
                table: "CartItems");
        }
    }
}
