'use client';

import * as React from 'react';
import { useQuery } from '@tanstack/react-query';
import { FolderTree } from 'lucide-react';
import { useWorkspace } from '@/features/workspaces';
import { cn } from '@/lib/utils';

export default function FoldersPage() {
  const { currentWorkspace, isLoading: workspaceLoading } = useWorkspace();

  const { data: folders, isLoading: foldersLoading } = useQuery({
    queryKey: ['folders', currentWorkspace?.id],
    queryFn: async () => {
      if (!currentWorkspace) return [];
      const response = await fetch(`/api/workspaces/${currentWorkspace.id}/folders`);
      if (!response.ok) return [];
      return response.json();
    },
    enabled: !!currentWorkspace,
  });

  const isLoading = workspaceLoading || foldersLoading;

  // Show no workspace state
  if (!currentWorkspace) {
    return (
      <div className="flex h-full items-center justify-center bg-gray-50 dark:bg-gray-900">
        <div className="text-center max-w-md">
          <div className="mb-4">
            <FolderTree className="h-16 w-16 text-gray-300 dark:text-gray-600 mx-auto" />
          </div>
          <h2 className="text-2xl font-bold text-gray-900 dark:text-gray-100 mb-2">
            No Workspace Selected
          </h2>
          <p className="text-gray-600 dark:text-gray-400 mb-6">
            Select a workspace to view and manage folders.
          </p>
        </div>
      </div>
    );
  }

  return (
    <div className="container mx-auto p-6">
      <div className="mb-6">
        <h1 className="text-3xl font-bold text-gray-900 dark:text-gray-100">Folders</h1>
        <p className="text-gray-600 dark:text-gray-400 mt-1">
          Organize your tasks and lists into folders
        </p>
      </div>

      {isLoading ? (
        <div className="flex items-center justify-center py-12">
          <div className="text-sm text-gray-500 dark:text-gray-400">Loading folders...</div>
        </div>
      ) : !folders || folders.length === 0 ? (
        <div className="flex flex-col items-center justify-center py-12 text-center">
          <div className="mb-4">
            <FolderTree className="h-16 w-16 text-gray-300 dark:text-gray-600" />
          </div>
          <h3 className="text-lg font-semibold text-gray-900 dark:text-gray-100 mb-2">
            No folders yet
          </h3>
          <p className="text-gray-600 dark:text-gray-400 max-w-md">
            Create folders to organize your tasks and lists within your workspace.
          </p>
        </div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          {folders.map((folder: { id: string; name: string; description?: string }) => (
            <div
              key={folder.id}
              className={cn(
                'p-4 rounded-lg border border-gray-200 dark:border-gray-700',
                'bg-white dark:bg-gray-800 hover:shadow-md transition-shadow'
              )}
            >
              <div className="flex items-center gap-3 mb-2">
                <FolderTree className="h-5 w-5 text-primary" />
                <h3 className="font-semibold text-gray-900 dark:text-gray-100">{folder.name}</h3>
              </div>
              {folder.description && (
                <p className="text-sm text-gray-600 dark:text-gray-400">{folder.description}</p>
              )}
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
