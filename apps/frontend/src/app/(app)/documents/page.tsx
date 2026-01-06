"use client";

import { useState } from "react";
import { FileText, Plus, Search, Star, Clock, FolderTree } from "lucide-react";
import { DocumentEditor, PageList, FavoritePages, RecentPages } from "@/features/documents";
import { PageDetail } from "@/features/documents/types";
import { cn } from "@/lib/utils";

// Mock data - replace with actual API calls
const mockPages: PageDetail[] = [
  {
    id: "1",
    workspaceId: "workspace-1",
    parentPageId: null,
    title: "Getting Started Guide",
    slug: "getting-started",
    icon: null,
    coverImage: null,
    contentType: "page",
    status: "published",
    isFavorite: true,
    positionOrder: 1,
    createdBy: "user-1",
    updatedBy: "user-1",
    createdByName: "Admin",
    updatedByName: "Admin",
    createdAt: new Date().toISOString(),
    updatedAt: new Date(Date.now() - 86400000).toISOString(),
    content: {},
  },
  {
    id: "2",
    workspaceId: "workspace-1",
    parentPageId: null,
    title: "Project Documentation",
    slug: "project-docs",
    icon: null,
    coverImage: null,
    contentType: "page",
    status: "published",
    isFavorite: false,
    positionOrder: 2,
    createdBy: "user-1",
    updatedBy: "user-2",
    createdByName: "Admin",
    updatedByName: "John Doe",
    createdAt: new Date(Date.now() - 172800000).toISOString(),
    updatedAt: new Date(Date.now() - 43200000).toISOString(),
    content: {},
  },
  {
    id: "3",
    workspaceId: "workspace-1",
    parentPageId: null,
    title: "Team Guidelines",
    slug: "team-guidelines",
    icon: null,
    coverImage: null,
    contentType: "page",
    status: "published",
    isFavorite: true,
    positionOrder: 3,
    createdBy: "user-2",
    updatedBy: "user-2",
    createdByName: "John Doe",
    updatedByName: "John Doe",
    createdAt: new Date(Date.now() - 259200000).toISOString(),
    updatedAt: new Date(Date.now() - 3600000).toISOString(),
    content: {},
  },
];

type ViewMode = "all" | "favorites" | "recent";

export default function DocumentsPage() {
  const [selectedPageId, setSelectedPageId] = useState<string | undefined>();
  const [viewMode, setViewMode] = useState<ViewMode>("all");
  const [searchQuery, setSearchQuery] = useState("");

  const selectedPage = mockPages.find((p) => p.id === selectedPageId);

  const handleToggleFavorite = (pageId: string) => {
    // TODO: Implement API call to toggle favorite
    console.log("Toggle favorite:", pageId);
  };

  const getFilteredPages = () => {
    let filtered = mockPages;

    if (searchQuery) {
      filtered = filtered.filter((p) =>
        p.title.toLowerCase().includes(searchQuery.toLowerCase())
      );
    }

    return filtered;
  };

  return (
    <div className="h-full flex">
      {/* Sidebar - Page List */}
      <div className="w-80 border-r border-gray-200 bg-white flex flex-col">
        {/* Header */}
        <div className="p-4 border-b border-gray-200">
          <div className="flex items-center justify-between mb-4">
            <h1 className="text-xl font-bold text-gray-900 flex items-center gap-2">
              <FileText className="w-5 h-5" />
              Documents
            </h1>
            <button
              className="p-2 hover:bg-gray-100 rounded-lg transition-colors"
              title="New Page"
            >
              <Plus className="w-5 h-5 text-gray-600" />
            </button>
          </div>

          {/* Search */}
          <div className="relative">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400" />
            <input
              type="text"
              placeholder="Search pages..."
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            />
          </div>
        </div>

        {/* View Mode Tabs */}
        <div className="flex border-b border-gray-200">
          <button
            onClick={() => setViewMode("all")}
            className={cn(
              "flex-1 flex items-center justify-center gap-2 py-3 text-sm font-medium transition-colors",
              viewMode === "all"
                ? "text-blue-600 border-b-2 border-blue-600"
                : "text-gray-600 hover:text-gray-900"
            )}
          >
            <FolderTree className="w-4 h-4" />
            All Pages
          </button>
          <button
            onClick={() => setViewMode("favorites")}
            className={cn(
              "flex-1 flex items-center justify-center gap-2 py-3 text-sm font-medium transition-colors",
              viewMode === "favorites"
                ? "text-blue-600 border-b-2 border-blue-600"
                : "text-gray-600 hover:text-gray-900"
            )}
          >
            <Star className="w-4 h-4" />
            Favorites
          </button>
          <button
            onClick={() => setViewMode("recent")}
            className={cn(
              "flex-1 flex items-center justify-center gap-2 py-3 text-sm font-medium transition-colors",
              viewMode === "recent"
                ? "text-blue-600 border-b-2 border-blue-600"
                : "text-gray-600 hover:text-gray-900"
            )}
          >
            <Clock className="w-4 h-4" />
            Recent
          </button>
        </div>

        {/* Page List */}
        <div className="flex-1 overflow-y-auto">
          {viewMode === "all" && (
            <PageList
              pages={getFilteredPages()}
              onPageSelect={setSelectedPageId}
              selectedPageId={selectedPageId}
              onToggleFavorite={handleToggleFavorite}
            />
          )}
          {viewMode === "favorites" && (
            <FavoritePages
              pages={getFilteredPages()}
              onPageSelect={setSelectedPageId}
              selectedPageId={selectedPageId}
            />
          )}
          {viewMode === "recent" && (
            <RecentPages
              pages={getFilteredPages()}
              onPageSelect={setSelectedPageId}
              selectedPageId={selectedPageId}
            />
          )}
        </div>
      </div>

      {/* Main Content - Editor */}
      <div className="flex-1 bg-gray-50 flex flex-col">
        {selectedPage ? (
          <>
            {/* Page Header */}
            <div className="bg-white border-b border-gray-200 p-6">
              <input
                type="text"
                value={selectedPage.title}
                className="text-2xl font-bold text-gray-900 w-full focus:outline-none"
                readOnly
              />
              <div className="flex items-center gap-4 mt-2 text-sm text-gray-500">
                <span>Last edited {new Date(selectedPage.updatedAt).toLocaleDateString()}</span>
                <span>by {selectedPage.updatedByName}</span>
              </div>
            </div>

            {/* Editor */}
            <div className="flex-1 overflow-y-auto p-6">
              <DocumentEditor
                content={selectedPage.content}
                onUpdate={(content) => console.log("Update:", content)}
                editable={false}
                placeholder="Start writing your document..."
              />
            </div>
          </>
        ) : (
          // Empty State
          <div className="flex-1 flex items-center justify-center">
            <div className="text-center">
              <FileText className="w-16 h-16 text-gray-300 mx-auto mb-4" />
              <h3 className="text-lg font-medium text-gray-900 mb-2">
                No page selected
              </h3>
              <p className="text-gray-500 mb-4">
                Select a page from the sidebar or create a new one
              </p>
              <button className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors">
                <Plus className="w-4 h-4 inline mr-2" />
                New Page
              </button>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}
