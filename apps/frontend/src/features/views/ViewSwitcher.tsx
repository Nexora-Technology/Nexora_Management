"use client";

import { useViewContext, type ViewType } from "./ViewContext";
import { List, LayoutDashboard, Calendar, BarChart3 } from "lucide-react";
import { buttonVariants } from "@/components/ui/button";
import { clsx } from "clsx";

const views: { id: ViewType; label: string; icon: React.ComponentType<{ className?: string }> }[] = [
  { id: "list", label: "List", icon: List },
  { id: "board", label: "Board", icon: LayoutDashboard },
  { id: "calendar", label: "Calendar", icon: Calendar },
  { id: "gantt", label: "Gantt", icon: BarChart3 },
];

export function ViewSwitcher() {
  const { currentView, setCurrentView } = useViewContext();

  return (
    <div className="inline-flex items-center gap-1 rounded-lg border bg-background p-1">
      {views.map((view) => {
        const Icon = view.icon;
        const isActive = currentView === view.id;

        return (
          <button
            key={view.id}
            onClick={() => setCurrentView(view.id)}
            className={clsx(
              buttonVariants({ variant: "ghost", size: "sm" }),
              "gap-2 rounded-md px-3",
              isActive && "bg-muted"
            )}
            title={`Switch to ${view.label} view`}
          >
            <Icon className="h-4 w-4" />
            <span className="hidden sm:inline">{view.label}</span>
          </button>
        );
      })}
    </div>
  );
}
