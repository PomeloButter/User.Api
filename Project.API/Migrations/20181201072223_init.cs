using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Project.API.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    UserId = table.Column<int>(nullable: false),
                    Avator = table.Column<string>(nullable: true),
                    Company = table.Column<string>(nullable: true),
                    OriginBpFile = table.Column<string>(nullable: true),
                    ForamteBpFile = table.Column<string>(nullable: true),
                    ShowSecurityInfo = table.Column<short>(nullable: false),
                    ProvinceId = table.Column<int>(nullable: false),
                    ProvinceName = table.Column<string>(nullable: true),
                    CityId = table.Column<int>(nullable: false),
                    CityName = table.Column<string>(nullable: true),
                    AreaId = table.Column<int>(nullable: false),
                    AreaName = table.Column<string>(nullable: true),
                    RegisterDateTime = table.Column<DateTime>(nullable: false),
                    Introduction = table.Column<string>(nullable: true),
                    FinPercentag = table.Column<string>(nullable: true),
                    FinStage = table.Column<string>(nullable: true),
                    FinMoney = table.Column<int>(nullable: false),
                    Income = table.Column<int>(nullable: false),
                    Revenue = table.Column<int>(nullable: false),
                    Valuation = table.Column<int>(nullable: false),
                    BrokerageOptions = table.Column<int>(nullable: false),
                    OnPlatform = table.Column<short>(nullable: false),
                    SourceId = table.Column<int>(nullable: false),
                    RefenceId = table.Column<int>(nullable: false),
                    Tags = table.Column<string>(nullable: true),
                    UpdateTime = table.Column<DateTime>(nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProjectContributor",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    ProjectId = table.Column<int>(nullable: false),
                    UserName = table.Column<string>(nullable: true),
                    Avator = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    IsColsed = table.Column<short>(nullable: false),
                    ContributorType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectContributor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectContributor_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectPropetry",
                columns: table => new
                {
                    ProjectId = table.Column<int>(nullable: false),
                    Key = table.Column<string>(maxLength: 100, nullable: false),
                    Value = table.Column<string>(maxLength: 100, nullable: false),
                    Text = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectPropetry", x => new { x.ProjectId, x.Key, x.Value });
                    table.ForeignKey(
                        name: "FK_ProjectPropetry_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectViewer",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    ProjectId = table.Column<int>(nullable: false),
                    UserName = table.Column<string>(nullable: true),
                    Avator = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectViewer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectViewer_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectVisableRule",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    ProjectId = table.Column<int>(nullable: false),
                    Visable = table.Column<short>(nullable: false),
                    Tags = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectVisableRule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectVisableRule_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectContributor_ProjectId",
                table: "ProjectContributor",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectViewer_ProjectId",
                table: "ProjectViewer",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectVisableRule_ProjectId",
                table: "ProjectVisableRule",
                column: "ProjectId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectContributor");

            migrationBuilder.DropTable(
                name: "ProjectPropetry");

            migrationBuilder.DropTable(
                name: "ProjectViewer");

            migrationBuilder.DropTable(
                name: "ProjectVisableRule");

            migrationBuilder.DropTable(
                name: "Projects");
        }
    }
}
