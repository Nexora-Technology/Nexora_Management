import type { Editor } from "@tiptap/react";

// Document/Page Types
export interface Page {
  id: string;
  workspaceId: string;
  parentPageId: string | null;
  title: string;
  slug: string;
  icon: string | null;
  coverImage: string | null;
  contentType: string;
  status: string;
  isFavorite: boolean;
  positionOrder: number;
  createdBy: string;
  updatedBy: string;
  createdAt: string;
  updatedAt: string;
}

export interface PageDetail extends Page {
  content: Record<string, unknown>;
  createdByName: string | null;
  updatedByName: string | null;
}

export interface PageTreeNode {
  id: string;
  parentPageId: string | null;
  title: string;
  slug: string;
  icon: string | null;
  contentType: string;
  status: string;
  isFavorite: boolean;
  positionOrder: number;
  children: PageTreeNode[];
}

export interface PageVersion {
  id: string;
  pageId: string;
  versionNumber: number;
  content: Record<string, unknown>;
  commitMessage: string | null;
  createdBy: string;
  createdByName: string | null;
  createdAt: string;
}

export interface PageComment {
  id: string;
  pageId: string;
  userId: string;
  userName: string | null;
  content: string;
  selection: Record<string, unknown> | null;
  parentCommentId: string | null;
  createdAt: string;
  resolvedAt: string | null;
}

// Request/Response Types
export interface CreatePageRequest {
  workspaceId: string;
  title: string;
  parentPageId?: string | null;
  icon?: string | null;
  contentType?: string;
}

export interface UpdatePageRequest {
  title: string;
  content: Record<string, unknown>;
  icon?: string | null;
  coverImage?: string | null;
}

export interface MovePageRequest {
  newParentPageId?: string | null;
  newPositionOrder: number;
}

export interface RestoreVersionRequest {
  versionNumber: number;
}

export interface SearchPagesRequest {
  workspaceId: string;
  searchTerm?: string | null;
  status?: string | null;
  favoriteOnly?: boolean | null;
  page?: number;
  pageSize?: number;
}

export interface SearchPagesResponse {
  items: Page[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface EditorProps {
  content: Record<string, unknown>;
  onUpdate: (content: Record<string, unknown>) => void;
  editable?: boolean;
  placeholder?: string;
}

export interface ToolbarProps {
  editor: Editor | null;
  onSetLink?: () => void;
  onAddImage?: () => void;
}

// Document View Types
export interface DocumentViewMode {
  mode: 'edit' | 'view' | 'split';
}

export interface PageTreeProps {
  workspaceId: string;
  onPageSelect: (pageId: string) => void;
  selectedPageId?: string;
}

export interface VersionHistoryProps {
  pageId: string;
  onRestore: (versionNumber: number) => void;
}
