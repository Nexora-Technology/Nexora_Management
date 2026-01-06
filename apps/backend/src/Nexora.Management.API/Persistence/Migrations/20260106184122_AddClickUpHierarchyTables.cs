using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexora.Management.API.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddClickUpHierarchyTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_comments_parent",
                table: "Comments");

            // DATA MIGRATION: Add TaskListId to TaskStatuses (nullable → migrate → NOT NULL)
            migrationBuilder.AddColumn<Guid>(
                name: "TaskListId",
                table: "TaskStatuses",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TaskListId",
                table: "Tasks",
                type: "uuid",
                nullable: true);

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
                name: "Spaces",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Color = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: true),
                    Icon = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IsPrivate = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    SettingsJsonb = table.Column<Dictionary<string, object>>(type: "jsonb", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spaces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Spaces_Workspaces_WorkspaceId",
                        column: x => x.WorkspaceId,
                        principalTable: "Workspaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "Folders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    SpaceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Color = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: true),
                    Icon = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    PositionOrder = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    SettingsJsonb = table.Column<Dictionary<string, object>>(type: "jsonb", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Folders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Folders_Spaces_SpaceId",
                        column: x => x.SpaceId,
                        principalTable: "Spaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.CreateTable(
                name: "TaskLists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    SpaceId = table.Column<Guid>(type: "uuid", nullable: false),
                    FolderId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Color = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: true),
                    Icon = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ListType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "task"),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "active"),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    PositionOrder = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    SettingsJsonb = table.Column<Dictionary<string, object>>(type: "jsonb", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskLists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskLists_Folders_FolderId",
                        column: x => x.FolderId,
                        principalTable: "Folders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskLists_Spaces_SpaceId",
                        column: x => x.SpaceId,
                        principalTable: "Spaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskLists_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            // DATA MIGRATION: Create default Spaces for each Workspace
            // Each Workspace gets one default Space named "General" to host migrated TaskLists
            migrationBuilder.Sql(
                @"INSERT INTO ""Spaces"" (""Id"", ""WorkspaceId"", ""Name"", ""Description"", ""Color"", ""Icon"", ""IsPrivate"", ""SettingsJsonb"", ""CreatedAt"", ""UpdatedAt"")
                  SELECT uuid_generate_v4(), ""Id"", 'General', 'Default space for migrated projects', NULL, NULL, false, '{}'::jsonb, NOW(), NOW()
                  FROM ""Workspaces""
                  WHERE NOT EXISTS (SELECT 1 FROM ""Spaces"" WHERE ""Spaces"".""WorkspaceId"" = ""Workspaces"".""Id"");");

            // DATA MIGRATION: Copy all Projects to TaskLists (preserving IDs)
            // This creates TaskLists from existing Projects, using the default Space from Workspace
            migrationBuilder.Sql(
                @"INSERT INTO ""TaskLists"" (""Id"", ""SpaceId"", ""FolderId"", ""Name"", ""Description"", ""Color"", ""Icon"", ""ListType"", ""Status"", ""OwnerId"", ""PositionOrder"", ""SettingsJsonb"", ""CreatedAt"", ""UpdatedAt"")
                  SELECT p.""Id"", s.""Id"", NULL, p.""Name"", p.""Description"", p.""Color"", p.""Icon"", 'task', p.""Status"", p.""OwnerId"", 0, p.""SettingsJsonb"", p.""CreatedAt"", p.""UpdatedAt""
                  FROM ""Projects"" p
                  INNER JOIN ""Spaces"" s ON s.""WorkspaceId"" = p.""WorkspaceId""
                  WHERE NOT EXISTS (SELECT 1 FROM ""TaskLists"" WHERE ""TaskLists"".""Id"" = p.""Id"");");

            // DATA MIGRATION: Validate all Projects were migrated to TaskLists
            migrationBuilder.Sql(
                @"DO $$
                  BEGIN
                      IF (SELECT COUNT(*) FROM ""Projects"") <> (SELECT COUNT(*) FROM ""TaskLists"" INNER JOIN ""Projects"" ON ""TaskLists"".""Id"" = ""Projects"".""Id"") THEN
                          RAISE EXCEPTION 'Data migration failed: Not all Projects were copied to TaskLists';
                      END IF;
                  END $$;");

            // DATA MIGRATION: Migrate Task.ProjectId → Task.TaskListId
            migrationBuilder.Sql(@"UPDATE ""Tasks"" SET ""TaskListId"" = ""ProjectId"" WHERE ""TaskListId"" IS NULL;");

            // DATA MIGRATION: Validate all Tasks have TaskListId
            migrationBuilder.Sql(
                @"DO $$
                  BEGIN
                      IF EXISTS (SELECT 1 FROM ""Tasks"" WHERE ""TaskListId"" IS NULL) THEN
                          RAISE EXCEPTION 'Data migration failed: Some Tasks do not have TaskListId';
                      END IF;
                  END $$;");

            // DATA MIGRATION: Migrate TaskStatus.ProjectId → TaskStatus.TaskListId
            migrationBuilder.Sql(@"UPDATE ""TaskStatuses"" SET ""TaskListId"" = ""ProjectId"" WHERE ""TaskListId"" IS NULL;");

            // DATA MIGRATION: Validate all TaskStatuses have TaskListId
            migrationBuilder.Sql(
                @"DO $$
                  BEGIN
                      IF EXISTS (SELECT 1 FROM ""TaskStatuses"" WHERE ""TaskListId"" IS NULL) THEN
                          RAISE EXCEPTION 'Data migration failed: Some TaskStatuses do not have TaskListId';
                      END IF;
                  END $$;");

            // DATA MIGRATION: Make TaskListId NOT NULL after data migration
            migrationBuilder.AlterColumn<Guid>(
                name: "TaskListId",
                table: "Tasks",
                type: "uuid",
                nullable: false);

            migrationBuilder.AlterColumn<Guid>(
                name: "TaskListId",
                table: "TaskStatuses",
                type: "uuid",
                nullable: false);

            migrationBuilder.CreateIndex(
                name: "idx_taskstatuses_tasklist",
                table: "TaskStatuses",
                column: "TaskListId");

            migrationBuilder.CreateIndex(
                name: "uq_taskstatuses_tasklist_order",
                table: "TaskStatuses",
                columns: new[] { "TaskListId", "OrderIndex" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_tasks_tasklist",
                table: "Tasks",
                column: "TaskListId");

            migrationBuilder.CreateIndex(
                name: "idx_comments_parent",
                table: "Comments",
                column: "ParentCommentId",
                filter: "\"ParentCommentId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "idx_folders_space",
                table: "Folders",
                column: "SpaceId");

            migrationBuilder.CreateIndex(
                name: "uq_folders_space_position",
                table: "Folders",
                columns: new[] { "SpaceId", "PositionOrder" },
                unique: true);

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

            migrationBuilder.CreateIndex(
                name: "idx_spaces_workspace",
                table: "Spaces",
                column: "WorkspaceId");

            migrationBuilder.CreateIndex(
                name: "idx_tasklists_folder",
                table: "TaskLists",
                column: "FolderId");

            migrationBuilder.CreateIndex(
                name: "idx_tasklists_position",
                table: "TaskLists",
                columns: new[] { "SpaceId", "FolderId", "PositionOrder" });

            migrationBuilder.CreateIndex(
                name: "idx_tasklists_space_active",
                table: "TaskLists",
                column: "SpaceId",
                filter: "status = 'active'");

            migrationBuilder.CreateIndex(
                name: "IX_TaskLists_OwnerId",
                table: "TaskLists",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_TaskLists_TaskListId",
                table: "Tasks",
                column: "TaskListId",
                principalTable: "TaskLists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskStatuses_TaskLists_TaskListId",
                table: "TaskStatuses",
                column: "TaskListId",
                principalTable: "TaskLists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_TaskLists_TaskListId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskStatuses_TaskLists_TaskListId",
                table: "TaskStatuses");

            migrationBuilder.DropIndex(
                name: "idx_taskstatuses_tasklist",
                table: "TaskStatuses");

            migrationBuilder.DropIndex(
                name: "uq_taskstatuses_tasklist_order",
                table: "TaskStatuses");

            migrationBuilder.DropIndex(
                name: "idx_tasks_tasklist",
                table: "Tasks");

            // ROLLBACK: Make TaskListId nullable again before dropping
            migrationBuilder.AlterColumn<Guid>(
                name: "TaskListId",
                table: "TaskStatuses",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "TaskListId",
                table: "Tasks",
                type: "uuid",
                nullable: true);

            // ROLLBACK: Drop TaskLists table (this removes the migrated Project copies)
            migrationBuilder.DropTable(
                name: "key_results");

            migrationBuilder.DropTable(
                name: "TaskLists");

            migrationBuilder.DropTable(
                name: "objectives");

            migrationBuilder.DropTable(
                name: "Folders");

            migrationBuilder.DropTable(
                name: "goal_periods");

            migrationBuilder.DropTable(
                name: "Spaces");

            // ROLLBACK: Remove TaskListId columns (ProjectId remains untouched, so no restore needed)
            migrationBuilder.DropColumn(
                name: "TaskListId",
                table: "TaskStatuses");

            migrationBuilder.DropColumn(
                name: "TaskListId",
                table: "Tasks");

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
