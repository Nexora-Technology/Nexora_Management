using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexora.Management.API.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTimeTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "time_entries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: true),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DurationMinutes = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IsBillable = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "draft"),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_time_entries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_time_entries_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_time_entries_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_time_entries_Workspaces_WorkspaceId",
                        column: x => x.WorkspaceId,
                        principalTable: "Workspaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "time_rates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: true),
                    HourlyRate = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false, defaultValue: "USD"),
                    EffectiveFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_time_rates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_time_rates_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_time_rates_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "idx_time_entries_start_time",
                table: "time_entries",
                column: "StartTime");

            migrationBuilder.CreateIndex(
                name: "idx_time_entries_status",
                table: "time_entries",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "idx_time_entries_task",
                table: "time_entries",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "idx_time_entries_user",
                table: "time_entries",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "idx_time_entries_user_time",
                table: "time_entries",
                columns: new[] { "UserId", "StartTime" });

            migrationBuilder.CreateIndex(
                name: "idx_time_entries_workspace",
                table: "time_entries",
                column: "WorkspaceId");

            migrationBuilder.CreateIndex(
                name: "idx_time_rates_project",
                table: "time_rates",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "idx_time_rates_project_effective",
                table: "time_rates",
                columns: new[] { "ProjectId", "EffectiveFrom" });

            migrationBuilder.CreateIndex(
                name: "idx_time_rates_user",
                table: "time_rates",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "idx_time_rates_user_effective",
                table: "time_rates",
                columns: new[] { "UserId", "EffectiveFrom" });

            // Enable Row-Level Security
            migrationBuilder.Sql(
                @"ALTER TABLE time_entries ENABLE ROW LEVEL SECURITY;");

            // Create policy function for time_entries
            migrationBuilder.Sql(
                @"
                CREATE OR REPLACE FUNCTION time_entries_rls_policy()
                RETURNS boolean AS $$
                BEGIN
                    -- Users can see their own time entries
                    IF UserId = current_setting('app.current_user_id', true)::uuid THEN
                        RETURN true;
                    END IF;

                    -- Managers can see time entries from their workspace
                    -- Check if user is a workspace member
                    IF EXISTS (
                        SELECT 1 FROM workspace_members
                        WHERE workspace_members.workspace_id = WorkspaceId
                        AND workspace_members.user_id = current_setting('app.current_user_id', true)::uuid
                    ) THEN
                        RETURN true;
                    END IF;

                    RETURN false;
                END;
                $$ LANGUAGE plpgsql SECURITY DEFINER;

                GRANT EXECUTE ON FUNCTION time_entries_rls_policy() TO PUBLIC;");

            // Create RLS policies for time_entries
            migrationBuilder.Sql(
                @"CREATE POLICY time_entries_select_policy ON time_entries
                FOR SELECT
                USING time_entries_rls_policy();");

            migrationBuilder.Sql(
                @"CREATE POLICY time_entries_insert_policy ON time_entries
                FOR INSERT
                WITH CHECK (
                    UserId = current_setting('app.current_user_id', true)::uuid
                );");

            migrationBuilder.Sql(
                @"CREATE POLICY time_entries_update_policy ON time_entries
                FOR UPDATE
                USING (
                    UserId = current_setting('app.current_user_id', true)::uuid
                )
                WITH CHECK (
                    UserId = current_setting('app.current_user_id', true)::uuid
                );");

            migrationBuilder.Sql(
                @"CREATE POLICY time_entries_delete_policy ON time_entries
                FOR DELETE
                USING (
                    UserId = current_setting('app.current_user_id', true)::uuid
                );");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop RLS policies
            migrationBuilder.Sql(@"DROP POLICY IF EXISTS time_entries_select_policy ON time_entries;");
            migrationBuilder.Sql(@"DROP POLICY IF EXISTS time_entries_insert_policy ON time_entries;");
            migrationBuilder.Sql(@"DROP POLICY IF EXISTS time_entries_update_policy ON time_entries;");
            migrationBuilder.Sql(@"DROP POLICY IF EXISTS time_entries_delete_policy ON time_entries;");

            // Drop RLS function
            migrationBuilder.Sql(@"DROP FUNCTION IF EXISTS time_entries_rls_policy() CASCADE;");

            // Disable RLS
            migrationBuilder.Sql(@"ALTER TABLE time_entries DISABLE ROW LEVEL SECURITY;");

            migrationBuilder.DropTable(
                name: "time_entries");

            migrationBuilder.DropTable(
                name: "time_rates");
        }
    }
}
