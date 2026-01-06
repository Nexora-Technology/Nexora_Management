# Code Standards & Development Guidelines

**Last Updated:** 2026-01-06
**Version:** Phase 08 Complete (Goal Tracking & OKRs, Drag-Drop Improvements)
**Document Status:** Active

## Table of Contents

- [Backend Standards (C# / .NET)](#backend-standards-c--net)
- [Frontend Standards (TypeScript / React)](#frontend-standards-typescript--react)
- [Database Standards](#database-standards)
- [API Standards](#api-standards)
- [Testing Standards](#testing-standards)
- [Documentation Standards](#documentation-standards)
- [Git Workflow](#git-workflow)
- [Code Review Guidelines](#code-review-guidelines)

## Backend Standards (C# / .NET)

### Code Style

**Naming Conventions:**

- **Classes:** PascalCase (e.g., `User`, `TaskService`, `IAppDbContext`)
- **Methods:** PascalCase (e.g., `GetById`, `CreateTask`, `ValidateUser`)
- **Properties:** PascalCase (e.g., `FirstName`, `LastName`, `CreatedAt`)
- **Local Variables:** camelCase (e.g., `userId`, `taskList`, `isNew`)
- **Constants:** PascalCase (e.g., `MaxFileSize`, `DefaultTimeout`)
- **Private Fields:** \_camelCase with underscore prefix (e.g., `_dbContext`, `_logger`)
- **Interfaces:** PascalCase with `I` prefix (e.g., `IUserService`, `IRepository<T>`)

**File Organization:**

```
Namespace/
├── Commands/
│   ├── CreateEntity/
│   │   ├── CreateEntityCommand.cs
│   │   └── CreateEntityCommandHandler.cs
│   └── UpdateEntity/
│       ├── UpdateEntityCommand.cs
│       └── UpdateEntityCommandHandler.cs
├── Queries/
│   ├── GetEntityById/
│   │   ├── GetEntityByIdQuery.cs
│   │   └── GetEntityByIdQueryHandler.cs
│   └── GetEntities/
│       ├── GetEntitiesQuery.cs
│       └── GetEntitiesQueryHandler.cs
└── DTOs/
    ├── EntityRequest.cs
    └── EntityResponse.cs
```

**Clean Architecture Principles:**

1. **Domain Layer** (`Nexora.Management.Domain`)
   - No dependencies on external frameworks
   - Pure C# POCO classes
   - Business logic and validation rules
   - Interfaces defined here

2. **Application Layer** (`Nexora.Management.Application`)
   - CQRS pattern with MediatR
   - Use case orchestration
   - DTOs for data transfer
   - Business logic implementation

3. **Infrastructure Layer** (`Nexora.Management.Infrastructure`)
   - EF Core implementations
   - External service integrations
   - Data access logic
   - Technical implementations

4. **API Layer** (`Nexora.Management.API`)
   - Minimal API endpoints
   - Request/response mapping
   - Middleware and filters
   - Presentation logic only

### C# Code Quality Standards

**Class Design:**

```csharp
// ✅ Good: Single responsibility, clear naming
public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, Result<TaskDto>>
{
    private readonly IAppDbContext _dbContext;
    private readonly IUserContext _userContext;
    private readonly ILogger<CreateTaskCommandHandler> _logger;

    public CreateTaskCommandHandler(
        IAppDbContext dbContext,
        IUserContext userContext,
        ILogger<CreateTaskCommandHandler> logger)
    {
        _dbContext = dbContext;
        _userContext = userContext;
        _logger = logger;
    }

    public async Task<Result<TaskDto>> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        // Implementation
    }
}

// ❌ Bad: Multiple responsibilities, unclear naming
public class TaskManager
{
    public async Task<object> DoSomething(object data) { }
}
```

**Async/Await Best Practices:**

```csharp
// ✅ Good: Configure await, pass cancellation token
public async Task<Result<TaskDto>> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
{
    var task = await _dbContext.Tasks
        .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

    await _dbContext.SaveChangesAsync(cancellationToken);
}

// ❌ Bad: No cancellation token, no configure await
public async Task<Result<TaskDto>> Handle(CreateTaskCommand request)
{
    var task = await _dbContext.Tasks.FirstOrDefaultAsync(t => t.Id == request.Id).ConfigureAwait(false);
    await _dbContext.SaveChangesAsync();
}
```

**Exception Handling:**

```csharp
// ✅ Good: Result pattern instead of exceptions
public async Task<Result<TaskDto>> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
{
    if (string.IsNullOrWhiteSpace(request.Title))
    {
        return Result<TaskDto>.Failure("Title is required");
    }

    try
    {
        var task = new Task { Title = request.Title };
        await _dbContext.Tasks.AddAsync(task, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result<TaskDto>.Success(_mapper.Map<TaskDto>(task));
    }
    catch (DbUpdateException ex)
    {
        _logger.LogError(ex, "Database error while creating task");
        return Result<TaskDto>.Failure("Failed to create task");
    }
}

// ❌ Bad: Throwing exceptions for flow control
public async Task<TaskDto> Handle(CreateTaskCommand request)
{
    if (string.IsNullOrWhiteSpace(request.Title))
    {
        throw new ValidationException("Title is required");
    }
    // ...
}
```

**LINQ Best Practices:**

```csharp
// ✅ Good: Deferred execution, proper projection
var tasks = await _dbContext.Tasks
    .Where(t => t.ProjectId == projectId)
    .Select(t => new TaskDto
    {
        Id = t.Id,
        Title = t.Title,
        Status = t.Status.Name
    })
    .ToListAsync(cancellationToken);

// ❌ Bad: Immediate execution, over-fetching
var tasks = _dbContext.Tasks
    .ToList()
    .Where(t => t.ProjectId == projectId)
    .Select(t => new TaskDto
    {
        Id = t.Id,
        Title = t.Title,
        Status = t.Status.Name,
        // Fetching unnecessary data
        CreatedAt = t.CreatedAt,
        UpdatedAt = t.UpdatedAt
    })
    .ToList();
```

### Authorization Standards

**Permission-Based Authorization:**

```csharp
// ✅ Good: Using RequirePermission attribute
[RequirePermission("tasks", "create")]
[HttpPost]
public async Task<IActionResult> CreateTask(CreateTaskRequest request)
{
    var result = await _mediator.Send(new CreateTaskCommand(request));
    return Ok(result);
}

// ❌ Bad: Manual authorization checks
[HttpPost]
public async Task<IActionResult> CreateTask(CreateTaskRequest request)
{
    var user = _userContext.GetCurrentUser();
    if (!user.HasPermission("tasks:create"))
    {
        return Forbid();
    }
    // ...
}
```

## Frontend Standards (TypeScript / React)

### Performance Optimization Patterns

**React.memo with Custom Comparison Functions:**

```typescript
// ✅ Good: React.memo with custom comparison for granular control
import { memo } from "react"

export const TaskCard = memo(function TaskCard({ task, onClick, className }: TaskCardProps) {
  return (
    <div onClick={onClick} className={className}>
      <h4>{task.title}</h4>
      <Badge status={task.status}>{task.status}</Badge>
    </div>
  )
}, (prevProps, nextProps) => {
  // Only re-render if specific props change
  return (
    prevProps.task.id === nextProps.task.id &&
    prevProps.task.title === nextProps.task.title &&
    prevProps.task.status === nextProps.task.status &&
    prevProps.task.priority === nextProps.task.priority &&
    prevProps.onClick === nextProps.onClick &&
    prevProps.className === nextProps.className
  )
})

// ✅ Good: React.memo for array-based props
export const TaskBoard = memo(function TaskBoard({ tasks, onTaskClick }: TaskBoardProps) {
  // ... component implementation
}, (prevProps, nextProps) => {
  // Compare array length and item properties
  return (
    prevProps.tasks.length === nextProps.tasks.length &&
    prevProps.tasks.every((t, i) => t.id === nextProps.tasks[i]?.id) &&
    prevProps.tasks.every((t, i) => t.status === nextProps.tasks[i]?.status) &&
    prevProps.onTaskClick === nextProps.onTaskClick
  )
})

// ❌ Bad: React.memo without comparison function
// Shallow comparison may cause unnecessary re-renders for complex objects
export const TaskCard = memo(function TaskCard({ task, onClick }) {
  return <div onClick={onClick}>{task.title}</div>
})

// ❌ Bad: Not using memo at all
// Component re-renders on every parent update
export const TaskCard = function TaskCard({ task, onClick }) {
  return <div onClick={onClick}>{task.title}</div>
}
```

**Benefits of Custom Comparison Functions:**

- Prevent unnecessary re-renders by comparing only relevant properties
- Handle complex objects (arrays, nested objects) correctly
- Fine-grained control over when components update
- Improved performance for lists and frequently updated data
- Reduced memory allocations from avoiding re-renders

**useCallback for Stable Function References:**

```typescript
// ✅ Good: useCallback with proper dependencies
export const TaskBoard = memo(function TaskBoard({ tasks, onTaskClick }: TaskBoardProps) {
  // Stable handler with useCallback
  const handleCardClick = useCallback((task: Task) => {
    onTaskClick?.(task)
  }, [onTaskClick])

  // Create stable click handlers for each task
  const createClickHandler = useCallback((task: Task) => () => {
    handleCardClick(task)
  }, [handleCardClick])

  return (
    <div>
      {tasks.map((task) => (
        <TaskCard key={task.id} task={task} onClick={createClickHandler(task)} />
      ))}
    </div>
  )
})

// ✅ Good: useCallback for event handlers passed to memoized components
function TaskList({ tasks }: TaskListProps) {
  const dispatch = useTaskDispatch()

  const handleSelect = useCallback((id: string) => {
    dispatch({ type: "SELECT_TASK", payload: id })
  }, [dispatch])

  return (
    <div>
      {tasks.map((task) => (
        <TaskRow key={task.id} task={task} onSelect={handleSelect} />
      ))}
    </div>
  )
}

// ❌ Bad: Not using useCallback, creating new functions on every render
function TaskBoard({ tasks, onTaskClick }: TaskBoardProps) {
  return (
    <div>
      {tasks.map((task) => (
        <TaskCard
          key={task.id}
          task={task}
          // New function created on every render - breaks memoization
          onClick={() => onTaskClick?.(task)}
        />
      ))}
    </div>
  )
}
```

**useMemo for Expensive Computations:**

```typescript
// ✅ Good: useMemo for derived state and expensive computations
export const TaskBoard = memo(function TaskBoard({ tasks }: TaskBoardProps) {
  // Group tasks by status - only recompute when tasks change
  const tasksByStatus = useMemo(() => {
    const result: Record<string, Task[]> = {
      todo: [],
      inProgress: [],
      complete: [],
      overdue: [],
    }
    for (const task of tasks) {
      if (result[task.status]) {
        result[task.status].push(task)
      }
    }
    return result
  }, [tasks])

  // Memoize derived values
  const totalTasks = useMemo(() => tasks.length, [tasks])

  return (
    <div>
      <div aria-live="polite">{totalTasks} tasks loaded</div>
      {Object.entries(tasksByStatus).map(([status, tasks]) => (
        <BoardColumn key={status} title={status} tasks={tasks} />
      ))}
    </div>
  )
})

// ❌ Bad: Computing on every render
export const TaskBoard = function TaskBoard({ tasks }: TaskBoardProps) {
  // Re-computed on every render, even if tasks haven't changed
  const tasksByStatus = {
    todo: tasks.filter(t => t.status === "todo"),
    inProgress: tasks.filter(t => t.status === "inProgress"),
    // ...
  }

  return <div>{/* ... */}</div>
}
```

**aria-live Regions for Accessibility:**

```typescript
// ✅ Good: Using aria-live for dynamic content announcements
export const TaskBoard = memo(function TaskBoard({ tasks }: TaskBoardProps) {
  const totalTasks = useMemo(() => tasks.length, [tasks])

  return (
    <BoardLayout>
      {/* Screen reader announcement for task count changes */}
      <div aria-live="polite" aria-atomic="true" className="sr-only">
        {totalTasks} tasks loaded
      </div>

      {/* Board columns */}
    </BoardLayout>
  )
})

// ✅ Good: aria-live with assertive for critical state changes
export const TaskModal = memo(function TaskModal({ open, mode }: TaskModalProps) {
  return (
    <Dialog open={open}>
      {/* Assertive announcement for dialog state */}
      <div aria-live="assertive" aria-atomic="true" className="sr-only">
        {open ? (mode === "create" ? "Create task dialog opened" : "Edit task dialog opened") : ""}
      </div>

      <DialogContent>
        {/* Modal content */}
      </DialogContent>
    </Dialog>
  )
})

// aria-live values:
// - "polite": Announces when user is idle (use for status updates, counts)
// - "assertive": Announces immediately (use for errors, critical state changes)
// - "off": Does not announce (default)
// aria-atomic="true": Announces entire region as one message

// ❌ Bad: Missing screen reader announcements
export const TaskBoard = function TaskBoard({ tasks }: TaskBoardProps) {
  return (
    <div>
      {/* Screen readers won't announce task count changes */}
      {tasks.map((task) => <TaskCard key={task.id} task={task} />)}
    </div>
  )
}
```

**Accessibility Best Practices:**

```typescript
// ✅ Good: Proper aria-labels for interactive elements
<button
  onClick={() => onOpenChange?.(false)}
  aria-label="Close dialog"
  className="close-button"
>
  <X className="h-5 w-5" />
</button>

// ✅ Good: Descriptive link/button text with aria-label
<button
  aria-label="Drag to reorder task"
  className="drag-handle"
>
  <GripVertical className="h-4 w-4" />
</button>

// ❌ Bad: Icon-only buttons without aria-label
<button onClick={onClose}>
  <X className="h-5 w-5" />
  {/* Screen reader says "button" with no context */}
</button>
```

**Performance Optimization Checklist:**

- [ ] Wrap list items in `React.memo` with custom comparison functions
- [ ] Use `useCallback` for event handlers passed to memoized children
- [ ] Use `useMemo` for expensive computations and derived state
- [ ] Add `aria-live` regions for dynamic content announcements
- [ ] Provide `aria-label` for icon-only buttons and interactive elements
- [ ] Use `aria-atomic="true"` for complete region announcements
- [ ] Choose appropriate `aria-live` politeness level (polite vs assertive)
- [ ] Test with screen readers to verify announcements work correctly

### TypeScript Code Style

**Type Definitions:**

```typescript
// ✅ Good: Explicit types, interfaces for public APIs
export interface Task {
  id: string;
  title: string;
  description?: string;
  status: TaskStatus;
  assignee?: User;
  createdAt: Date;
  updatedAt: Date;
}

export interface TaskStatus {
  id: string;
  name: string;
  color: string;
}

export type TaskPriority = 'low' | 'medium' | 'high' | 'urgent';

// ❌ Bad: Using `any`, implicit types
export function processTask(task: any) {
  return task.map((t: any) => t.name);
}
```

**Component Structure:**

```typescript
// ✅ Good: Functional component with hooks, proper types
interface TaskCardProps {
  task: Task;
  onEdit: (task: Task) => void;
  onDelete: (id: string) => void;
}

export function TaskCard({ task, onEdit, onDelete }: TaskCardProps) {
  const [isEditing, setIsEditing] = useState(false);
  const { data: assignee } = useQuery({
    queryKey: ['user', task.assigneeId],
    queryFn: () => api.getUser(task.assigneeId),
    enabled: !!task.assigneeId,
  });

  if (isEditing) {
    return <TaskEditForm task={task} onSave={onEdit} onCancel={() => setIsEditing(false)} />;
  }

  return (
    <div className="task-card">
      <h3>{task.title}</h3>
      {/* ... */}
    </div>
  );
}

// ❌ Bad: Class component, missing types
export class TaskCard extends React.Component {
  render() {
    const task = this.props.task;
    return <div>{task.title}</div>;
  }
}
```

**Shared Constants Pattern:**

```typescript
// ✅ Good: Centralized constants file for shared values
// src/components/tasks/constants.ts
import { TASK_STATUSES, TASK_PRIORITIES, BOARD_COLUMNS } from "@/components/tasks/constants";

// Type-safe with `as const` assertion
export const TASK_STATUSES = [
  { value: "todo", label: "To Do" },
  { value: "inProgress", label: "In Progress" },
  { value: "complete", label: "Complete" },
  { value: "overdue", label: "Overdue" },
] as const;

export const TASK_PRIORITIES = [
  { value: "urgent", label: "Urgent" },
  { value: "high", label: "High" },
  { value: "medium", label: "Medium" },
  { value: "low", label: "Low" },
] as const;

export const BOARD_COLUMNS = [
  { id: "todo", title: "To Do" },
  { id: "inProgress", title: "In Progress" },
  { id: "complete", title: "Complete" },
  { id: "overdue", title: "Overdue" },
] as const;

// Usage in components
import { TASK_STATUSES, TASK_PRIORITIES } from "@/components/tasks/constants";

function TaskFilter() {
  return (
    <Select>
      {TASK_STATUSES.map((status) => (
        <SelectItem key={status.value} value={status.value}>
          {status.label}
        </SelectItem>
      ))}
    </Select>
  );
}

// ❌ Bad: Hardcoded values scattered across components
function TaskFilter() {
  return (
    <Select>
      <SelectItem value="todo">To Do</SelectItem>
      <SelectItem value="inProgress">In Progress</SelectItem>
      <SelectItem value="complete">Complete</SelectItem>
      <SelectItem value="overdue">Overdue</SelectItem>
      {/* Duplicated in multiple components */}
    </Select>
  );
}
```

**Benefits of Shared Constants:**

- Single source of truth prevents inconsistencies
- Type-safe with `as const` for readonly arrays
- Easier to maintain and update
- Reduces duplication across components
- Prevents typos in string literals
- Improves IDE autocomplete suggestions

**State Management:**

```typescript
// ✅ Good: Using Zustand with proper typing
interface TaskStore {
  tasks: Task[];
  selectedTask: Task | null;
  fetchTasks: () => Promise<void>;
  selectTask: (id: string) => void;
}

export const useTaskStore = create<TaskStore>((set, get) => ({
  tasks: [],
  selectedTask: null,
  fetchTasks: async () => {
    const tasks = await api.getTasks();
    set({ tasks });
  },
  selectTask: (id) => {
    const task = get().tasks.find(t => t.id === id);
    set({ selectedTask: task ?? null });
  },
}));

// ❌ Bad: Prop drilling, no state management
function App() {
  const [tasks, setTasks] = useState<Task[]>([]);
  return (
    <TaskList tasks={tasks} setTasks={setTasks} />
  );
}
```

### React Best Practices

**Component Naming:**

- **Components:** PascalCase (e.g., `TaskCard`, `UserAvatar`, `LoginPage`)
- **Hooks:** camelCase with `use` prefix (e.g., `useTaskData`, `useUserContext`)
- **Utilities:** camelCase (e.g., `formatDate`, `calculateProgress`)
- **Types/Interfaces:** PascalCase (e.g., `TaskProps`, `ApiResponse`)

**File Structure:**

```
src/features/tasks/
├── components/
│   ├── TaskCard.tsx
│   ├── TaskList.tsx
│   └── TaskForm.tsx
├── hooks/
│   ├── useTasks.ts
│   └── useTaskMutation.ts
├── api/
│   └── tasks-api.ts
├── types/
│   └── task.types.ts
└── index.ts
```

**React Hooks Usage:**

```typescript
// ✅ Good: Custom hooks, proper dependencies
function useTaskData(taskId: string) {
  const query = useQuery({
    queryKey: ['task', taskId],
    queryFn: () => api.getTask(taskId),
    enabled: !!taskId,
  });

  return query;
}

// Component using the hook
function TaskDetail({ taskId }: { taskId: string }) {
  const { data: task, isLoading, error } = useTaskData(taskId);

  if (isLoading) return <Skeleton />;
  if (error) return <Error message={error.message} />;
  return <TaskCard task={task} />;
}

// ❌ Bad: Logic in component, missing dependencies
function TaskDetail({ taskId }: { taskId: string }) {
  const [task, setTask] = useState<Task | null>(null);

  useEffect(() => {
    api.getTask(taskId).then(setTask);
    // Missing: taskId dependency, error handling
  }, []);

  if (!task) return <div>Loading...</div>;
  return <TaskCard task={task} />;
}
```

## UI Component Standards

### Component Organization

**Feature-Based Structure:**

```typescript
// ✅ Good: Feature-based component organization
src/features/tasks/
├── components/
│   ├── TaskCard.tsx
│   ├── TaskList.tsx
│   └── TaskForm.tsx
├── hooks/
│   ├── useTasks.ts
│   └── useTaskMutation.ts
├── api/
│   └── tasks-api.ts
├── types/
│   └── task.types.ts
└── index.ts

// ❌ Bad: Flat structure without grouping
src/components/
├── TaskCard.tsx
├── TaskList.tsx
├── UserCard.tsx
├── UserList.tsx
└── ...
```

### Radix UI Component Patterns

**Dialog Component:**

```typescript
// ✅ Good: Proper Dialog usage with controlled state
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogDescription, DialogFooter } from "@/components/ui/dialog"

function TaskModal({ task, open, onOpenChange, onSubmit }: TaskModalProps) {
  const [formData, setFormData] = useState<Partial<Task>>(task || {})

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    onSubmit(formData)
    onOpenChange(false)
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-[425px]">
        <DialogHeader>
          <DialogTitle>{task ? "Edit Task" : "Create Task"}</DialogTitle>
          <DialogDescription>
            {task ? "Update the task details below." : "Enter the task details below."}
          </DialogDescription>
        </DialogHeader>
        <form onSubmit={handleSubmit}>
          {/* Form fields */}
        </form>
        <DialogFooter>
          <Button type="submit">Save</Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}

// ❌ Bad: Uncontrolled state, missing accessibility
function TaskModal() {
  const [open, setOpen] = useState(false)
  return (
    <Dialog open={open}>
      <DialogContent>
        <h2>Create Task</h2>
        {/* Missing DialogHeader, DialogTitle, DialogDescription */}
      </DialogContent>
    </Dialog>
  )
}
```

**Select Component:**

```typescript
// ✅ Good: Controlled Select with proper typing
import { Select, SelectTrigger, SelectValue, SelectContent, SelectItem } from "@/components/ui/select"

function TaskFilter({ value, onChange }: { value: TaskStatus; onChange: (status: TaskStatus) => void }) {
  return (
    <Select value={value} onValueChange={onChange}>
      <SelectTrigger className="w-[180px]">
        <SelectValue placeholder="Select status" />
      </SelectTrigger>
      <SelectContent>
        <SelectItem value="todo">To Do</SelectItem>
        <SelectItem value="inProgress">In Progress</SelectItem>
        <SelectItem value="complete">Complete</SelectItem>
        <SelectItem value="overdue">Overdue</SelectItem>
      </SelectContent>
    </Select>
  )
}

// ❌ Bad: Uncontrolled, improper value types
function TaskFilter() {
  return (
    <Select>
      <SelectTrigger>
        <SelectValue />
      </SelectTrigger>
      <SelectContent>
        <SelectItem value={1}>To Do</SelectItem> {/* Wrong type */}
      </SelectContent>
    </Select>
  )
}
```

**Checkbox Component:**

```typescript
// ✅ Good: Controlled checkbox with proper form integration
import { Checkbox } from "@/components/ui/checkbox"

function TaskRow({ task, selected, onSelectChange }: TaskRowProps) {
  return (
    <div className="flex items-center space-x-2">
      <Checkbox
        id={task.id}
        checked={selected}
        onCheckedChange={(checked) => onSelectChange(checked === true)}
      />
      <label htmlFor={task.id} className="text-sm font-medium">
        {task.title}
      </label>
    </div>
  )
}

// ❌ Bad: Uncontrolled, missing label association
function TaskRow({ task }: { task: Task }) {
  return (
    <div className="flex items-center space-x-2">
      <Checkbox id={task.id} />
      <span className="text-sm">{task.title}</span>
    </div>
  )
}
```

**Table Component:**

```typescript
// ✅ Good: Semantic table structure with proper headers
import { Table, TableHeader, TableBody, TableFooter, TableRow, TableHead, TableCell } from "@/components/ui/table"

function TaskList({ tasks }: { tasks: Task[] }) {
  return (
    <Table>
      <TableHeader>
        <TableRow>
          <TableHead className="w-[100px]">Title</TableHead>
          <TableHead>Status</TableHead>
          <TableHead>Priority</TableHead>
          <TableHead className="text-right">Actions</TableHead>
        </TableRow>
      </TableHeader>
      <TableBody>
        {tasks.map((task) => (
          <TableRow key={task.id}>
            <TableCell className="font-medium">{task.title}</TableCell>
            <TableCell><Badge status={task.status}>{task.status}</Badge></TableCell>
            <TableCell><Badge priority={task.priority}>{task.priority}</Badge></TableCell>
            <TableCell className="text-right">
              <Button variant="ghost" size="sm">Edit</Button>
            </TableCell>
          </TableRow>
        ))}
      </TableBody>
    </Table>
  )
}

// ❌ Bad: Div soup, not semantic, missing accessibility
function TaskList({ tasks }: { tasks: Task[] }) {
  return (
    <div>
      <div className="header">
        <div>Title</div>
        <div>Status</div>
        <div>Priority</div>
      </div>
      {tasks.map((task) => (
        <div key={task.id}>
          <div>{task.title}</div>
          <div>{task.status}</div>
          <div>{task.priority}</div>
        </div>
      ))}
    </div>
  )
}
```

### TanStack Table Patterns

**Column Definition:**

```typescript
// ✅ Good: Proper column definition with TypeScript typing
import { ColumnDef } from "@tanstack/react-table"
import { Task } from "@/features/tasks/types"

export const taskColumns: ColumnDef<Task>[] = [
  {
    accessorKey: "title",
    header: "Title",
    cell: ({ row }) => (
      <div className="font-medium">{row.getValue("title")}</div>
    ),
  },
  {
    accessorKey: "status",
    header: "Status",
    cell: ({ row }) => {
      const status = row.getValue("status") as TaskStatus
      return <Badge status={status}>{status}</Badge>
    },
  },
  {
    accessorKey: "priority",
    header: "Priority",
    cell: ({ row }) => {
      const priority = row.getValue("priority") as TaskPriority
      return <Badge priority={priority}>{priority}</Badge>
    },
  },
  {
    id: "actions",
    cell: ({ row }) => (
      <DropdownMenu>
        <DropdownMenuTrigger asChild>
          <Button variant="ghost" size="sm">•••</Button>
        </DropdownMenuTrigger>
        <DropdownMenuContent>
          <DropdownMenuItem onClick={() => onEdit(row.original)}>
            Edit
          </DropdownMenuItem>
          <DropdownMenuItem onClick={() => onDelete(row.original.id)}>
            Delete
          </DropdownMenuItem>
        </DropdownMenuContent>
      </DropdownMenu>
    ),
  },
]

// ❌ Bad: Loose typing, no accessorKey, poor cell rendering
const columns = [
  {
    header: "Title",
    cell: (info) => info.getValue(), // No typing
  },
  {
    id: "status",
    cell: (row) => <span>{row.status}</span>, // Incorrect API usage
  },
]
```

**Table Usage:**

```typescript
// ✅ Good: Proper table setup with sorting and pagination
import { useReactTable, getCoreRowModel, getSortedRowModel, getPaginationRowModel } from "@tanstack/react-table"

function TaskTable({ tasks }: { tasks: Task[] }) {
  const [sorting, setSorting] = useState<SortingState>([])
  const [pagination, setPagination] = useState({ pageIndex: 0, pageSize: 10 })

  const table = useReactTable({
    data: tasks,
    columns: taskColumns,
    getCoreRowModel: getCoreRowModel(),
    getSortedRowModel: getSortedRowModel(),
    getPaginationRowModel: getPaginationRowModel(),
    onSortingChange: setSorting,
    onPaginationChange: setPagination,
    state: {
      sorting,
      pagination,
    },
  })

  return (
    <div className="rounded-md border">
      <Table>
        <TableHeader>
          {table.getHeaderGroups().map((headerGroup) => (
            <TableRow key={headerGroup.id}>
              {headerGroup.headers.map((header) => (
                <TableHead key={header.id}>
                  {header.isPlaceholder
                    ? null
                    : flexRender(
                        header.column.columnDef.header,
                        header.getContext()
                      )}
                </TableHead>
              ))}
            </TableRow>
          ))}
        </TableHeader>
        <TableBody>
          {table.getRowModel().rows?.length ? (
            table.getRowModel().rows.map((row) => (
              <TableRow key={row.id}>
                {row.getVisibleCells().map((cell) => (
                  <TableCell key={cell.id}>
                    {flexRender(cell.column.columnDef.cell, cell.getContext())}
                  </TableCell>
                ))}
              </TableRow>
            ))
          ) : (
            <TableRow>
              <TableCell colSpan={taskColumns.length} className="h-24 text-center">
                No results.
              </TableCell>
            </TableRow>
          )}
        </TableBody>
      </Table>
    </div>
  )
}

// ❌ Bad: Manual table rendering, missing table features
function TaskTable({ tasks }: { tasks: Task[] }) {
  return (
    <table>
      <thead>
        <tr>
          <th>Title</th>
          <th>Status</th>
        </tr>
      </thead>
      <tbody>
        {tasks.map((task) => (
          <tr key={task.id}>
            <td>{task.title}</td>
            <td>{task.status}</td>
          </tr>
        ))}
      </tbody>
    </table>
  )
}
```

### Drag-and-Drop Patterns (@dnd-kit)

**Board View Implementation:**

```typescript
// ✅ Good: Proper drag-and-drop setup with @dnd-kit
import { DndContext, closestCenter, KeyboardSensor, PointerSensor, useSensor, useSensors } from "@dnd-kit/core"
import { SortableContext, sortableKeyboardCoordinates, verticalListSortingStrategy } from "@dnd-kit/sortable"
import { useSortable } from "@dnd-kit/sortable"
import { CSS } from "@dnd-kit/utilities"

function SortableTask({ task }: { task: Task }) {
  const { attributes, listeners, setNodeRef, transform, transition, isDragging } = useSortable({
    id: task.id,
  })

  const style = {
    transform: CSS.Transform.toString(transform),
    transition,
    opacity: isDragging ? 0.5 : 1,
  }

  return (
    <div ref={setNodeRef} style={style} {...attributes} {...listeners}>
      <TaskCard task={task} />
    </div>
  )
}

function TaskBoard({ tasks }: { tasks: Task[] }) {
  const sensors = useSensors(
    useSensor(PointerSensor),
    useSensor(KeyboardSensor, {
      coordinateGetter: sortableKeyboardCoordinates,
    })
  )

  const handleDragEnd = (event: DragEndEvent) => {
    const { active, over } = event
    if (over && active.id !== over.id) {
      // Update task order
    }
  }

  return (
    <DndContext sensors={sensors} collisionDetection={closestCenter} onDragEnd={handleDragEnd}>
      <SortableContext items={tasks.map((t) => t.id)} strategy={verticalListSortingStrategy}>
        {tasks.map((task) => (
          <SortableTask key={task.id} task={task} />
        ))}
      </SortableContext>
    </DndContext>
  )
}

// ❌ Bad: Missing accessibility, no keyboard support
function TaskBoard({ tasks }: { tasks: Task[] }) {
  const handleDragStart = (e: DragEvent) => {
    // Native HTML5 drag API (less accessible)
  }

  return (
    <div>
      {tasks.map((task) => (
        <div key={task.id} draggable onDragStart={handleDragStart}>
          <TaskCard task={task} />
        </div>
      ))}
    </div>
  )
}
```

### State Management Patterns

**Local Component State:**

```typescript
// ✅ Good: useState for simple local state
function TaskToolbar() {
  const [filter, setFilter] = useState<TaskFilter>({})
  const [view, setView] = useState<"list" | "board">("list")

  return (
    <div className="flex items-center justify-between">
      <Select value={filter.status} onValueChange={(status) => setFilter({ ...filter, status })}>
        {/* Filter options */}
      </Select>
      <Button onClick={() => setView(view === "list" ? "board" : "list")}>
        Toggle View
      </Button>
    </div>
  )
}

// ❌ Bad: Overusing global state for local component state
function TaskToolbar() {
  const { filter, setFilter } = useTaskStore() // Unnecessary global state
  const { view, setView } = useViewStore() // Unnecessary global state

  return (
    <div>
      {/* Component logic */}
    </div>
  )
}
```

**Form State:**

```typescript
// ✅ Good: Controlled form with proper validation
function TaskModal({ task, open, onOpenChange, onSubmit }: TaskModalProps) {
  const [formData, setFormData] = useState<Partial<Task>>(task || {
    title: "",
    description: "",
    status: "todo",
    priority: "medium",
  })

  const [errors, setErrors] = useState<Record<string, string>>({})

  const handleChange = (field: keyof Task, value: string) => {
    setFormData((prev) => ({ ...prev, [field]: value }))
    // Clear error when user starts typing
    if (errors[field]) {
      setErrors((prev) => ({ ...prev, [field]: "" }))
    }
  }

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()

    // Validation
    const newErrors: Record<string, string> = {}
    if (!formData.title) {
      newErrors.title = "Title is required"
    }
    if (formData.title && formData.title.length > 200) {
      newErrors.title = "Title must be less than 200 characters"
    }

    if (Object.keys(newErrors).length > 0) {
      setErrors(newErrors)
      return
    }

    onSubmit(formData)
  }

  return (
    <form onSubmit={handleSubmit}>
      <Input
        value={formData.title}
        onChange={(e) => handleChange("title", e.target.value)}
        error={!!errors.title}
      />
      {errors.title && <p className="text-sm text-error">{errors.title}</p>}
      {/* Other form fields */}
      <Button type="submit">Save</Button>
    </form>
  )
}

// ❌ Bad: Uncontrolled form, missing validation
function TaskModal({ onSubmit }: TaskModalProps) {
  return (
    <form onSubmit={(e) => {
      e.preventDefault()
      const formData = new FormData(e.currentTarget)
      onSubmit(Object.fromEntries(formData))
    }}>
      <Input name="title" />
      <Input name="description" />
      <Button type="submit">Save</Button>
    </form>
  )
}
```

## Database Standards

### Entity Framework Core Conventions

**Configuration Files:**

```csharp
// ✅ Good: Separate configuration class
public class TaskConfiguration : IEntityTypeConfiguration<Task>
{
    public void Configure(EntityTypeBuilder<Task> builder)
    {
        builder.ToTable("Tasks");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasOne(t => t.Project)
            .WithMany(p => p.Tasks)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(t => t.ProjectId);
        builder.HasIndex(t => t.StatusId);
        builder.HasIndex(t => t.AssigneeId);
    }
}

// ❌ Bad: Configuration in OnModelCreating
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Task>(entity =>
    {
        entity.ToTable("Tasks");
        entity.HasKey(e => e.Id);
        // ... 50 more lines of configuration
    });
}
```

**Migration Standards:**

```csharp
// ✅ Good: Descriptive migration name, up/down methods
public class AddDocumentTables : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Pages",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Title = table.Column<string>(type: "text", nullable: false),
                // ...
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Pages", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable("Pages");
    }
}

// ❌ Bad: Non-descriptive name, missing down method
public class Migration1 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Changes
    }
}
```

## API Standards

### Endpoint Design

**Minimal API Pattern:**

```csharp
// ✅ Good: Grouped endpoints, route handlers, proper HTTP methods
app.MapGroup("/api/tasks")
    .MapTasksEndpoints()
    .RequireAuthorization()
    .WithTags("Tasks");

public static class TaskEndpoints
{
    public static RouteGroupBuilder MapTasksEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/", async (IMediator mediator, [AsParameters] GetTasksQuery query) =>
        {
            var result = await mediator.Send(query);
            return Results.Ok(result);
        })
        .RequirePermission("tasks", "read");

        group.MapPost("/", async (IMediator mediator, CreateTaskCommand command) =>
        {
            var result = await mediator.Send(command);
            return Results.Created($"/api/tasks/{result.Value.Id}", result);
        })
        .RequirePermission("tasks", "create");

        return group;
    }
}

// ❌ Bad: Controller-based, inline logic
[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll()
    {
        var tasks = _dbContext.Tasks.ToList();
        return Ok(tasks);
    }
}
```

**Response Format:**

```csharp
// ✅ Good: Consistent API response wrapper
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = new();
}

// Usage
return Ok(new ApiResponse<TaskDto>
{
    Success = true,
    Message = "Task created successfully",
    Data = taskDto
});

// ❌ Bad: Inconsistent responses
return Ok(new { data = task, status = "ok", message = "success" });
return Ok(task);
return BadRequest(new { error = "Invalid data" });
```

## Testing Standards

### Unit Testing

```csharp
// ✅ Good: Arrange-Act-Assert pattern, descriptive names
[Fact]
public async Task CreateTask_WithValidData_ReturnsSuccess()
{
    // Arrange
    var command = new CreateTaskCommand { Title = "Test Task" };
    var handler = new CreateTaskCommandHandler(_dbContext, _userContext);

    // Act
    var result = await handler.Handle(command, CancellationToken.None);

    // Assert
    Assert.True(result.IsSuccess);
    Assert.NotNull(result.Value);
    Assert.Equal("Test Task", result.Value.Title);
}

// ❌ Bad: Unclear test, no assertions
[Fact]
public async Task Test1()
{
    var command = new CreateTaskCommand { Title = "Test" };
    var handler = new CreateTaskCommandHandler(_dbContext, _userContext);
    var result = await handler.Handle(command, CancellationToken.None);
    // Missing assertions
}
```

### Integration Testing

```csharp
// ✅ Good: API endpoint testing with WebApplicationFactory
public class TaskEndpointsTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;

    public TaskEndpointsTests(ApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateTask_ReturnsCreated()
    {
        // Arrange
        var command = new CreateTaskCommand { Title = "Test Task" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/tasks", command);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}
```

## Documentation Standards

### XML Documentation Comments

```csharp
// ✅ Good: Complete XML documentation
/// <summary>
/// Creates a new task in the system.
/// </summary>
/// <param name="request">The task creation request containing title, description, and metadata.</param>
/// <returns>A result containing the created task DTO or an error message.</returns>
/// <remarks>
/// The user must have "tasks:create" permission.
/// The task will be associated with the current user's workspace.
/// </remarks>
[HttpPost]
public async Task<IActionResult> CreateTask([FromBody] CreateTaskRequest request)
{
    // Implementation
}

// ❌ Bad: No documentation
[HttpPost]
public IActionResult CreateTask([FromBody] CreateTaskRequest request)
{
    // Implementation
}
```

### Code Comments

```csharp
// ✅ Good: Explain "why", not "what"
// Using RLS (Row-Level Security) to ensure users can only access tasks in their workspaces
// This filter is applied at the database level for security
var tasks = await _dbContext.Tasks
    .Where(t => t.Project.WorkspaceId == _userContext.CurrentWorkspaceId)
    .ToListAsync();

// ❌ Bad: Stating the obvious
// Get all tasks from database
var tasks = await _dbContext.Tasks.ToListAsync();
```

## Git Workflow

### Commit Message Format

```
<type>(<scope>): <subject>

<body>

<footer>
```

**Types:**

- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation changes
- `style`: Code style changes (formatting, no logic change)
- `refactor`: Code refactoring
- `test`: Adding or updating tests
- `chore`: Build process or auxiliary tool changes

**Examples:**

```
feat(tasks): add task status drag-and-drop in board view

Implement drag-and-drop functionality for task status updates
using @dnd-kit library. Users can now drag tasks between
status columns in the board view.

Closes #123
```

```
fix(auth): resolve JWT token validation issue on refresh

Fixed incorrect audience validation in JWT refresh flow.
Previously, tokens were being rejected due to case-sensitive
audience comparison.

Fixes #456
```

### Branch Naming

- `feature/` - New features (e.g., `feature/document-wiki`)
- `fix/` - Bug fixes (e.g., `fix/authentication-error`)
- `hotfix/` - Production hotfixes (e.g., `hotfix/security-patch`)
- `refactor/` - Refactoring (e.g., `refactor/cleanup-task-queries`)
- `docs/` - Documentation (e.g., `docs/update-api-guide`)

## Code Review Guidelines

### Review Checklist

**Functionality:**

- [ ] Code implements the requirements
- [ ] Edge cases are handled
- [ ] Error handling is appropriate
- [ ] Tests cover critical paths

**Code Quality:**

- [ ] Code follows project standards
- [ ] Naming is clear and consistent
- [ ] No code duplication
- [ ] Proper separation of concerns

**Performance:**

- [ ] No N+1 query problems
- [ ] Efficient database queries
- [ ] Proper indexing
- [ ] No memory leaks

**Security:**

- [ ] Input validation
- [ ] SQL injection prevention
- [ ] XSS prevention
- [ ] Proper authorization

### Review Process

1. **Self-Review:** Review your own code before submitting
2. **Keep Changes Small:** <400 lines per PR ideal
3. **Clear Description:** Explain what and why
4. **Respond to Feedback:** Address all review comments
5. **Approval Required:** At least one approval before merge

---

**Document Version:** 1.3
**Last Updated:** 2026-01-06
**Maintained By:** Development Team
**Review Frequency:** Quarterly
**Next Review:** 2026-04-06
