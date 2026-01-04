import apiClient from "@/lib/api-client";
import {
  Page,
  PageDetail,
  PageTreeNode,
  PageVersion,
  PageComment,
  CreatePageRequest,
  UpdatePageRequest,
  MovePageRequest,
  RestoreVersionRequest,
  SearchPagesRequest,
  SearchPagesResponse,
} from "./types";

// Base URL for documents
const DOCUMENTS_BASE = "/api/documents";

// Documents API
export const documentsApi = {
  // Get page by ID
  getPageById: async (pageId: string): Promise<PageDetail> => {
    const response = await apiClient.get<PageDetail>(`${DOCUMENTS_BASE}/${pageId}`);
    return response.data;
  },

  // Create new page
  createPage: async (data: CreatePageRequest): Promise<PageDetail> => {
    const response = await apiClient.post<PageDetail>(DOCUMENTS_BASE, data);
    return response.data;
  },

  // Update page
  updatePage: async (
    pageId: string,
    data: UpdatePageRequest
  ): Promise<PageDetail> => {
    const response = await apiClient.put<PageDetail>(
      `${DOCUMENTS_BASE}/${pageId}`,
      data
    );
    return response.data;
  },

  // Delete page (soft delete)
  deletePage: async (pageId: string): Promise<void> => {
    await apiClient.delete(`${DOCUMENTS_BASE}/${pageId}`);
  },

  // Get page tree for workspace
  getPageTree: async (workspaceId: string): Promise<PageTreeNode[]> => {
    const response = await apiClient.get<PageTreeNode[]>(
      `${DOCUMENTS_BASE}/tree/${workspaceId}`
    );
    return response.data;
  },

  // Toggle favorite
  toggleFavorite: async (pageId: string): Promise<Page> => {
    const response = await apiClient.post<Page>(
      `${DOCUMENTS_BASE}/${pageId}/favorite`
    );
    return response.data;
  },

  // Get page version history
  getPageHistory: async (pageId: string): Promise<PageVersion[]> => {
    const response = await apiClient.get<PageVersion[]>(
      `${DOCUMENTS_BASE}/${pageId}/versions`
    );
    return response.data;
  },

  // Restore page version
  restoreVersion: async (
    pageId: string,
    data: RestoreVersionRequest
  ): Promise<PageDetail> => {
    const response = await apiClient.post<PageDetail>(
      `${DOCUMENTS_BASE}/${pageId}/restore`,
      data
    );
    return response.data;
  },

  // Move page
  movePage: async (pageId: string, data: MovePageRequest): Promise<Page> => {
    const response = await apiClient.post<Page>(
      `${DOCUMENTS_BASE}/${pageId}/move`,
      data
    );
    return response.data;
  },

  // Search pages
  searchPages: async (
    params: SearchPagesRequest
  ): Promise<SearchPagesResponse> => {
    const response = await apiClient.get<SearchPagesResponse>(
      `${DOCUMENTS_BASE}/search`,
      { params }
    );
    return response.data;
  },

  // Get page comments (if needed)
  getPageComments: async (pageId: string): Promise<PageComment[]> => {
    const response = await apiClient.get<PageComment[]>(
      `${DOCUMENTS_BASE}/${pageId}/comments`
    );
    return response.data;
  },

  // Add comment to page (if needed)
  addComment: async (
    pageId: string,
    content: string,
    selection?: Record<string, unknown> | null,
    parentCommentId?: string | null
  ): Promise<PageComment> => {
    const response = await apiClient.post<PageComment>(
      `${DOCUMENTS_BASE}/${pageId}/comments`,
      {
        content,
        selection,
        parentCommentId,
      }
    );
    return response.data;
  },

  // Resolve comment (if needed)
  resolveComment: async (
    pageId: string,
    commentId: string
  ): Promise<PageComment> => {
    const response = await apiClient.post<PageComment>(
      `${DOCUMENTS_BASE}/${pageId}/comments/${commentId}/resolve`
    );
    return response.data;
  },
};

export default documentsApi;
