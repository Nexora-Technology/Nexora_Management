"use client";

import { useState } from "react";
import {
  ChevronRight,
  ChevronDown,
  FileText,
  Star,
  Folder,
  Plus,
  Search,
} from "lucide-react";
import { PageTreeNode } from "./types";
import { cn } from "@/lib/utils";

interface PageTreeProps {
  pages: PageTreeNode[];
  onPageSelect: (pageId: string) => void;
  selectedPageId?: string;
  onCreatePage?: (parentId?: string) => void;
  onToggleFavorite?: (pageId: string) => void;
  className?: string;
}

interface PageNodeProps {
  node: PageTreeNode;
  level: number;
  selectedPageId?: string;
  onPageSelect: (pageId: string) => void;
  onCreatePage?: (parentId?: string) => void;
  onToggleFavorite?: (pageId: string) => void;
}

function PageNode({
  node,
  level,
  selectedPageId,
  onPageSelect,
  onCreatePage,
  onToggleFavorite,
}: PageNodeProps) {
  const [isExpanded, setIsExpanded] = useState(true);
  const hasChildren = node.children && node.children.length > 0;

  const getPageIcon = () => {
    if (node.icon) {
      return <span className="text-sm">{node.icon}</span>;
    }

    if (node.contentType === "folder") {
      return <Folder size={16} className="text-blue-500" />;
    }

    return <FileText size={16} className="text-gray-400" />;
  };

  return (
    <div>
      <div
        className={cn(
          "flex items-center gap-1 px-2 py-1.5 rounded-md cursor-pointer hover:bg-gray-100 transition-colors",
          selectedPageId === node.id && "bg-blue-100 hover:bg-blue-100"
        )}
        style={{ paddingLeft: `${level * 16 + 8}px` }}
        onClick={() => onPageSelect(node.id)}
      >
        {/* Expand/Collapse */}
        <div
          className="w-4 h-4 flex items-center justify-center flex-shrink-0"
          onClick={(e) => {
            e.stopPropagation();
            if (hasChildren) {
              setIsExpanded(!isExpanded);
            }
          }}
        >
          {hasChildren &&
            (isExpanded ? (
              <ChevronDown size={14} className="text-gray-400" />
            ) : (
              <ChevronRight size={14} className="text-gray-400" />
            ))}
        </div>

        {/* Page Icon */}
        <div className="flex-shrink-0">{getPageIcon()}</div>

        {/* Title */}
        <span className="flex-1 text-sm truncate">{node.title}</span>

        {/* Favorite */}
        {node.isFavorite && (
          <Star size={14} className="text-yellow-500 flex-shrink-0 fill-yellow-500" />
        )}

        {/* Actions (on hover) */}
        <div
          className="hidden group-hover:flex gap-1 ml-2"
          onClick={(e) => e.stopPropagation()}
        >
          {onCreatePage && (
            <button
              onClick={() => onCreatePage(node.id)}
              className="p-1 hover:bg-gray-200 rounded"
              title="Add subpage"
            >
              <Plus size={14} />
            </button>
          )}
          {onToggleFavorite && !node.isFavorite && (
            <button
              onClick={() => onToggleFavorite(node.id)}
              className="p-1 hover:bg-gray-200 rounded"
              title="Add to favorites"
            >
              <Star size={14} className="text-gray-400" />
            </button>
          )}
        </div>
      </div>

      {/* Children */}
      {hasChildren && isExpanded && (
        <div>
          {node.children.map((child) => (
            <PageNode
              key={child.id}
              node={child}
              level={level + 1}
              selectedPageId={selectedPageId}
              onPageSelect={onPageSelect}
              onCreatePage={onCreatePage}
              onToggleFavorite={onToggleFavorite}
            />
          ))}
        </div>
      )}
    </div>
  );
}

export function PageTree({
  pages,
  onPageSelect,
  selectedPageId,
  onCreatePage,
  onToggleFavorite,
  className = "",
}: PageTreeProps) {
  const [searchTerm, setSearchTerm] = useState("");

  // Filter pages based on search term
  const filterPages = (nodes: PageTreeNode[]): PageTreeNode[] => {
    if (!searchTerm) return nodes;

    return nodes.reduce((acc: PageTreeNode[], node) => {
      const matchesSearch = node.title
        .toLowerCase()
        .includes(searchTerm.toLowerCase());
      const filteredChildren = filterPages(node.children);

      if (matchesSearch || filteredChildren.length > 0) {
        acc.push({
          ...node,
          children: filteredChildren,
        });
      }

      return acc;
    }, []);
  };

  const filteredPages = filterPages(pages);

  return (
    <div className={cn("flex flex-col h-full", className)}>
      {/* Header */}
      <div className="p-4 border-b border-gray-200">
        <div className="flex items-center justify-between mb-3">
          <h2 className="font-semibold text-lg">Pages</h2>
          {onCreatePage && (
            <button
              onClick={() => onCreatePage()}
              className="p-1 hover:bg-gray-100 rounded transition-colors"
              title="New page"
            >
              <Plus size={18} />
            </button>
          )}
        </div>

        {/* Search */}
        <div className="relative">
          <Search
            size={16}
            className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400"
          />
          <input
            type="text"
            placeholder="Search pages..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="w-full pl-9 pr-3 py-2 border border-gray-300 rounded-md text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
          />
        </div>
      </div>

      {/* Page Tree */}
      <div className="flex-1 overflow-y-auto p-2">
        {filteredPages.length === 0 ? (
          <div className="text-center py-8 text-gray-500 text-sm">
            {searchTerm ? "No pages found" : "No pages yet"}
          </div>
        ) : (
          filteredPages.map((page) => (
            <PageNode
              key={page.id}
              node={page}
              level={0}
              selectedPageId={selectedPageId}
              onPageSelect={onPageSelect}
              onCreatePage={onCreatePage}
              onToggleFavorite={onToggleFavorite}
            />
          ))
        )}
      </div>
    </div>
  );
}
