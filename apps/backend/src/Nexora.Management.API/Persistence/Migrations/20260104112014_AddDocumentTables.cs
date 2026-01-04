using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexora.Management.API.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Pages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    ParentPageId = table.Column<Guid>(type: "uuid", nullable: true),
                    Title = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Slug = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Icon = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CoverImage = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Content = table.Column<JsonDocument>(type: "jsonb", nullable: false, defaultValueSql: "'{}'::jsonb"),
                    ContentType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "rich-text"),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "active"),
                    IsFavorite = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    PositionOrder = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pages_Pages_ParentPageId",
                        column: x => x.ParentPageId,
                        principalTable: "Pages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pages_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pages_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pages_Workspaces_WorkspaceId",
                        column: x => x.WorkspaceId,
                        principalTable: "Workspaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PageCollaborators",
                columns: table => new
                {
                    PageId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    Role = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "viewer"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageCollaborators", x => new { x.PageId, x.UserId });
                    table.ForeignKey(
                        name: "FK_PageCollaborators_Pages_PageId",
                        column: x => x.PageId,
                        principalTable: "Pages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PageCollaborators_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PageComments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    PageId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    Content = table.Column<string>(type: "text", nullable: false),
                    Selection = table.Column<JsonDocument>(type: "jsonb", nullable: true),
                    ParentCommentId = table.Column<Guid>(type: "uuid", nullable: true, defaultValueSql: "uuid_generate_v4()"),
                    ResolvedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PageComments_PageComments_ParentCommentId",
                        column: x => x.ParentCommentId,
                        principalTable: "PageComments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PageComments_Pages_PageId",
                        column: x => x.PageId,
                        principalTable: "Pages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PageComments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PageVersions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    PageId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    VersionNumber = table.Column<int>(type: "integer", nullable: false),
                    Content = table.Column<JsonDocument>(type: "jsonb", nullable: false, defaultValueSql: "'{}'::jsonb"),
                    CommitMessage = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PageVersions_Pages_PageId",
                        column: x => x.PageId,
                        principalTable: "Pages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PageVersions_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "idx_page_collaborators_page",
                table: "PageCollaborators",
                column: "PageId");

            migrationBuilder.CreateIndex(
                name: "idx_page_collaborators_user",
                table: "PageCollaborators",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "idx_page_comments_page",
                table: "PageComments",
                column: "PageId");

            migrationBuilder.CreateIndex(
                name: "idx_page_comments_page_parent",
                table: "PageComments",
                columns: new[] { "PageId", "ParentCommentId" });

            migrationBuilder.CreateIndex(
                name: "idx_page_comments_parent",
                table: "PageComments",
                column: "ParentCommentId",
                filter: "parent_comment_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "idx_page_comments_resolved",
                table: "PageComments",
                column: "ResolvedAt",
                filter: "resolved_at IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "idx_page_comments_user",
                table: "PageComments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "idx_pages_content",
                table: "Pages",
                column: "Content")
                .Annotation("Npgsql:IndexMethod", "gin");

            migrationBuilder.CreateIndex(
                name: "idx_pages_parent",
                table: "Pages",
                column: "ParentPageId",
                filter: "parent_page_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "idx_pages_slug",
                table: "Pages",
                column: "Slug");

            migrationBuilder.CreateIndex(
                name: "idx_pages_status",
                table: "Pages",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "idx_pages_workspace",
                table: "Pages",
                column: "WorkspaceId");

            migrationBuilder.CreateIndex(
                name: "idx_pages_workspace_parent",
                table: "Pages",
                columns: new[] { "WorkspaceId", "ParentPageId" });

            migrationBuilder.CreateIndex(
                name: "idx_pages_workspace_status_favorite",
                table: "Pages",
                columns: new[] { "WorkspaceId", "Status", "IsFavorite" });

            migrationBuilder.CreateIndex(
                name: "IX_Pages_CreatedBy",
                table: "Pages",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Pages_UpdatedBy",
                table: "Pages",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "uq_pages_workspace_slug",
                table: "Pages",
                columns: new[] { "WorkspaceId", "Slug" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_page_versions_created_at",
                table: "PageVersions",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "idx_page_versions_page",
                table: "PageVersions",
                column: "PageId");

            migrationBuilder.CreateIndex(
                name: "IX_PageVersions_CreatedBy",
                table: "PageVersions",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "uq_page_versions_page_version",
                table: "PageVersions",
                columns: new[] { "PageId", "VersionNumber" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PageCollaborators");

            migrationBuilder.DropTable(
                name: "PageComments");

            migrationBuilder.DropTable(
                name: "PageVersions");

            migrationBuilder.DropTable(
                name: "Pages");
        }
    }
}
