using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexora.Management.API.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedRolesAndPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Seed system roles
            migrationBuilder.Sql(@"
                INSERT INTO ""Roles"" (""Id"", ""Name"", ""Description"", ""IsSystem"", ""CreatedAt"", ""UpdatedAt"")
                VALUES
                    (uuid_generate_v4(), 'Owner', 'Full workspace control', true, NOW(), NOW()),
                    (uuid_generate_v4(), 'Admin', 'Manage projects and members', true, NOW(), NOW()),
                    (uuid_generate_v4(), 'Member', 'Create tasks, comment', true, NOW(), NOW()),
                    (uuid_generate_v4(), 'Guest', 'View-only access', true, NOW(), NOW())
                ON CONFLICT (""Name"") DO NOTHING;
            ");

            // Seed permissions for projects
            migrationBuilder.Sql(@"
                INSERT INTO ""Permissions"" (""Id"", ""Resource"", ""Action"", ""Description"", ""CreatedAt"", ""UpdatedAt"")
                VALUES
                    (uuid_generate_v4(), 'projects', 'create', 'Create new projects', NOW(), NOW()),
                    (uuid_generate_v4(), 'projects', 'read', 'View projects', NOW(), NOW()),
                    (uuid_generate_v4(), 'projects', 'update', 'Edit projects', NOW(), NOW()),
                    (uuid_generate_v4(), 'projects', 'delete', 'Delete projects', NOW(), NOW())
                ON CONFLICT DO NOTHING;
            ");

            // Seed permissions for tasks
            migrationBuilder.Sql(@"
                INSERT INTO ""Permissions"" (""Id"", ""Resource"", ""Action"", ""Description"", ""CreatedAt"", ""UpdatedAt"")
                VALUES
                    (uuid_generate_v4(), 'tasks', 'create', 'Create new tasks', NOW(), NOW()),
                    (uuid_generate_v4(), 'tasks', 'read', 'View tasks', NOW(), NOW()),
                    (uuid_generate_v4(), 'tasks', 'update', 'Edit tasks', NOW(), NOW()),
                    (uuid_generate_v4(), 'tasks', 'delete', 'Delete tasks', NOW(), NOW()),
                    (uuid_generate_v4(), 'tasks', 'assign', 'Assign tasks to users', NOW(), NOW())
                ON CONFLICT DO NOTHING;
            ");

            // Seed permissions for comments
            migrationBuilder.Sql(@"
                INSERT INTO ""Permissions"" (""Id"", ""Resource"", ""Action"", ""Description"", ""CreatedAt"", ""UpdatedAt"")
                VALUES
                    (uuid_generate_v4(), 'comments', 'create', 'Add comments', NOW(), NOW()),
                    (uuid_generate_v4(), 'comments', 'read', 'View comments', NOW(), NOW()),
                    (uuid_generate_v4(), 'comments', 'update', 'Edit own comments', NOW(), NOW()),
                    (uuid_generate_v4(), 'comments', 'delete', 'Delete own comments', NOW(), NOW())
                ON CONFLICT DO NOTHING;
            ");

            // Seed permissions for attachments
            migrationBuilder.Sql(@"
                INSERT INTO ""Permissions"" (""Id"", ""Resource"", ""Action"", ""Description"", ""CreatedAt"", ""UpdatedAt"")
                VALUES
                    (uuid_generate_v4(), 'attachments', 'create', 'Upload attachments', NOW(), NOW()),
                    (uuid_generate_v4(), 'attachments', 'read', 'Download attachments', NOW(), NOW()),
                    (uuid_generate_v4(), 'attachments', 'delete', 'Delete attachments', NOW(), NOW())
                ON CONFLICT DO NOTHING;
            ");

            // Seed permissions for workspaces
            migrationBuilder.Sql(@"
                INSERT INTO ""Permissions"" (""Id"", ""Resource"", ""Action"", ""Description"", ""CreatedAt"", ""UpdatedAt"")
                VALUES
                    (uuid_generate_v4(), 'workspaces', 'read', 'View workspace details', NOW(), NOW()),
                    (uuid_generate_v4(), 'workspaces', 'update', 'Edit workspace settings', NOW(), NOW()),
                    (uuid_generate_v4(), 'workspaces', 'delete', 'Delete workspace', NOW(), NOW()),
                    (uuid_generate_v4(), 'workspaces', 'manage_members', 'Add/remove workspace members', NOW(), NOW())
                ON CONFLICT DO NOTHING;
            ");

            // Assign all permissions to Owner role
            migrationBuilder.Sql(@"
                INSERT INTO ""RolePermissions"" (""RoleId"", ""PermissionId"")
                SELECT r.""Id"", p.""Id""
                FROM ""Roles"" r
                CROSS JOIN ""Permissions"" p
                WHERE r.""Name"" = 'Owner'
                ON CONFLICT DO NOTHING;
            ");

            // Assign most permissions to Admin role (except delete workspace)
            migrationBuilder.Sql(@"
                INSERT INTO ""RolePermissions"" (""RoleId"", ""PermissionId"")
                SELECT r.""Id"", p.""Id""
                FROM ""Roles"" r
                CROSS JOIN ""Permissions"" p
                WHERE r.""Name"" = 'Admin'
                    AND p.""Action"" != 'delete'
                    AND (p.""Resource"" != 'workspaces' OR p.""Action"" = 'read')
                ON CONFLICT DO NOTHING;
            ");

            // Assign basic permissions to Member role
            migrationBuilder.Sql(@"
                INSERT INTO ""RolePermissions"" (""RoleId"", ""PermissionId"")
                SELECT r.""Id"", p.""Id""
                FROM ""Roles"" r
                CROSS JOIN ""Permissions"" p
                WHERE r.""Name"" = 'Member'
                    AND (
                        (p.""Resource"" = 'projects' AND p.""Action"" IN ('read'))
                        OR (p.""Resource"" = 'tasks' AND p.""Action"" IN ('create', 'read', 'update'))
                        OR (p.""Resource"" = 'comments' AND p.""Action"" IN ('create', 'read'))
                        OR (p.""Resource"" = 'attachments' AND p.""Action"" IN ('create', 'read'))
                        OR (p.""Resource"" = 'workspaces' AND p.""Action"" = 'read')
                    )
                ON CONFLICT DO NOTHING;
            ");

            // Assign read-only permissions to Guest role
            migrationBuilder.Sql(@"
                INSERT INTO ""RolePermissions"" (""RoleId"", ""PermissionId"")
                SELECT r.""Id"", p.""Id""
                FROM ""Roles"" r
                CROSS JOIN ""Permissions"" p
                WHERE r.""Name"" = 'Guest'
                    AND p.""Action"" = 'read'
                ON CONFLICT DO NOTHING;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove role-permission assignments
            migrationBuilder.Sql(@"DELETE FROM ""RolePermissions"" WHERE ""RoleId"" IN (SELECT ""Id"" FROM ""Roles"" WHERE ""IsSystem"" = true);");

            // Remove system permissions
            migrationBuilder.Sql(@"DELETE FROM ""Permissions"" WHERE ""Resource"" IN ('projects', 'tasks', 'comments', 'attachments', 'workspaces');");

            // Remove system roles
            migrationBuilder.Sql(@"DELETE FROM ""Roles"" WHERE ""IsSystem"" = true;");
        }
    }
}
