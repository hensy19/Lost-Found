using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lost_Found.Migrations
{
    /// <inheritdoc />
    public partial class AddWasRejectedFlag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "WasRejected",
                table: "Items",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WasRejected",
                table: "Items");
        }
    }
}
