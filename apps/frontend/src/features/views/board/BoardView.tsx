"use client";

import { useState } from "react";
import {
  DndContext,
  closestCenter,
  KeyboardSensor,
  PointerSensor,
  useSensor,
  useSensors,
  DragStartEvent,
  DragOverEvent,
  DragEndEvent,
} from "@dnd-kit/core";
import {
  arrayMove,
  SortableContext,
  sortableKeyboardCoordinates,
  useSortable,
  verticalListSortingStrategy,
} from "@dnd-kit/sortable";
import { restrictToVerticalAxis } from "@dnd-kit/modifiers";
import { GripVertical, Plus } from "lucide-react";
import { clsx } from "clsx";
import type { Task } from "../../tasks/types";

interface BoardViewProps {
  projectId: string;
}

interface Column {
  id: string;
  title: string;
  tasks: Task[];
  color: string;
}

interface TaskCardProps {
  task: Task;
}

function TaskCard({ task }: TaskCardProps) {
  const { attributes, listeners, setNodeRef, transform, transition, isDragging } = useSortable({
    id: task.id,
  });

  const style = {
    transform: transform ? `translate3d(${transform.x}px, ${transform.y}px, 0)` : undefined,
    transition,
  };

  return (
    <div
      ref={setNodeRef}
      style={style}
      className={clsx(
        "p-3 mb-2 bg-background border rounded-lg shadow-sm cursor-grab active:cursor-grabbing",
        isDragging && "opacity-50 shadow-lg"
      )}
      {...attributes}
      {...listeners}
    >
      <div className="flex items-start gap-2">
        <GripVertical className="h-4 w-4 mt-0.5 text-muted-foreground" />
        <div className="flex-1 min-w-0">
          <h4 className="font-medium text-sm truncate">{task.title}</h4>
          {task.description && (
            <p className="text-xs text-muted-foreground mt-1 line-clamp-2">{task.description}</p>
          )}
          <div className="flex items-center gap-2 mt-2">
            {task.priority && (
              <span className={clsx(
                "inline-flex px-1.5 py-0.5 rounded text-xs font-medium",
                task.priority === "high" && "bg-red-100 text-red-800",
                task.priority === "medium" && "bg-yellow-100 text-yellow-800",
                task.priority === "low" && "bg-green-100 text-green-800"
              )}>
                {task.priority}
              </span>
            )}
            {task.assigneeId && (
              <span className="text-xs text-muted-foreground">
                Assigned
              </span>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}

interface DraggableColumnProps {
  column: Column;
}

function DraggableColumn({ column }: DraggableColumnProps) {
  const { attributes, isDragging, setNodeRef, transform, transition } = useSortable({
    id: column.id,
  });

  const style = {
    transform: transform ? `translate3d(${transform.x}px, ${transform.y}px, 0)` : undefined,
    transition,
  };

  return (
    <div
      ref={setNodeRef}
      style={style}
      className={clsx(
        "flex-shrink-0 w-80 bg-muted/30 rounded-lg p-4",
        isDragging && "opacity-50"
      )}
      {...attributes}
    >
      <div className="flex items-center justify-between mb-4">
        <div className="flex items-center gap-2">
          <div className={clsx("w-3 h-3 rounded-full", column.color)} />
          <h3 className="font-semibold">{column.title}</h3>
          <span className="text-sm text-muted-foreground">({column.tasks.length})</span>
        </div>
        <button className="p-1 hover:bg-muted rounded">
          <Plus className="h-4 w-4" />
        </button>
      </div>

      <SortableContext items={column.tasks.map((t) => t.id)} strategy={verticalListSortingStrategy}>
        <div>
          {column.tasks.map((task) => (
            <TaskCard key={task.id} task={task} />
          ))}
        </div>
      </SortableContext>
    </div>
  );
}

export function BoardView({ projectId }: BoardViewProps) {
  const [columns, setColumns] = useState<Column[]>([
    { id: "todo", title: "To Do", tasks: [], color: "bg-gray-500" },
    { id: "in-progress", title: "In Progress", tasks: [], color: "bg-blue-500" },
    { id: "review", title: "Review", tasks: [], color: "bg-yellow-500" },
    { id: "done", title: "Done", tasks: [], color: "bg-green-500" },
  ]);

  // TODO: Fetch tasks by status from API
  // const { data } = useTasksByStatus(projectId);

  const sensors = useSensors(
    useSensor(PointerSensor),
    useSensor(KeyboardSensor, {
      coordinateGetter: sortableKeyboardCoordinates,
    })
  );

  const handleDragStart = (event: DragStartEvent) => {
    // TODO: Track dragged item
  };

  const handleDragOver = (event: DragOverEvent) => {
    const { active, over } = event;
    if (!over) return;

    const activeId = active.id as string;
    const overId = over.id as string;

    // Find active and over columns
    const activeColumn = columns.find((col) => col.tasks.some((t) => t.id === activeId));
    const overColumn = columns.find((col) => col.id === overId || col.tasks.some((t) => t.id === overId));

    if (!activeColumn || !overColumn) return;

    // If dragging over same column, reorder
    if (activeColumn.id === overColumn.id) {
      const oldIndex = activeColumn.tasks.findIndex((t) => t.id === activeId);
      const newIndex = activeColumn.tasks.findIndex((t) => t.id === overId);

      setColumns((cols) =>
        cols.map((col) =>
          col.id === activeColumn.id
            ? { ...col, tasks: arrayMove(col.tasks, oldIndex, newIndex) }
            : col
        )
      );
      return;
    }

    // Moving to different column - handle in DragEnd
  };

  const handleDragEnd = (event: DragEndEvent) => {
    const { active, over } = event;
    if (!over) return;

    const activeId = active.id as string;
    const overId = over.id as string;

    // Find the task and columns
    let activeTask: Task | undefined;
    let sourceColumn: Column | undefined;
    let targetColumn: Column | undefined;

    for (const col of columns) {
      const task = col.tasks.find((t) => t.id === activeId);
      if (task) {
        activeTask = task;
        sourceColumn = col;
      }
      if (col.id === overId || col.tasks.some((t) => t.id === overId)) {
        targetColumn = col;
      }
    }

    if (!activeTask || !sourceColumn || !targetColumn) return;

    const sourceColumnId = sourceColumn.id;
    const targetColumnId = targetColumn.id;

    // If different columns, move the task
    if (sourceColumnId !== targetColumnId) {
      setColumns((cols) =>
        cols.map((col) => {
          if (col.id === sourceColumnId) {
            return { ...col, tasks: col.tasks.filter((t) => t.id !== activeId) };
          }
          if (col.id === targetColumnId) {
            return { ...col, tasks: [...col.tasks, activeTask] };
          }
          return col;
        })
      );

      // TODO: Call API to update task status
      // await updateTaskStatus(activeId, targetColumnId);
    }
  };

  return (
    <div className="board-view">
      <DndContext
        sensors={sensors}
        collisionDetection={closestCenter}
        onDragStart={handleDragStart}
        onDragOver={handleDragOver}
        onDragEnd={handleDragEnd}
      >
        <div className="flex gap-4 overflow-x-auto pb-4">
          {columns.map((column) => (
            <DraggableColumn key={column.id} column={column} />
          ))}
        </div>
      </DndContext>
    </div>
  );
}
