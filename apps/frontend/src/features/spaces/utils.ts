import type { Space, Folder, TaskList, SpaceTreeNode } from './types';

/**
 * Builds a hierarchical tree structure from flat lists of spaces, folders, and tasklists.
 * This transforms the database relationship model into a UI-friendly tree for navigation.
 *
 * Hierarchy: Space → Folder (optional) → TaskList
 *
 * @param spaces - Array of space objects
 * @param folders - Array of folder objects (optional level in hierarchy)
 * @param taskLists - Array of tasklist objects (must be in a space or folder)
 * @returns Hierarchical tree nodes suitable for rendering in a tree component
 */
export function buildSpaceTree(
  spaces: Space[],
  folders: Folder[],
  taskLists: TaskList[]
): SpaceTreeNode[] {
  const tree: SpaceTreeNode[] = [];

  // Build space nodes map
  const spaceMap = new Map<string, SpaceTreeNode>();
  spaces.forEach((space) => {
    spaceMap.set(space.id, {
      id: space.id,
      name: space.name,
      type: 'space',
      color: space.color,
      icon: space.icon,
      children: [],
    });
  });

  // Build folder nodes and attach to parent spaces
  const folderMap = new Map<string, SpaceTreeNode>();
  folders.forEach((folder) => {
    const node: SpaceTreeNode = {
      id: folder.id,
      name: folder.name,
      type: 'folder',
      spaceId: folder.spaceId,
      color: folder.color,
      icon: folder.icon,
      children: [],
    };
    folderMap.set(folder.id, node);

    // Attach folder to its parent space
    const parentSpace = spaceMap.get(folder.spaceId);
    if (parentSpace) {
      parentSpace.children!.push(node);
    }
  });

  // Attach tasklists to their parent (folder or space)
  taskLists.forEach((taskList) => {
    const node: SpaceTreeNode = {
      id: taskList.id,
      name: taskList.name,
      type: 'tasklist',
      spaceId: taskList.spaceId,
      folderId: taskList.folderId,
      color: taskList.color,
      icon: taskList.icon,
      listType: taskList.listType,
    };

    if (taskList.folderId) {
      // Attach to parent folder
      const parentFolder = folderMap.get(taskList.folderId);
      if (parentFolder) {
        parentFolder.children!.push(node);
      }
    } else {
      // Attach directly to parent space
      const parentSpace = spaceMap.get(taskList.spaceId);
      if (parentSpace) {
        parentSpace.children!.push(node);
      }
    }
  });

  // Return only spaces (folders and tasklists are nested inside)
  return Array.from(spaceMap.values());
}

/**
 * Finds a node in the space tree by its ID.
 * Useful for finding a specific space, folder, or tasklist in a large tree.
 *
 * @param nodes - Array of tree nodes to search
 * @param id - ID of the node to find
 * @returns The found node or undefined
 */
export function findNodeById(nodes: SpaceTreeNode[], id: string): SpaceTreeNode | undefined {
  for (const node of nodes) {
    if (node.id === id) {
      return node;
    }
    if (node.children) {
      const found = findNodeById(node.children, id);
      if (found) {
        return found;
      }
    }
  }
  return undefined;
}

/**
 * Gets the breadcrumb path to a node.
 * Returns an array of nodes from root to the target node.
 *
 * @param nodes - Array of tree nodes to search
 * @param id - ID of the target node
 * @returns Array of nodes representing the path (root → target)
 */
export function getNodePath(nodes: SpaceTreeNode[], id: string): SpaceTreeNode[] {
  for (const node of nodes) {
    if (node.id === id) {
      return [node];
    }
    if (node.children) {
      const path = getNodePath(node.children, id);
      if (path.length > 0) {
        return [node, ...path];
      }
    }
  }
  return [];
}
