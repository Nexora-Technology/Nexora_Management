"use client"

import * as React from "react"
import { useState, useCallback } from "react"
import { ChevronRight, Folder, List, Plus, Circle } from "lucide-react"
import { cn } from "@/lib/utils"
import type { SpaceTreeNode } from "@/features/spaces/types"

interface SpaceTreeNavProps {
  spaces: SpaceTreeNode[]
  onNodeClick?: (node: SpaceTreeNode) => void
  onCreateSpace?: () => void
  onCreateFolder?: (spaceId: string) => void
  onCreateList?: (spaceId: string, folderId?: string) => void
  collapsed?: boolean
  className?: string
  selectedNodeId?: string
}

export function SpaceTreeNav({
  spaces,
  onNodeClick,
  onCreateSpace,
  onCreateFolder,
  onCreateList,
  collapsed = false,
  className,
  selectedNodeId,
}: SpaceTreeNavProps) {
  // Track expanded nodes using a Set for O(1) lookups
  const [expandedNodes, setExpandedNodes] = useState<Set<string>>(new Set())

  const toggleNode = useCallback((nodeId: string) => {
    setExpandedNodes((prev) => {
      const next = new Set(prev)
      if (next.has(nodeId)) {
        next.delete(nodeId)
      } else {
        next.add(nodeId)
      }
      return next
    })
  }, [])

  const handleNodeClick = useCallback((node: SpaceTreeNode, event: React.MouseEvent) => {
    event.stopPropagation()
    const hasChildren = node.children && node.children.length > 0
    if (hasChildren) {
      toggleNode(node.id)
    }
    onNodeClick?.(node)
  }, [toggleNode, onNodeClick])

  const renderNode = useCallback((node: SpaceTreeNode, level: number = 0): React.ReactNode => {
    const isExpanded = expandedNodes.has(node.id)
    const hasChildren = node.children && node.children.length > 0

    // Node icon based on type
    const NodeIcon = node.type === 'space'
      ? Circle
      : node.type === 'folder'
      ? Folder
      : List

    // Icon color based on node type and custom color property
    const defaultIconColor = node.type === 'space' ? '#a855f7' : '#6b7280' // purple-500 or gray-500
    const iconColor = node.color || defaultIconColor

    return (
      <div key={node.id} className="w-full">
        {/* Node Row */}
        <div
          className={cn(
            "flex items-center gap-2 py-1.5 px-2 rounded-md hover:bg-gray-100 dark:hover:bg-gray-800 cursor-pointer text-sm transition-colors group",
            collapsed && "justify-center px-1"
          )}
          style={{
            paddingLeft: collapsed ? 0 : `${level * 16 + 8}px`,
          }}
          onClick={(e) => handleNodeClick(node, e)}
          role="treeitem"
          aria-expanded={isExpanded}
          aria-selected={selectedNodeId === node.id}
          data-node-type={node.type}
          data-node-id={node.id}
        >
          {/* Expand/Collapse Chevron */}
          {hasChildren && !collapsed && (
            <ChevronRight
              className={cn(
                "h-4 w-4 text-gray-400 transition-transform duration-200 flex-shrink-0",
                isExpanded && "transform rotate-90"
              )}
            />
          )}

          {/* Node Icon */}
          {!collapsed && (
            <NodeIcon
              className="h-4 w-4 flex-shrink-0"
              style={{ color: iconColor }}
            />
          )}

          {/* Node Name */}
          {!collapsed && (
            <span className="flex-1 truncate text-gray-900 dark:text-gray-100 font-medium">
              {node.name}
            </span>
          )}

          {/* Action Buttons (visible on hover for non-collapsed mode) */}
          {!collapsed && (
            <div className="opacity-0 group-hover:opacity-100 flex gap-1 transition-opacity">
              {node.type === 'space' && onCreateFolder && (
                <button
                  onClick={(e) => {
                    e.stopPropagation()
                    onCreateFolder(node.id)
                  }}
                  className="p-1 hover:bg-gray-200 dark:hover:bg-gray-700 rounded"
                  title="New Folder"
                >
                  <Folder className="h-3 w-3" />
                </button>
              )}
              {(node.type === 'space' || node.type === 'folder') && onCreateList && (
                <button
                  onClick={(e) => {
                    e.stopPropagation()
                    onCreateList(
                      node.type === 'space' ? node.id : node.spaceId!,
                      node.type === 'folder' ? node.id : undefined
                    )
                  }}
                  className="p-1 hover:bg-gray-200 dark:hover:bg-gray-700 rounded"
                  title="New List"
                >
                  <Plus className="h-3 w-3" />
                </button>
              )}
            </div>
          )}
        </div>

        {/* Children */}
        {isExpanded && hasChildren && !collapsed && (
          <div className="mt-0.5" role="group">
            {node.children!.map((child) => renderNode(child, level + 1))}
          </div>
        )}
      </div>
    )
  }, [expandedNodes, handleNodeClick, collapsed, onCreateFolder, onCreateList])

  return (
    <nav
      className={cn("space-y-1", className)}
      role="tree"
      aria-label="Space hierarchy"
    >
      {/* Render all space nodes */}
      {spaces.map((space) => renderNode(space))}

      {/* Create Space Button */}
      {!collapsed && onCreateSpace && (
        <button
          onClick={onCreateSpace}
          className="w-full flex items-center gap-2 py-1.5 px-2 text-sm text-gray-500 hover:text-gray-700 dark:text-gray-400 dark:hover:text-gray-200 hover:bg-gray-100 dark:hover:bg-gray-800 rounded-md transition-colors mt-2"
        >
          <Plus className="h-4 w-4" />
          <span className="font-medium">New Space</span>
        </button>
      )}
    </nav>
  )
}

// Export memoized version for performance
export const MemoizedSpaceTreeNav = React.memo(SpaceTreeNav, (prevProps, nextProps) => {
  return (
    prevProps.spaces === nextProps.spaces &&
    prevProps.collapsed === nextProps.collapsed &&
    prevProps.className === nextProps.className &&
    prevProps.selectedNodeId === nextProps.selectedNodeId &&
    prevProps.onCreateSpace === nextProps.onCreateSpace &&
    prevProps.onCreateFolder === nextProps.onCreateFolder &&
    prevProps.onCreateList === nextProps.onCreateList &&
    prevProps.onNodeClick === nextProps.onNodeClick
  )
})
