import { useEffect, useState } from "react";
import { cn } from "@/lib/utils";

interface TypingUser {
  id: string;
  name: string;
  timestamp: Date;
}

interface TypingIndicatorProps {
  typingUsers: TypingUser[];
  expireAfter?: number; // milliseconds
  className?: string;
}

export function TypingIndicator({
  typingUsers,
  expireAfter = 3000,
  className = ""
}: TypingIndicatorProps) {
  const [visibleUsers, setVisibleUsers] = useState<TypingUser[]>([]);

  useEffect(() => {
    // Filter out expired typing indicators
    const now = new Date();
    const activeUsers = typingUsers.filter(
      (user) => now.getTime() - new Date(user.timestamp).getTime() < expireAfter
    );
    setVisibleUsers(activeUsers);

    // Clear expired users after expireAfter
    const timer = setTimeout(() => {
      setVisibleUsers([]);
    }, expireAfter);

    return () => clearTimeout(timer);
  }, [typingUsers, expireAfter]);

  if (visibleUsers.length === 0) {
    return null;
  }

  const getText = () => {
    if (visibleUsers.length === 1) {
      return `${visibleUsers[0].name} is typing...`;
    } else if (visibleUsers.length === 2) {
      return `${visibleUsers[0].name} and ${visibleUsers[1].name} are typing...`;
    } else {
      return `${visibleUsers.length} people are typing...`;
    }
  };

  return (
    <div
      className={cn(
        "flex items-center gap-2 text-sm text-muted-foreground animate-fade-in",
        className
      )}
    >
      <div className="flex gap-1">
        <span className="animate-bounce">•</span>
        <span className="animate-bounce" style={{ animationDelay: "0.1s" }}>
          •
        </span>
        <span className="animate-bounce" style={{ animationDelay: "0.2s" }}>
          •
        </span>
      </div>
      <span>{getText()}</span>
    </div>
  );
}
