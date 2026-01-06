// Space Types
export interface Space {
  id: string;
  workspaceId: string;
  name: string;
  description?: string;
  color?: string;
  icon?: string;
  isPrivate: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface CreateSpaceRequest {
  workspaceId: string;
  name: string;
  description?: string;
  color?: string;
  icon?: string;
  isPrivate?: boolean;
}

export interface UpdateSpaceRequest {
  name?: string;
  description?: string;
  color?: string;
  icon?: string;
  isPrivate?: boolean;
}

// Folder Types
export interface Folder {
  id: string;
  spaceId: string;
  name: string;
  description?: string;
  color?: string;
  icon?: string;
  positionOrder: number;
  createdAt: string;
  updatedAt: string;
}

export interface CreateFolderRequest {
  spaceId: string;
  name: string;
  description?: string;
  color?: string;
  icon?: string;
}

export interface UpdateFolderRequest {
  name?: string;
  description?: string;
  color?: string;
  icon?: string;
}

export interface UpdateFolderPositionRequest {
  positionOrder: number;
}

// TaskList Types (NEW ENTITY - replacing Project in hierarchy)
export interface TaskList {
  id: string;
  spaceId: string;
  folderId?: string;
  name: string;
  description?: string;
  color?: string;
  icon?: string;
  listType: 'task' | 'project' | 'team' | 'campaign';
  status: string;
  ownerId: string;
  positionOrder: number;
  createdAt: string;
  updatedAt: string;
}

export interface CreateTaskListRequest {
  spaceId: string;
  folderId?: string;
  name: string;
  description?: string;
  color?: string;
  icon?: string;
  listType?: string;
  ownerId?: string;
}

export interface UpdateTaskListRequest {
  name?: string;
  description?: string;
  color?: string;
  icon?: string;
  status?: string;
}

export interface UpdateTaskListPositionRequest {
  positionOrder: number;
}

// Tree Navigation Types
export type SpaceTreeNodeType = 'space' | 'folder' | 'tasklist';

export interface SpaceTreeNode {
  id: string;
  name: string;
  type: SpaceTreeNodeType;
  spaceId?: string;
  folderId?: string;
  children?: SpaceTreeNode[];
  color?: string;
  icon?: string;
  listType?: string;
}
