using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace v3x.Migrations
{
    public partial class new_schema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Attendance",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(nullable: false),
                    EmployeeId = table.Column<int>(nullable: false),
                    Status = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attendance", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "People",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false),
                    Password = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: false),
                    Role = table.Column<string>(nullable: true),
                    Tel = table.Column<string>(nullable: false),
                    Nationality = table.Column<string>(nullable: false),
                    DateOfBirth = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_People", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Job",
                columns: table => new
                {
                    JobId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<string>(nullable: true),
                    BasePay = table.Column<double>(nullable: false),
                    Position = table.Column<string>(nullable: true),
                    PeopleId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Job", x => x.JobId);
                    table.ForeignKey(
                        name: "FK_Job_People_PeopleId",
                        column: x => x.PeopleId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaySlip",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(nullable: false),
                    AdvancePay = table.Column<double>(nullable: false),
                    Bonus = table.Column<double>(nullable: false),
                    Basic = table.Column<double>(nullable: false),
                    NetSalary = table.Column<double>(nullable: false),
                    EmployeeEPF = table.Column<double>(nullable: false),
                    EmployerEPF = table.Column<double>(nullable: false),
                    EmployeeSocso = table.Column<double>(nullable: false),
                    EmployerSocso = table.Column<double>(nullable: false),
                    JobId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaySlip", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaySlip_Job_JobId",
                        column: x => x.JobId,
                        principalTable: "Job",
                        principalColumn: "JobId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Job_PeopleId",
                table: "Job",
                column: "PeopleId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaySlip_JobId",
                table: "PaySlip",
                column: "JobId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Attendance");

            migrationBuilder.DropTable(
                name: "PaySlip");

            migrationBuilder.DropTable(
                name: "Job");

            migrationBuilder.DropTable(
                name: "People");
        }
    }
}
