using Microsoft.EntityFrameworkCore.Migrations;

namespace v3x.Migrations
{
    public partial class add_new_schema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Addtess",
                table: "People",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DateOfBirth",
                table: "People",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Nationality",
                table: "People",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Addtess",
                table: "People");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "People");

            migrationBuilder.DropColumn(
                name: "Nationality",
                table: "People");
        }
    }
}
