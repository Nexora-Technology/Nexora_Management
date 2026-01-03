import { Avatar } from "@/components/ui/avatar";
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from "@/components/ui/tooltip";

interface ViewingUser {
  id: string;
  name: string;
  email?: string;
  avatar?: string;
}

interface ViewingAvatarsProps {
  users: ViewingUser[];
  maxVisible?: number;
  containerClassName?: string;
}

export function ViewingAvatars({
  users,
  maxVisible = 3,
  containerClassName = ""
}: ViewingAvatarsProps) {
  if (users.length === 0) {
    return null;
  }

  const visibleUsers = users.slice(0, maxVisible);
  const remainingCount = Math.max(0, users.length - maxVisible);

  const getInitials = (name: string) => {
    return name
      .split(" ")
      .map((n) => n[0])
      .join("")
      .toUpperCase()
      .slice(0, 2);
  };

  return (
    <TooltipProvider>
      <div className={`flex items-center -space-x-2 ${containerClassName}`}>
        {visibleUsers.map((user, index) => (
          <Tooltip key={user.id}>
            <TooltipTrigger asChild>
              <Avatar
                className="h-8 w-8 border-2 border-background cursor-pointer hover:z-10 transition-transform hover:scale-110"
                style={{ zIndex: index }}
              >
                {user.avatar ? (
                  <img src={user.avatar} alt={user.name} className="h-full w-full object-cover" />
                ) : (
                  <span className="text-xs font-medium">{getInitials(user.name)}</span>
                )}
              </Avatar>
            </TooltipTrigger>
            <TooltipContent>
              <div className="space-y-1">
                <p className="font-medium">{user.name}</p>
                {user.email && <p className="text-xs text-muted-foreground">{user.email}</p>}
                <p className="text-xs text-muted-foreground">Viewing now</p>
              </div>
            </TooltipContent>
          </Tooltip>
        ))}

        {remainingCount > 0 && (
          <Tooltip>
            <TooltipTrigger asChild>
              <div className="h-8 w-8 rounded-full bg-muted border-2 border-background flex items-center justify-center text-xs font-medium cursor-pointer hover:bg-muted-foreground/20 transition-colors">
                +{remainingCount}
              </div>
            </TooltipTrigger>
            <TooltipContent>
              <div className="space-y-1">
                <p className="font-medium">{remainingCount} more viewing</p>
                <div className="max-h-32 overflow-y-auto space-y-1">
                  {users.slice(maxVisible).map((user) => (
                    <div key={user.id} className="text-xs">
                      {user.name}
                    </div>
                  ))}
                </div>
              </div>
            </TooltipContent>
          </Tooltip>
        )}
      </div>
    </TooltipProvider>
  );
}
