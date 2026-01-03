"use client";

import { useEffect, useState } from "react";
import { useTaskHub } from "@/hooks/signalr/useTaskHub";
import { usePresenceHub } from "@/hooks/signalr/usePresenceHub";
import { ViewingAvatars } from "@/features/tasks/ViewingAvatars";
import { OnlineStatus } from "@/features/users/OnlineStatus";
import { TypingIndicator } from "@/features/tasks/TypingIndicator";
import type { Task } from "./types";

interface Comment {
  id: string;
  taskId: string;
  userId: string;
  userName: string;
  content: string;
  createdAt: Date;
}

interface Attachment {
  id: string;
  taskId: string;
  userId: string;
  userName: string;
  fileName: string;
  fileSizeBytes?: number;
  mimeType?: string;
  createdAt: Date;
}

interface TaskDetailWithRealtimeProps {
  taskId: string;
  projectId: string;
}

export function TaskDetailWithRealtime({
  taskId,
  projectId
}: TaskDetailWithRealtimeProps) {
  const [task, setTask] = useState<Task | null>(null);
  const [comments, setComments] = useState<Comment[]>([]);
  const [attachments, setAttachments] = useState<Attachment[]>([]);
  const [typingUsers, setTypingUsers] = useState<Array<{ id: string; name: string; timestamp: Date }>>([]);
  const [viewingUsers, setViewingUsers] = useState<Array<{ id: string; name: string; email?: string }>>([]);

  // Initialize TaskHub for real-time updates
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
    leaveViewing,
    trackTyping
  } = usePresenceHub();

  useEffect(() => {
    // Join the project group to receive real-time updates
    if (taskHubConnected) {
      joinProject(projectId);
      joinViewing(taskId);
    }

    return () => {
      leaveProject();
      leaveViewing(taskId);
    };
  }, [taskId, projectId, taskHubConnected, joinProject, leaveProject, joinViewing, leaveViewing]);

  // Register TaskUpdated listener
  useEffect(() => {
    const unsubscribe = taskListeners.onTaskUpdated((message) => {
      if (message.taskId === taskId) {
        // Update task in local state
        const taskData = message.data as Partial<Task>;
        setTask((prev) => prev ? { ...prev, ...taskData } : null);
      }
    });

    return unsubscribe;
  }, [taskListeners, taskId]);

  // Register TaskStatusChanged listener
  useEffect(() => {
    const unsubscribe = taskListeners.onTaskStatusChanged((message) => {
      if (message.taskId === taskId) {
        // Update task status in local state
        const taskData = message.data as Partial<Task>;
        setTask((prev) => prev ? { ...prev, ...taskData } : null);
      }
    });

    return unsubscribe;
  }, [taskListeners, taskId]);

  // Register TaskDeleted listener
  useEffect(() => {
    const unsubscribe = taskListeners.onTaskDeleted((message) => {
      if (message.taskId === taskId) {
        // Navigate back to project view
        window.location.href = `/projects/${projectId}`;
      }
    });

    return unsubscribe;
  }, [taskListeners, taskId, projectId]);

  // Register CommentAdded listener
  useEffect(() => {
    const unsubscribe = taskListeners.onCommentAdded((message) => {
      if (message.taskId === taskId) {
        // Add comment to local state
        const commentData = message.data as Comment;
        setComments((prev) => [...prev, commentData]);
      }
    });

    return unsubscribe;
  }, [taskListeners, taskId]);

  // Register CommentUpdated listener
  useEffect(() => {
    const unsubscribe = taskListeners.onCommentUpdated((message) => {
      if (message.taskId === taskId) {
        // Update comment in local state
        const commentData = message.data as Partial<Comment>;
        setComments((prev) =>
          prev.map((comment) =>
            comment.id === message.commentId ? { ...comment, ...commentData } : comment
          )
        );
      }
    });

    return unsubscribe;
  }, [taskListeners, taskId]);

  // Register CommentDeleted listener
  useEffect(() => {
    const unsubscribe = taskListeners.onCommentDeleted((message) => {
      if (message.taskId === taskId) {
        // Remove comment from local state
        setComments((prev) => prev.filter((comment) => comment.id !== message.commentId));
      }
    });

    return unsubscribe;
  }, [taskListeners, taskId]);

  // Register AttachmentUploaded listener
  useEffect(() => {
    const unsubscribe = taskListeners.onAttachmentUploaded((message) => {
      if (message.taskId === taskId) {
        // Add attachment to local state
        const attachmentData = message.data as Attachment;
        setAttachments((prev) => [...prev, attachmentData]);
      }
    });

    return unsubscribe;
  }, [taskListeners, taskId]);

  // Register AttachmentDeleted listener
  useEffect(() => {
    const unsubscribe = taskListeners.onAttachmentDeleted((message) => {
      if (message.taskId === taskId) {
        // Remove attachment from local state
        setAttachments((prev) => prev.filter((attachment) => attachment.id !== message.attachmentId));
      }
    });

    return unsubscribe;
  }, [taskListeners, taskId]);

  // Sync viewing users from presence hub
  useEffect(() => {
    const viewers = onlineUsers
      .filter((user) => user.currentView?.entityId === taskId && user.currentView?.viewType === "task")
      .map((user) => ({
        id: user.userId,
        name: user.userName,
        email: user.userEmail
      }));

    setViewingUsers(viewers);
  }, [onlineUsers, taskId]);

  // Handle typing in comments
  const handleCommentInput = () => {
    trackTyping(taskId);
  };

  // TODO: Fetch initial task data from API
  // useEffect(() => {
  //   fetchTask(taskId).then(setTask);
  //   fetchComments(taskId).then(setComments);
  //   fetchAttachments(taskId).then(setAttachments);
  // }, [taskId]);

  if (!task) {
    return <div>Loading...</div>;
  }

  return (
    <div className="task-detail relative">
      {/* Header with viewing avatars */}
      <div className="flex items-center justify-between mb-4">
        <div>
          <h1 className="text-3xl font-bold">{task.title}</h1>
          <p className="text-muted-foreground">Task #{task.id.slice(0, 8)}</p>
        </div>
        <ViewingAvatars users={viewingUsers} />
      </div>

      {/* Typing indicator */}
      {typingUsers.length > 0 && (
        <div className="mb-4">
          <TypingIndicator typingUsers={typingUsers} />
        </div>
      )}

      {/* Task assignee with online status */}
      {task.assigneeId && (
        <div className="mb-4">
          <span className="text-sm text-muted-foreground">Assigned to: </span>
          <OnlineStatus
            userName="John Doe"
            userEmail="john@example.com"
            isOnline={onlineUsers.some((u) => u.userId === task.assigneeId)}
          />
        </div>
      )}

      {/* Task details */}
      <div className="space-y-4">
        <div>
          <h2 className="text-lg font-semibold mb-2">Description</h2>
          <p className="text-muted-foreground">{task.description || "No description"}</p>
        </div>

        <div>
          <h2 className="text-lg font-semibold mb-2">Comments</h2>
          <div className="space-y-2">
            {comments.map((comment) => (
              <div key={comment.id} className="p-3 border rounded-lg">
                <div className="flex items-center gap-2 mb-1">
                  <span className="font-medium">{comment.userName}</span>
                  <span className="text-xs text-muted-foreground">
                    {new Date(comment.createdAt).toLocaleString()}
                  </span>
                </div>
                <p className="text-sm">{comment.content}</p>
              </div>
            ))}
          </div>
          <textarea
            className="mt-4 w-full p-3 border rounded-lg"
            placeholder="Add a comment..."
            onFocus={handleCommentInput}
          />
        </div>

        <div>
          <h2 className="text-lg font-semibold mb-2">Attachments</h2>
          <div className="space-y-2">
            {attachments.map((attachment) => (
              <div key={attachment.id} className="p-3 border rounded-lg flex items-center gap-2">
                <span>📎</span>
                <span>{attachment.fileName}</span>
              </div>
            ))}
          </div>
        </div>
      </div>
    </div>
  );
}
