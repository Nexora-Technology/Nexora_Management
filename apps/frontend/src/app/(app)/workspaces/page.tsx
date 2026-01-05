import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import Link from "next/link";
import type { Route } from "next";

// Mock workspace data - TODO: Replace with API call
const mockWorkspaces = [
  {
    id: "1" as const,
    name: "Engineering",
    description: "Product development workspace",
    projectCount: 5,
    memberCount: 12,
  },
  {
    id: "2" as const,
    name: "Marketing",
    description: "Marketing campaigns and content",
    projectCount: 3,
    memberCount: 8,
  },
];

export default function WorkspacesPage() {
  return (
    <div className="min-h-screen bg-gradient-to-br from-sky-50 via-white to-teal-50 dark:from-slate-900 dark:via-slate-800 dark:to-slate-900">
      <div className="container mx-auto px-4 py-8">
        <div className="mb-8 flex items-center justify-between">
          <div>
            <h1 className="text-4xl font-bold text-slate-900 dark:text-slate-50">Workspaces</h1>
            <p className="text-slate-600 dark:text-slate-400">Manage your team workspaces</p>
          </div>
          <Button>Create Workspace</Button>
        </div>

        <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-3">
          {mockWorkspaces.map((workspace) => (
            <Link key={workspace.id} href={`/workspaces/${workspace.id}` as Route}>
              <Card className="hover:shadow-lg transition-shadow cursor-pointer h-full">
                <CardHeader>
                  <CardTitle>{workspace.name}</CardTitle>
                  <CardDescription>{workspace.description}</CardDescription>
                </CardHeader>
                <CardContent>
                  <div className="flex gap-4 text-sm text-slate-600 dark:text-slate-400">
                    <span>{workspace.projectCount} projects</span>
                    <span>{workspace.memberCount} members</span>
                  </div>
                </CardContent>
              </Card>
            </Link>
          ))}
        </div>

        {mockWorkspaces.length === 0 && (
          <Card className="text-center py-12">
            <CardContent>
              <p className="text-slate-600 dark:text-slate-400 mb-4">No workspaces yet</p>
              <Button>Create your first workspace</Button>
            </CardContent>
          </Card>
        )}
      </div>
    </div>
  );
}
