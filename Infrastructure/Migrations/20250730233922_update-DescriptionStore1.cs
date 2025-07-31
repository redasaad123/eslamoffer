using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateDescriptionStore1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StoreId",
                table: "DescriptionStores",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_DescriptionStores_StoreId",
                table: "DescriptionStores",
                column: "StoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_DescriptionStores_Stores_StoreId",
                table: "DescriptionStores",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DescriptionStores_Stores_StoreId",
                table: "DescriptionStores");

            migrationBuilder.DropIndex(
                name: "IX_DescriptionStores_StoreId",
                table: "DescriptionStores");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "DescriptionStores");
        }
    }
}
