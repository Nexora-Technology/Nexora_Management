using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexora.Management.API.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDashboardsAndAnalytics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create dashboards table
            migrationBuilder.CreateTable(
                name: "dashboards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Layout = table.Column<string>(type: "jsonb", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    IsTemplate = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dashboards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dashboards_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_dashboards_Workspaces_WorkspaceId",
                        column: x => x.WorkspaceId,
                        principalTable: "Workspaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Create indexes
            migrationBuilder.CreateIndex(
                name: "idx_dashboards_created_by",
                table: "dashboards",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "idx_dashboards_is_template",
                table: "dashboards",
                column: "IsTemplate");

            migrationBuilder.CreateIndex(
                name: "idx_dashboards_workspace",
                table: "dashboards",
                column: "WorkspaceId");

            // Create materialized view for task statistics
            migrationBuilder.Sql(@"
                CREATE MATERIALIZED VIEW IF NOT EXISTS mv_task_stats AS
                SELECT
                    tl.Id AS ProjectId,
                    tl.Name AS ProjectName,
                    ts.Id AS StatusId,
                    ts.Name AS StatusName,
                    COUNT(t.Id) AS TaskCount,
                    COUNT(CASE WHEN t.AssigneeId IS NOT NULL THEN 1 END) AS AssignedCount,
                    CASE
                        WHEN COUNT(t.Id) > 0 THEN
                            ROUND(100.0 * COUNT(CASE WHEN ts.Name = 'complete' THEN 1 END) / COUNT(t.Id), 2)
                        ELSE 0
                    END AS CompletionPercentage
                FROM TaskLists tl
                LEFT JOIN Tasks t ON t.TaskListId = tl.Id
                LEFT JOIN TaskStatuses ts ON ts.Id = t.StatusId
                GROUP BY tl.Id, tl.Name, ts.Id, ts.Name
                WITH DATA;

                CREATE UNIQUE INDEX idx_mv_task_stats_unique ON mv_task_stats(ProjectId, StatusId);
                CREATE INDEX idx_mv_task_stats_project ON mv_task_stats(ProjectId);
                CREATE INDEX idx_mv_task_stats_status ON mv_task_stats(StatusId);
            ");

            // Create trigger function to refresh materialized view
            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION refresh_task_stats_mv()
                RETURNS TRIGGER AS $$
                BEGIN
                    REFRESH MATERIALIZED VIEW CONCURRENTLY mv_task_stats;
                    RETURN NULL;
                END;
                $$ LANGUAGE plpgsql;
            ");

            // Create triggers to refresh MV after task changes
            migrationBuilder.Sql(@"
                DROP TRIGGER IF EXISTS trigger_refresh_task_stats_insert ON Tasks;
                CREATE TRIGGER trigger_refresh_task_stats_insert
                AFTER INSERT ON Tasks
                FOR EACH STATEMENT
                EXECUTE FUNCTION refresh_task_stats_mv();
            ");

            migrationBuilder.Sql(@"
                DROP TRIGGER IF EXISTS trigger_refresh_task_stats_update ON Tasks;
                CREATE TRIGGER trigger_refresh_task_stats_update
                AFTER UPDATE OF StatusId, TaskListId ON Tasks
                FOR EACH STATEMENT
                EXECUTE FUNCTION refresh_task_stats_mv();
            ");

            migrationBuilder.Sql(@"
                DROP TRIGGER IF EXISTS trigger_refresh_task_stats_delete ON Tasks;
                CREATE TRIGGER trigger_refresh_task_stats_delete
                AFTER DELETE ON Tasks
                FOR EACH STATEMENT
                EXECUTE FUNCTION refresh_task_stats_mv();
            ");

            // Enable Row-Level Security for dashboards
            migrationBuilder.Sql(@"ALTER TABLE dashboards ENABLE ROW LEVEL SECURITY;");

            // Create RLS policy function for dashboards
            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION dashboards_rls_policy()
                RETURNS boolean AS $$
                BEGIN
                    -- Users can see dashboards in their workspaces
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

                GRANT EXECUTE ON FUNCTION dashboards_rls_policy() TO PUBLIC;
            ");

            // Create RLS policies for dashboards
            migrationBuilder.Sql(@"
                CREATE POLICY dashboards_select_policy ON dashboards
                FOR SELECT
                USING dashboards_rls_policy();
            ");

            migrationBuilder.Sql(@"
                CREATE POLICY dashboards_insert_policy ON dashboards
                FOR INSERT
                WITH CHECK (
                    EXISTS (
                        SELECT 1 FROM workspace_members
                        WHERE workspace_members.workspace_id = WorkspaceId
                        AND workspace_members.user_id = current_setting('app.current_user_id', true)::uuid
                    )
                );
            ");

            migrationBuilder.Sql(@"
                CREATE POLICY dashboards_update_policy ON dashboards
                FOR UPDATE
                USING dashboards_rls_policy()
                WITH CHECK dashboards_rls_policy();
            ");

            migrationBuilder.Sql(@"
                CREATE POLICY dashboards_delete_policy ON dashboards
                FOR DELETE
                USING (
                    CreatedBy = current_setting('app.current_user_id', true)::uuid
                    OR EXISTS (
                        SELECT 1 FROM workspace_members wm
                        JOIN Roles r ON wm.RoleId = r.Id
                        WHERE wm.workspace_id = WorkspaceId
                        AND wm.user_id = current_setting('app.current_user_id', true)::uuid
                        AND r.Name = 'Admin'
                    )
                );
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop RLS policies
            migrationBuilder.Sql(@"DROP POLICY IF EXISTS dashboards_select_policy ON dashboards;");
            migrationBuilder.Sql(@"DROP POLICY IF EXISTS dashboards_insert_policy ON dashboards;");
            migrationBuilder.Sql(@"DROP POLICY IF EXISTS dashboards_update_policy ON dashboards;");
            migrationBuilder.Sql(@"DROP POLICY IF EXISTS dashboards_delete_policy ON dashboards;");

            // Drop RLS function
            migrationBuilder.Sql(@"DROP FUNCTION IF EXISTS dashboards_rls_policy() CASCADE;");

            // Disable RLS
            migrationBuilder.Sql(@"ALTER TABLE dashboards DISABLE ROW LEVEL SECURITY;");

            // Drop triggers
            migrationBuilder.Sql(@"DROP TRIGGER IF EXISTS trigger_refresh_task_stats_insert ON Tasks;");
            migrationBuilder.Sql(@"DROP TRIGGER IF EXISTS trigger_refresh_task_stats_update ON Tasks;");
            migrationBuilder.Sql(@"DROP TRIGGER IF EXISTS trigger_refresh_task_stats_delete ON Tasks;");

            // Drop trigger function
            migrationBuilder.Sql(@"DROP FUNCTION IF EXISTS refresh_task_stats_mv() CASCADE;");

            // Drop materialized view
            migrationBuilder.Sql(@"DROP MATERIALIZED VIEW IF EXISTS mv_task_stats CASCADE;");

            // Drop indexes
            migrationBuilder.DropIndex(
                name: "idx_dashboards_workspace",
                table: "dashboards");

            migrationBuilder.DropIndex(
                name: "idx_dashboards_is_template",
                table: "dashboards");

            migrationBuilder.DropIndex(
                name: "idx_dashboards_created_by",
                table: "dashboards");

            // Drop table
            migrationBuilder.DropTable(
                name: "dashboards");
        }
    }
}
