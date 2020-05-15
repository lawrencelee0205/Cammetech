using Microsoft.EntityFrameworkCore.Migrations;

namespace v3x.Migrations
{
    public partial class update_address : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Addtess",
                table: "People");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "People",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "People");

            migrationBuilder.AddColumn<string>(
                name: "Addtess",
                table: "People",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
