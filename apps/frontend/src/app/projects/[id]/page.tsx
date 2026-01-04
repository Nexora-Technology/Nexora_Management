"use client";

import { ViewSwitcher } from "@/features/views/ViewSwitcher";
import { ViewLayout } from "@/features/views/ViewLayout";
import { ViewProvider } from "@/features/views/ViewContext";
import { use } from "react";

export default function ProjectPage({ params }: { params: Promise<{ id: string }> }) {
  const { id: projectId } = use(params);

  return (
    <ViewProvider>
      <div className="min-h-screen bg-gradient-to-br from-sky-50 via-white to-teal-50 dark:from-slate-900 dark:via-slate-800 dark:to-slate-900">
        <div className="container mx-auto px-4 py-8">
          <div className="mb-8">
            <a href="/workspaces/1/projects" className="text-sky-600 hover:text-sky-700 dark:text-sky-400">
              ← Back to Projects
            </a>
            <h1 className="mt-4 text-4xl font-bold text-slate-900 dark:text-slate-50">Project View</h1>
            <p className="text-slate-600 dark:text-slate-400">Project ID: {projectId}</p>
          </div>

          <ViewSwitcher />

          <div className="mt-6">
            <ViewLayout projectId={projectId} />
          </div>
        </div>
      </div>
    </ViewProvider>
  );
}
