// Workspace Types

export interface Workspace {
  id: string;
  name: string;
  description?: string;
  ownerId: string;
  isDefault: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface CreateWorkspaceRequest {
  name: string;
  description?: string;
}

export interface UpdateWorkspaceRequest {
  name?: string;
  description?: string;
}

// Workspace Member Types
export interface WorkspaceMember {
  id: string;
  workspaceId: string;
  userId: string;
  role: 'owner' | 'admin' | 'member' | 'viewer';
  joinedAt: string;
}

export interface WorkspaceMemberWithUser extends WorkspaceMember {
  user: {
    id: string;
    name: string;
    email: string;
    avatar?: string;
  };
}
