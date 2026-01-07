'use client';

import * as React from 'react';
import { Building2, ChevronDown, Plus } from 'lucide-react';
import { useWorkspace } from '@/features/workspaces';
import { cn } from '@/lib/utils';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';

export function WorkspaceSelector() {
  const { currentWorkspace, workspaces, setCurrentWorkspace, createWorkspace, isLoading } =
    useWorkspace();

  const [isCreateDialogOpen, setIsCreateDialogOpen] = React.useState(false);
  const [newWorkspaceName, setNewWorkspaceName] = React.useState('');
  const [newWorkspaceDescription, setNewWorkspaceDescription] = React.useState('');
  const [isCreating, setIsCreating] = React.useState(false);

  const handleCreateWorkspace = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!newWorkspaceName.trim()) return;

    setIsCreating(true);
    try {
      await createWorkspace({
        name: newWorkspaceName,
        description: newWorkspaceDescription || undefined,
      });
      setIsCreateDialogOpen(false);
      setNewWorkspaceName('');
      setNewWorkspaceDescription('');
    } catch (error) {
      console.error('Failed to create workspace:', error);
    } finally {
      setIsCreating(false);
    }
  };

  if (isLoading) {
    return (
      <div className="flex items-center gap-2 px-3 py-1.5 rounded-md bg-gray-100 dark:bg-gray-800 animate-pulse">
        <div className="h-4 w-4 bg-gray-300 dark:bg-gray-700 rounded" />
        <div className="h-4 w-24 bg-gray-300 dark:bg-gray-700 rounded" />
      </div>
    );
  }

  if (!currentWorkspace) {
    return (
      <Dialog open={isCreateDialogOpen} onOpenChange={setIsCreateDialogOpen}>
        <DialogTrigger asChild>
          <Button variant="outline" size="sm">
            <Plus className="h-4 w-4 mr-2" />
            Create Workspace
          </Button>
        </DialogTrigger>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Create Workspace</DialogTitle>
            <DialogDescription>
              Create a new workspace to organize your spaces and tasks.
            </DialogDescription>
          </DialogHeader>
          <form onSubmit={handleCreateWorkspace} className="space-y-4">
            <div>
              <label
                htmlFor="workspace-name"
                className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1"
              >
                Name <span className="text-red-500">*</span>
              </label>
              <Input
                id="workspace-name"
                type="text"
                value={newWorkspaceName}
                onChange={(e) => setNewWorkspaceName(e.target.value)}
                placeholder="My Workspace"
                required
              />
            </div>
            <div>
              <label
                htmlFor="workspace-description"
                className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1"
              >
                Description
              </label>
              <Textarea
                id="workspace-description"
                value={newWorkspaceDescription}
                onChange={(e) => setNewWorkspaceDescription(e.target.value)}
                placeholder="Optional description..."
                rows={3}
              />
            </div>
            <div className="flex justify-end gap-2">
              <Button
                type="button"
                variant="secondary"
                onClick={() => setIsCreateDialogOpen(false)}
                disabled={isCreating}
              >
                Cancel
              </Button>
              <Button type="submit" disabled={!newWorkspaceName.trim() || isCreating}>
                {isCreating ? 'Creating...' : 'Create Workspace'}
              </Button>
            </div>
          </form>
        </DialogContent>
      </Dialog>
    );
  }

  return (
    <DropdownMenu>
      <DropdownMenuTrigger asChild>
        <button
          className={cn(
            'flex items-center gap-2 px-3 py-1.5 rounded-md',
            'bg-gray-100 dark:bg-gray-800',
            'hover:bg-gray-200 dark:hover:bg-gray-700',
            'transition-colors',
            'focus:outline-none focus:ring-2 focus:ring-primary'
          )}
        >
          <Building2 className="h-4 w-4 text-primary" />
          <span className="text-sm font-medium text-gray-900 dark:text-gray-100 truncate max-w-[150px]">
            {currentWorkspace.name}
          </span>
          <ChevronDown className="h-4 w-4 text-gray-500" />
        </button>
      </DropdownMenuTrigger>
      <DropdownMenuContent align="start" className="w-56">
        <DropdownMenuLabel>Workspaces</DropdownMenuLabel>
        <DropdownMenuSeparator />
        {workspaces.map((workspace) => (
          <DropdownMenuItem
            key={workspace.id}
            onClick={() => setCurrentWorkspace(workspace)}
            className={cn(
              'cursor-pointer',
              workspace.id === currentWorkspace.id && 'bg-accent'
            )}
          >
            <Building2
              className={cn(
                'h-4 w-4 mr-2',
                workspace.id === currentWorkspace.id ? 'text-primary' : 'text-gray-400'
              )}
            />
            <span className="truncate">{workspace.name}</span>
            {workspace.isDefault && (
              <span className="ml-auto text-xs text-gray-500">Default</span>
            )}
          </DropdownMenuItem>
        ))}
        <DropdownMenuSeparator />
        <DialogTrigger asChild>
          <DropdownMenuItem
            onSelect={(e) => {
              e.preventDefault();
              setIsCreateDialogOpen(true);
            }}
            className="cursor-pointer"
          >
            <Plus className="h-4 w-4 mr-2" />
            <span>Create Workspace</span>
          </DropdownMenuItem>
        </DialogTrigger>
      </DropdownMenuContent>

      <DialogContent>
        <DialogHeader>
          <DialogTitle>Create Workspace</DialogTitle>
          <DialogDescription>
            Create a new workspace to organize your spaces and tasks.
          </DialogDescription>
        </DialogHeader>
        <form onSubmit={handleCreateWorkspace} className="space-y-4">
          <div>
            <label
              htmlFor="new-workspace-name"
              className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1"
            >
              Name <span className="text-red-500">*</span>
            </label>
            <Input
              id="new-workspace-name"
              type="text"
              value={newWorkspaceName}
              onChange={(e) => setNewWorkspaceName(e.target.value)}
              placeholder="My Workspace"
              required
            />
          </div>
          <div>
            <label
              htmlFor="new-workspace-description"
              className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1"
            >
              Description
            </label>
            <Textarea
              id="new-workspace-description"
              value={newWorkspaceDescription}
              onChange={(e) => setNewWorkspaceDescription(e.target.value)}
              placeholder="Optional description..."
              rows={3}
            />
          </div>
          <div className="flex justify-end gap-2">
            <Button
              type="button"
              variant="secondary"
              onClick={() => setIsCreateDialogOpen(false)}
              disabled={isCreating}
            >
              Cancel
            </Button>
            <Button type="submit" disabled={!newWorkspaceName.trim() || isCreating}>
              {isCreating ? 'Creating...' : 'Create Workspace'}
            </Button>
          </div>
        </form>
      </DialogContent>
    </DropdownMenu>
  );
}
