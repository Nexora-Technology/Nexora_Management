import { Avatar, AvatarProps } from "@/components/ui/avatar";
import { Badge } from "@/components/ui/badge";
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from "@/components/ui/tooltip";

interface OnlineStatusProps extends Omit<AvatarProps, 'children'> {
  userName: string;
  userEmail?: string;
  isOnline?: boolean;
  lastSeen?: Date;
}

export function OnlineStatus({
  userName,
  userEmail,
  isOnline = false,
  lastSeen,
  ...avatarProps
}: OnlineStatusProps) {
  const initials = userName
    .split(" ")
    .map((n) => n[0])
    .join("")
    .toUpperCase()
    .slice(0, 2);

  const statusColor = isOnline ? "bg-green-500" : "bg-gray-400";
  const statusText = isOnline ? "Online" : `Last seen ${lastSeen ? new Date(lastSeen).toLocaleString() : "recently"}`;

  return (
    <TooltipProvider>
      <Tooltip>
        <TooltipTrigger asChild>
          <div className="relative inline-block">
            <Avatar {...avatarProps}>
              {/* Avatar content will be handled by parent */}
              <span className="text-sm font-medium">{initials}</span>
            </Avatar>
            <Badge
              className={`absolute -bottom-1 -right-1 h-4 w-4 rounded-full p-0 border-2 border-background ${statusColor}`}
              aria-label={statusText}
            />
          </div>
        </TooltipTrigger>
        <TooltipContent>
          <div className="space-y-1">
            <p className="font-medium">{userName}</p>
            {userEmail && <p className="text-xs text-muted-foreground">{userEmail}</p>}
            <p className="text-xs text-muted-foreground">{statusText}</p>
          </div>
        </TooltipContent>
      </Tooltip>
    </TooltipProvider>
  );
}
