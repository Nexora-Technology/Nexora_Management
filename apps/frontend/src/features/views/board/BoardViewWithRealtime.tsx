"use client";

import { useEffect, useState } from "react";
import { BoardView } from "./BoardView";
import { useTaskHub } from "@/hooks/signalr/useTaskHub";
import { usePresenceHub } from "@/hooks/signalr/usePresenceHub";
import { ViewingAvatars } from "@/features/tasks/ViewingAvatars";
import { TypingIndicator } from "@/features/tasks/TypingIndicator";
import type { Task } from "@/features/tasks/types";

interface BoardViewWithRealtimeProps {
  projectId: string;
}

export function BoardViewWithRealtime({
  projectId
}: BoardViewWithRealtimeProps) {
  const [tasks, setTasks] = useState<Task[]>([]);
  const [typingUsers, setTypingUsers] = useState<Array<{ id: string; name: string; timestamp: Date }>>([]);
  const [viewingUsers, setViewingUsers] = useState<Array<{ id: string; name: string; email?: string }>>([]);

  // Initialize TaskHub for real-time task updates
  const {
    isConnected: taskHubConnected,
    joinProject,
    leaveProject,
    listeners: taskListeners
  } = useTaskHub();

  // Initialize PresenceHub for user presence
  const {
    onlineUsers,
    joinViewing,
    leaveViewing
  } = usePresenceHub();

  useEffect(() => {
    // Join the project group to receive real-time updates
    if (taskHubConnected) {
      joinProject(projectId);
      joinViewing(projectId);
    }

    return () => {
      leaveProject();
      leaveViewing(projectId);
    };
  }, [projectId, taskHubConnected, joinProject, leaveProject, joinViewing, leaveViewing]);

  // Register TaskCreated listener
  useEffect(() => {
    const unsubscribe = taskListeners.onTaskCreated((message) => {
      if (message.projectId === projectId) {
        // Add new task to local state
        setTasks((prev) => [...prev, message.data as Task]);
      }
    });

    return unsubscribe;
  }, [taskListeners, projectId]);

  // Register TaskUpdated listener
  useEffect(() => {
    const unsubscribe = taskListeners.onTaskUpdated((message) => {
      if (message.projectId === projectId) {
        // Update existing task in local state
        setTasks((prev) =>
          prev.map((task) =>
            task.id === message.taskId ? { ...task, ...(message.data as Task) } : task
          )
        );
      }
    });

    return unsubscribe;
  }, [taskListeners, projectId]);

  // Register TaskDeleted listener
  useEffect(() => {
    const unsubscribe = taskListeners.onTaskDeleted((message) => {
      if (message.projectId === projectId) {
        // Remove task from local state
        setTasks((prev) => prev.filter((task) => task.id !== message.taskId));
      }
    });

    return unsubscribe;
  }, [taskListeners, projectId]);

  // Register TaskStatusChanged listener
  useEffect(() => {
    const unsubscribe = taskListeners.onTaskStatusChanged((message) => {
      if (message.projectId === projectId) {
        // Update task status in local state
        setTasks((prev) =>
          prev.map((task) =>
            task.id === message.taskId ? { ...task, ...(message.data as Task) } : task
          )
        );
      }
    });

    return unsubscribe;
  }, [taskListeners, projectId]);

  // Sync viewing users from presence hub
  useEffect(() => {
    const viewers = onlineUsers
      .filter((user) => user.currentView?.entityId === projectId && user.currentView?.viewType === "board")
      .map((user) => ({
        id: user.userId,
        name: user.userName,
        email: user.userEmail
      }));

    setViewingUsers(viewers);
  }, [onlineUsers, projectId]);

  // TODO: Fetch initial tasks from API
  // useEffect(() => {
  //   fetchTasks(projectId).then(setTasks);
  // }, [projectId]);

  return (
    <div className="relative">
      {/* Header with viewing avatars */}
      <div className="flex items-center justify-between mb-4">
        <h2 className="text-2xl font-bold">Board View</h2>
        <ViewingAvatars users={viewingUsers} />
      </div>

      {/* Typing indicator */}
      {typingUsers.length > 0 && (
        <div className="mb-4">
          <TypingIndicator typingUsers={typingUsers} />
        </div>
      )}

      {/* Board view with real-time updates */}
      <BoardView projectId={projectId} />
    </div>
  );
}
