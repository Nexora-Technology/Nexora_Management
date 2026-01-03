using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexora.Management.API.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EnableRowLevelSecurity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create function to set current user context
            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION set_current_user_id(user_id UUID)
                RETURNS VOID AS $$
                BEGIN
                    PERFORM set_config('app.current_user_id', user_id::TEXT, true);
                END;
                $$ LANGUAGE plpgsql SECURITY DEFINER;
            ");

            // Enable RLS on Tasks table
            migrationBuilder.Sql("ALTER TABLE \"Tasks\" ENABLE ROW LEVEL SECURITY;");

            // Create RLS policies for Tasks
            migrationBuilder.Sql(@"
                CREATE POLICY tasks_select_policy ON ""Tasks""
                    FOR SELECT
                    USING (
                        ""ProjectId"" IN (
                            SELECT ""Id"" FROM ""Projects""
                            WHERE ""WorkspaceId"" IN (
                                SELECT ""WorkspaceId"" FROM ""WorkspaceMembers""
                                WHERE ""UserId"" = current_setting('app.current_user_id', true)::UUID
                            )
                        )
                    );
            ");

            migrationBuilder.Sql(@"
                CREATE POLICY tasks_insert_policy ON ""Tasks""
                    FOR INSERT
                    WITH CHECK (
                        ""ProjectId"" IN (
                            SELECT ""Id"" FROM ""Projects""
                            WHERE ""WorkspaceId"" IN (
                                SELECT ""WorkspaceId"" FROM ""WorkspaceMembers""
                                WHERE ""UserId"" = current_setting('app.current_user_id', true)::UUID
                            )
                        )
                    );
            ");

            migrationBuilder.Sql(@"
                CREATE POLICY tasks_update_policy ON ""Tasks""
                    FOR UPDATE
                    USING (
                        ""ProjectId"" IN (
                            SELECT ""Id"" FROM ""Projects""
                            WHERE ""WorkspaceId"" IN (
                                SELECT ""WorkspaceId"" FROM ""WorkspaceMembers""
                                WHERE ""UserId"" = current_setting('app.current_user_id', true)::UUID
                            )
                        )
                    );
            ");

            migrationBuilder.Sql(@"
                CREATE POLICY tasks_delete_policy ON ""Tasks""
                    FOR DELETE
                    USING (
                        ""ProjectId"" IN (
                            SELECT ""Id"" FROM ""Projects""
                            WHERE ""WorkspaceId"" IN (
                                SELECT ""WorkspaceId"" FROM ""WorkspaceMembers""
                                WHERE ""UserId"" = current_setting('app.current_user_id', true)::UUID
                            )
                        )
                    );
            ");

            // Enable RLS on Projects table
            migrationBuilder.Sql("ALTER TABLE \"Projects\" ENABLE ROW LEVEL SECURITY;");

            migrationBuilder.Sql(@"
                CREATE POLICY projects_select_policy ON ""Projects""
                    FOR SELECT
                    USING (
                        ""WorkspaceId"" IN (
                            SELECT ""WorkspaceId"" FROM ""WorkspaceMembers""
                            WHERE ""UserId"" = current_setting('app.current_user_id', true)::UUID
                        )
                    );
            ");

            migrationBuilder.Sql(@"
                CREATE POLICY projects_insert_policy ON ""Projects""
                    FOR INSERT
                    WITH CHECK (
                        ""WorkspaceId"" IN (
                            SELECT ""WorkspaceId"" FROM ""WorkspaceMembers""
                            WHERE ""UserId"" = current_setting('app.current_user_id', true)::UUID
                        )
                    );
            ");

            migrationBuilder.Sql(@"
                CREATE POLICY projects_update_policy ON ""Projects""
                    FOR UPDATE
                    USING (
                        ""WorkspaceId"" IN (
                            SELECT ""WorkspaceId"" FROM ""WorkspaceMembers""
                            WHERE ""UserId"" = current_setting('app.current_user_id', true)::UUID
                        )
                    );
            ");

            migrationBuilder.Sql(@"
                CREATE POLICY projects_delete_policy ON ""Projects""
                    FOR DELETE
                    USING (
                        ""WorkspaceId"" IN (
                            SELECT ""WorkspaceId"" FROM ""WorkspaceMembers""
                            WHERE ""UserId"" = current_setting('app.current_user_id', true)::UUID
                        )
                    );
            ");

            // Enable RLS on Comments table
            migrationBuilder.Sql("ALTER TABLE \"Comments\" ENABLE ROW LEVEL SECURITY;");

            migrationBuilder.Sql(@"
                CREATE POLICY comments_select_policy ON ""Comments""
                    FOR SELECT
                    USING (
                        ""TaskId"" IN (
                            SELECT ""Id"" FROM ""Tasks""
                            WHERE ""ProjectId"" IN (
                                SELECT ""Id"" FROM ""Projects""
                                WHERE ""WorkspaceId"" IN (
                                    SELECT ""WorkspaceId"" FROM ""WorkspaceMembers""
                                    WHERE ""UserId"" = current_setting('app.current_user_id', true)::UUID
                                )
                            )
                        )
                    );
            ");

            migrationBuilder.Sql(@"
                CREATE POLICY comments_insert_policy ON ""Comments""
                    FOR INSERT
                    WITH CHECK (
                        ""TaskId"" IN (
                            SELECT ""Id"" FROM ""Tasks""
                            WHERE ""ProjectId"" IN (
                                SELECT ""Id"" FROM ""Projects""
                                WHERE ""WorkspaceId"" IN (
                                    SELECT ""WorkspaceId"" FROM ""WorkspaceMembers""
                                    WHERE ""UserId"" = current_setting('app.current_user_id', true)::UUID
                                )
                            )
                        )
                    );
            ");

            migrationBuilder.Sql(@"
                CREATE POLICY comments_update_policy ON ""Comments""
                    FOR UPDATE
                    USING (
                        ""UserId"" = current_setting('app.current_user_id', true)::UUID
                    );
            ");

            migrationBuilder.Sql(@"
                CREATE POLICY comments_delete_policy ON ""Comments""
                    FOR DELETE
                    USING (
                        ""UserId"" = current_setting('app.current_user_id', true)::UUID
                    );
            ");

            // Enable RLS on Attachments table
            migrationBuilder.Sql("ALTER TABLE \"Attachments\" ENABLE ROW LEVEL SECURITY;");

            migrationBuilder.Sql(@"
                CREATE POLICY attachments_select_policy ON ""Attachments""
                    FOR SELECT
                    USING (
                        ""TaskId"" IN (
                            SELECT ""Id"" FROM ""Tasks""
                            WHERE ""ProjectId"" IN (
                                SELECT ""Id"" FROM ""Projects""
                                WHERE ""WorkspaceId"" IN (
                                    SELECT ""WorkspaceId"" FROM ""WorkspaceMembers""
                                    WHERE ""UserId"" = current_setting('app.current_user_id', true)::UUID
                                )
                            )
                        )
                    );
            ");

            migrationBuilder.Sql(@"
                CREATE POLICY attachments_insert_policy ON ""Attachments""
                    FOR INSERT
                    WITH CHECK (
                        ""TaskId"" IN (
                            SELECT ""Id"" FROM ""Tasks""
                            WHERE ""ProjectId"" IN (
                                SELECT ""Id"" FROM ""Projects""
                                WHERE ""WorkspaceId"" IN (
                                    SELECT ""WorkspaceId"" FROM ""WorkspaceMembers""
                                    WHERE ""UserId"" = current_setting('app.current_user_id', true)::UUID
                                )
                            )
                        )
                    );
            ");

            migrationBuilder.Sql(@"
                CREATE POLICY attachments_delete_policy ON ""Attachments""
                    FOR DELETE
                    USING (
                        ""UserId"" = current_setting('app.current_user_id', true)::UUID
                    );
            ");

            // Enable RLS on ActivityLog table
            migrationBuilder.Sql("ALTER TABLE \"ActivityLog\" ENABLE ROW LEVEL SECURITY;");

            migrationBuilder.Sql(@"
                CREATE POLICY activity_log_select_policy ON ""ActivityLog""
                    FOR SELECT
                    USING (
                        ""WorkspaceId"" IS NULL OR
                        ""WorkspaceId"" IN (
                            SELECT ""WorkspaceId"" FROM ""WorkspaceMembers""
                            WHERE ""UserId"" = current_setting('app.current_user_id', true)::UUID
                        )
                    );
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop policies from ActivityLog
            migrationBuilder.Sql("DROP POLICY IF EXISTS activity_log_select_policy ON \"ActivityLog\";");
            migrationBuilder.Sql("ALTER TABLE \"ActivityLog\" DISABLE ROW LEVEL SECURITY;");

            // Drop policies from Attachments
            migrationBuilder.Sql("DROP POLICY IF EXISTS attachments_delete_policy ON \"Attachments\";");
            migrationBuilder.Sql("DROP POLICY IF EXISTS attachments_insert_policy ON \"Attachments\";");
            migrationBuilder.Sql("DROP POLICY IF EXISTS attachments_select_policy ON \"Attachments\";");
            migrationBuilder.Sql("ALTER TABLE \"Attachments\" DISABLE ROW LEVEL SECURITY;");

            // Drop policies from Comments
            migrationBuilder.Sql("DROP POLICY IF EXISTS comments_delete_policy ON \"Comments\";");
            migrationBuilder.Sql("DROP POLICY IF EXISTS comments_update_policy ON \"Comments\";");
            migrationBuilder.Sql("DROP POLICY IF EXISTS comments_insert_policy ON \"Comments\";");
            migrationBuilder.Sql("DROP POLICY IF EXISTS comments_select_policy ON \"Comments\";");
            migrationBuilder.Sql("ALTER TABLE \"Comments\" DISABLE ROW LEVEL SECURITY;");

            // Drop policies from Projects
            migrationBuilder.Sql("DROP POLICY IF EXISTS projects_delete_policy ON \"Projects\";");
            migrationBuilder.Sql("DROP POLICY IF EXISTS projects_update_policy ON \"Projects\";");
            migrationBuilder.Sql("DROP POLICY IF EXISTS projects_insert_policy ON \"Projects\";");
            migrationBuilder.Sql("DROP POLICY IF EXISTS projects_select_policy ON \"Projects\";");
            migrationBuilder.Sql("ALTER TABLE \"Projects\" DISABLE ROW LEVEL SECURITY;");

            // Drop policies from Tasks
            migrationBuilder.Sql("DROP POLICY IF EXISTS tasks_delete_policy ON \"Tasks\";");
            migrationBuilder.Sql("DROP POLICY IF EXISTS tasks_update_policy ON \"Tasks\";");
            migrationBuilder.Sql("DROP POLICY IF EXISTS tasks_insert_policy ON \"Tasks\";");
            migrationBuilder.Sql("DROP POLICY IF EXISTS tasks_select_policy ON \"Tasks\";");
            migrationBuilder.Sql("ALTER TABLE \"Tasks\" DISABLE ROW LEVEL SECURITY;");

            // Drop the function
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS set_current_user_id(UUID);");
        }
    }
}
