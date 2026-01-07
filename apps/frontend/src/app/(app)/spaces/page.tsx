"use client"

import * as React from "react"
import { useQuery } from "@tanstack/react-query"
import { useRouter } from "next/navigation"
import { spacesApi } from "@/features/spaces/api"
import { buildSpaceTree } from "@/features/spaces/utils"
import { SpaceTreeNav } from "@/components/spaces/space-tree-nav"
import type { SpaceTreeNode } from "@/features/spaces/types"
import { cn } from "@/lib/utils"

export default function SpacesPage() {
  const router = useRouter()

  // TODO: Get current workspace ID from auth context
  const currentWorkspaceId = "default-workspace"

  const { data: spaces, isLoading: spacesLoading } = useQuery({
    queryKey: ["spaces", currentWorkspaceId],
    queryFn: async () => {
      const response = await spacesApi.getSpacesByWorkspace(currentWorkspaceId)
      return response.data
    },
  })

  // Get folders for all spaces
  const { data: folders, isLoading: foldersLoading } = useQuery({
    queryKey: ["folders"],
    queryFn: async () => {
      // TODO: Implement getFoldersByWorkspace endpoint or aggregate from spaces
      const response = await fetch("/api/folders")
      return response.json()
    },
    enabled: !!spaces,
  })

  // Get tasklists for all spaces
  const { data: taskLists, isLoading: taskListsLoading } = useQuery({
    queryKey: ["tasklists"],
    queryFn: async () => {
      const response = await spacesApi.getTaskLists()
      return response.data
    },
  })

  const isLoading = spacesLoading || foldersLoading || taskListsLoading

  // Build hierarchical tree
  const tree: SpaceTreeNode[] = React.useMemo(() => {
    if (!spaces || !folders || !taskLists) return []
    return buildSpaceTree(spaces, folders, taskLists)
  }, [spaces, folders, taskLists])

  const handleNodeClick = (node: SpaceTreeNode) => {
    if (node.type === "tasklist") {
      router.push(`/lists/${node.id}` as any)
    } else if (node.type === "folder") {
      // TODO: Show folder detail view
      console.log("Folder clicked:", node)
    } else if (node.type === "space") {
      // TODO: Show space detail view
      console.log("Space clicked:", node)
    }
  }

  return (
    <div className="flex h-full">
      {/* Left sidebar with space tree */}
      <div
        className={cn(
          "w-72 border-r border-gray-200 dark:border-gray-700 bg-white dark:bg-gray-800",
          "flex flex-col"
        )}
      >
        <div className="border-b border-gray-200 dark:border-gray-700 p-4">
          <h2 className="text-lg font-semibold text-gray-900 dark:text-gray-100">
            Spaces
          </h2>
          <p className="text-sm text-gray-500 dark:text-gray-400">
            Navigate your workspace
          </p>
        </div>

        <div className="flex-1 overflow-auto p-4">
          {isLoading ? (
            <div className="flex items-center justify-center py-8">
              <div className="text-sm text-gray-500 dark:text-gray-400">
                Loading spaces...
              </div>
            </div>
          ) : tree.length === 0 ? (
            <div className="flex flex-col items-center justify-center py-8 text-center">
              <p className="text-sm text-gray-500 dark:text-gray-400 mb-4">
                No spaces yet
              </p>
              <button
                className="text-sm text-primary hover:underline"
                onClick={() => console.log("Create space")}
              >
                Create your first space
              </button>
            </div>
          ) : (
            <SpaceTreeNav
              spaces={tree}
              onNodeClick={handleNodeClick}
              onCreateSpace={() => console.log("Create space")}
              onCreateFolder={(spaceId) =>
                console.log("Create folder in space:", spaceId)
              }
              onCreateList={(spaceId, folderId) =>
                console.log("Create list in space:", spaceId, "folder:", folderId)
              }
            />
          )}
        </div>
      </div>

      {/* Main content area */}
      <div className="flex-1 overflow-auto bg-gray-50 dark:bg-gray-900">
        <div className="flex h-full items-center justify-center p-6">
          <div className="text-center max-w-md">
            <div className="mb-4">
              <div className="inline-flex items-center justify-center w-16 h-16 rounded-full bg-gray-100 dark:bg-gray-800">
                <svg
                  className="w-8 h-8 text-gray-400"
                  fill="none"
                  stroke="currentColor"
                  viewBox="0 0 24 24"
                >
                  <path
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    strokeWidth={2}
                    d="M3 7v10a2 2 0 002 2h14a2 2 0 002-2V9a2 2 0 00-2-2h-6l-2-2H5a2 2 0 00-2 2z"
                  />
                </svg>
              </div>
            </div>
            <h1 className="text-2xl font-bold text-gray-900 dark:text-gray-100 mb-2">
              Select a Space, Folder, or List
            </h1>
            <p className="text-gray-600 dark:text-gray-400">
              Navigate through your workspace hierarchy on the left to view and
              manage tasks.
            </p>
          </div>
        </div>
      </div>
    </div>
  )
}
