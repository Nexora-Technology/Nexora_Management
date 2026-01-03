"use client";

import { useViewContext } from "./ViewContext";
import { ListView } from "./list/ListView";
import { BoardView } from "./board/BoardView";
import { CalendarView } from "./calendar/CalendarView";
import { GanttView } from "./gantt/GanttView";

interface ViewLayoutProps {
  projectId: string;
}

export function ViewLayout({ projectId }: ViewLayoutProps) {
  const { currentView } = useViewContext();

  const renderView = () => {
    switch (currentView) {
      case "list":
        return <ListView projectId={projectId} />;
      case "board":
        return <BoardView projectId={projectId} />;
      case "calendar":
        return <CalendarView projectId={projectId} />;
      case "gantt":
        return <GanttView projectId={projectId} />;
      default:
        return <ListView projectId={projectId} />;
    }
  };

  return (
    <div className="view-container">
      <div className="view-header flex items-center justify-between mb-6">
        <h1 className="text-2xl font-bold">Tasks</h1>
        <div className="flex items-center gap-4">
          {/* Additional toolbar items like filters can go here */}
        </div>
      </div>
      {renderView()}
    </div>
  );
}
