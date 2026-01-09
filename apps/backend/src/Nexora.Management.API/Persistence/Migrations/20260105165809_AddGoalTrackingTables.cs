using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexora.Management.API.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddGoalTrackingTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_comments_parent",
                table: "Comments");

            migrationBuilder.CreateTable(
                name: "goal_periods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "active"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_goal_periods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_goal_periods_Workspaces_WorkspaceId",
                        column: x => x.WorkspaceId,
                        principalTable: "Workspaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "objectives",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false),
                    PeriodId = table.Column<Guid>(type: "uuid", nullable: true),
                    ParentObjectiveId = table.Column<Guid>(type: "uuid", nullable: true),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "text", maxLength: 2000, nullable: true),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: true),
                    Weight = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "on-track"),
                    Progress = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    PositionOrder = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_objectives", x => x.Id);
                    table.ForeignKey(
                        name: "FK_objectives_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_objectives_Workspaces_WorkspaceId",
                        column: x => x.WorkspaceId,
                        principalTable: "Workspaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_objectives_goal_periods_PeriodId",
                        column: x => x.PeriodId,
                        principalTable: "goal_periods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_objectives_objectives_ParentObjectiveId",
                        column: x => x.ParentObjectiveId,
                        principalTable: "objectives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "key_results",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    ObjectiveId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    MetricType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CurrentValue = table.Column<decimal>(type: "numeric", nullable: false),
                    TargetValue = table.Column<decimal>(type: "numeric", nullable: false),
                    Unit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Progress = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    Weight = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_key_results", x => x.Id);
                    table.ForeignKey(
                        name: "FK_key_results_objectives_ObjectiveId",
                        column: x => x.ObjectiveId,
                        principalTable: "objectives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "idx_comments_parent",
                table: "Comments",
                column: "ParentCommentId",
                filter: "\"ParentCommentId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_goal_periods_WorkspaceId",
                table: "goal_periods",
                column: "WorkspaceId");

            migrationBuilder.CreateIndex(
                name: "IX_key_results_ObjectiveId",
                table: "key_results",
                column: "ObjectiveId");

            migrationBuilder.CreateIndex(
                name: "IX_key_results_ObjectiveId_DueDate",
                table: "key_results",
                columns: new[] { "ObjectiveId", "DueDate" });

            migrationBuilder.CreateIndex(
                name: "IX_objectives_OwnerId",
                table: "objectives",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_objectives_ParentObjectiveId",
                table: "objectives",
                column: "ParentObjectiveId");

            migrationBuilder.CreateIndex(
                name: "IX_objectives_PeriodId",
                table: "objectives",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_objectives_WorkspaceId",
                table: "objectives",
                column: "WorkspaceId");

            migrationBuilder.CreateIndex(
                name: "IX_objectives_WorkspaceId_ParentObjectiveId",
                table: "objectives",
                columns: new[] { "WorkspaceId", "ParentObjectiveId" });

            migrationBuilder.CreateIndex(
                name: "IX_objectives_WorkspaceId_Status",
                table: "objectives",
                columns: new[] { "WorkspaceId", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "key_results");

            migrationBuilder.DropTable(
                name: "objectives");

            migrationBuilder.DropTable(
                name: "goal_periods");

            migrationBuilder.DropIndex(
                name: "idx_comments_parent",
                table: "Comments");

            migrationBuilder.CreateIndex(
                name: "idx_comments_parent",
                table: "Comments",
                column: "ParentCommentId",
                filter: "\"ParentCommentId\" IS NOT NULL");
        }
    }
}
