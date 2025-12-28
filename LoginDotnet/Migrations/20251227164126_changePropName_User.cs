using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoginDotnet.Migrations
{
    /// <inheritdoc />
    public partial class changePropName_User : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProfilePictureUrl",
                table: "AspNetUsers",
                newName: "ProfilePath");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProfilePath",
                table: "AspNetUsers",
                newName: "ProfilePictureUrl");
        }
    }
}
