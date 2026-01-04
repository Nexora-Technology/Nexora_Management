import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import Link from "next/link";

// Mock project data - TODO: Replace with API call
const mockProjects = [
  {
    id: "1",
    name: "Q1 Planning",
    description: "Q1 2026 planning and roadmap",
    status: "Active",
    taskCount: 24,
    completedCount: 18,
  },
  {
    id: "2",
    name: "Website Redesign",
    description: "Complete website overhaul",
    status: "In Progress",
    taskCount: 42,
    completedCount: 15,
  },
];

export default async function ProjectsPage({ params }: { params: Promise<{ id: string }> }) {
  const { id: workspaceId } = await params;

  return (
    <div className="min-h-screen bg-gradient-to-br from-sky-50 via-white to-teal-50 dark:from-slate-900 dark:via-slate-800 dark:to-slate-900">
      <div className="container mx-auto px-4 py-8">
        <div className="mb-8">
          <Link href="/workspaces" className="text-sky-600 hover:text-sky-700 dark:text-sky-400">
            ← Back to Workspaces
          </Link>
          <h1 className="mt-4 text-4xl font-bold text-slate-900 dark:text-slate-50">Projects</h1>
          <p className="text-slate-600 dark:text-slate-400">Workspace ID: {workspaceId}</p>
        </div>

        <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-3">
          {mockProjects.map((project) => (
            <Link key={project.id} href={`/projects/${project.id}`}>
              <Card className="hover:shadow-lg transition-shadow cursor-pointer h-full">
                <CardHeader>
                  <div className="flex items-start justify-between">
                    <div>
                      <CardTitle>{project.name}</CardTitle>
                      <CardDescription>{project.description}</CardDescription>
                    </div>
                    <span className="px-2 py-1 text-xs rounded-full bg-sky-100 text-sky-700 dark:bg-sky-900 dark:text-sky-300">
                      {project.status}
                    </span>
                  </div>
                </CardHeader>
                <CardContent>
                  <div className="space-y-2">
                    <div className="flex items-center justify-between text-sm">
                      <span className="text-slate-600 dark:text-slate-400">Progress</span>
                      <span className="font-medium">
                        {project.completedCount}/{project.taskCount}
                      </span>
                    </div>
                    <div className="w-full bg-slate-200 rounded-full h-2">
                      <div
                        className="bg-sky-500 h-2 rounded-full"
                        style={{
                          width: `${(project.completedCount / project.taskCount) * 100}%`,
                        }}
                      />
                    </div>
                  </div>
                </CardContent>
              </Card>
            </Link>
          ))}
        </div>

        {mockProjects.length === 0 && (
          <Card className="text-center py-12">
            <CardContent>
              <p className="text-slate-600 dark:text-slate-400 mb-4">No projects yet</p>
              <Button>Create your first project</Button>
            </CardContent>
          </Card>
        )}
      </div>
    </div>
  );
}
