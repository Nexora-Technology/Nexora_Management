"use client";

import { formatDistanceToNow } from "date-fns";
import { FileText, Star, Clock, User } from "lucide-react";
import { Page, PageDetail } from "./types";
import { cn } from "@/lib/utils";

type PageWithDetails = Page | PageDetail;

interface PageListProps {
  pages: PageWithDetails[];
  onPageSelect: (pageId: string) => void;
  selectedPageId?: string;
  onToggleFavorite?: (pageId: string) => void;
  className?: string;
}

export function PageList({
  pages,
  onPageSelect,
  selectedPageId,
  onToggleFavorite,
  className = "",
}: PageListProps) {
  const getPageIcon = () => {
    return <FileText size={16} className="text-gray-400" />;
  };

  if (pages.length === 0) {
    return (
      <div className={cn("flex flex-col items-center justify-center py-12", className)}>
        <FileText size={48} className="text-gray-300 mb-4" />
        <p className="text-gray-500">No pages found</p>
      </div>
    );
  }

  return (
    <div className={cn("divide-y divide-gray-200", className)}>
      {pages.map((page) => (
        <div
          key={page.id}
          className={cn(
            "flex items-center gap-3 p-4 cursor-pointer hover:bg-gray-50 transition-colors group",
            selectedPageId === page.id && "bg-blue-50 hover:bg-blue-50"
          )}
          onClick={() => onPageSelect(page.id)}
        >
          {/* Icon */}
          <div className="flex-shrink-0">{getPageIcon()}</div>

          {/* Content */}
          <div className="flex-1 min-w-0">
            <div className="flex items-center gap-2">
              <h3 className="font-medium text-gray-900 truncate">{page.title}</h3>
              {page.isFavorite && (
                <Star
                  size={14}
                  className="text-yellow-500 flex-shrink-0 fill-yellow-500"
                />
              )}
            </div>

            {/* Meta info */}
            <div className="flex items-center gap-3 mt-1 text-xs text-gray-500">
              <span className="flex items-center gap-1">
                <Clock size={12} />
                {formatDistanceToNow(new Date(page.updatedAt), {
                  addSuffix: true,
                })}
              </span>
              {"updatedByName" in page && page.updatedByName && (
                <span className="flex items-center gap-1">
                  <User size={12} />
                  {page.updatedByName}
                </span>
              )}
            </div>
          </div>

          {/* Actions */}
          {onToggleFavorite && (
            <button
              onClick={(e) => {
                e.stopPropagation();
                onToggleFavorite(page.id);
              }}
              className="p-2 hover:bg-gray-200 rounded transition-colors opacity-0 group-hover:opacity-100"
              title={page.isFavorite ? "Remove from favorites" : "Add to favorites"}
            >
              <Star
                size={16}
                className={cn(
                  page.isFavorite
                    ? "text-yellow-500 fill-yellow-500"
                    : "text-gray-400"
                )}
              />
            </button>
          )}
        </div>
      ))}
    </div>
  );
}

// Favorites list component
export function FavoritePages({
  pages,
  onPageSelect,
  selectedPageId,
  className = "",
}: Omit<PageListProps, "onToggleFavorite">) {
  const favoritePages = pages.filter((p) => p.isFavorite);

  if (favoritePages.length === 0) {
    return (
      <div className={cn("text-center py-8 text-gray-500 text-sm", className)}>
        No favorite pages yet
      </div>
    );
  }

  return (
    <PageList
      pages={favoritePages}
      onPageSelect={onPageSelect}
      selectedPageId={selectedPageId}
      className={className}
    />
  );
}

// Recent pages component
export function RecentPages({
  pages,
  onPageSelect,
  selectedPageId,
  limit = 10,
  className = "",
}: Omit<PageListProps, "onToggleFavorite"> & { limit?: number }) {
  const sortedPages = [...pages]
    .sort((a, b) => new Date(b.updatedAt).getTime() - new Date(a.updatedAt).getTime())
    .slice(0, limit);

  return (
    <PageList
      pages={sortedPages}
      onPageSelect={onPageSelect}
      selectedPageId={selectedPageId}
      className={className}
    />
  );
}
