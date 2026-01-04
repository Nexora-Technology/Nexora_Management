"use client";

import { useRouter } from "next/navigation";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";

export default function DashboardPage() {
  const router = useRouter();

  // TODO: Fetch actual workspaces from API
  // For now, show a simple dashboard
  return (
    <div className="min-h-screen bg-gradient-to-br from-sky-50 via-white to-teal-50 dark:from-slate-900 dark:via-slate-800 dark:to-slate-900">
      <div className="container mx-auto px-4 py-8">
        <div className="mb-8">
          <h1 className="text-4xl font-bold text-slate-900 dark:text-slate-50">Dashboard</h1>
          <p className="text-slate-600 dark:text-slate-400">Welcome to Nexora Management Platform</p>
        </div>

        <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-3">
          <Card className="hover:shadow-lg transition-shadow cursor-pointer">
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <span className="text-2xl">📋</span>
                Workspaces
              </CardTitle>
              <CardDescription>Your team workspaces</CardDescription>
            </CardHeader>
            <CardContent>
              <Button onClick={() => router.push("/workspaces")}>View Workspaces</Button>
            </CardContent>
          </Card>

          <Card className="hover:shadow-lg transition-shadow">
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <span className="text-2xl">📁</span>
                Projects
              </CardTitle>
              <CardDescription>Active projects</CardDescription>
            </CardHeader>
            <CardContent>
              <p className="text-sm text-slate-600 dark:text-slate-400">Select a workspace to view projects</p>
            </CardContent>
          </Card>

          <Card className="hover:shadow-lg transition-shadow">
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <span className="text-2xl">✅</span>
                My Tasks
              </CardTitle>
              <CardDescription>Tasks assigned to you</CardDescription>
            </CardHeader>
            <CardContent>
              <p className="text-sm text-slate-600 dark:text-slate-400">No tasks assigned yet</p>
            </CardContent>
          </Card>
        </div>

        <Card className="mt-8">
          <CardHeader>
            <CardTitle>Quick Start</CardTitle>
            <CardDescription>Get started with Nexora</CardDescription>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="flex items-center gap-3">
              <span className="text-xl">1️⃣</span>
              <div className="flex-1">
                <p className="font-medium">Create a workspace</p>
                <p className="text-sm text-slate-600 dark:text-slate-400">Organize your projects in workspaces</p>
              </div>
              <Button variant="outline" size="sm">Create</Button>
            </div>
            <div className="flex items-center gap-3">
              <span className="text-xl">2️⃣</span>
              <div className="flex-1">
                <p className="font-medium">Invite team members</p>
                <p className="text-sm text-slate-600 dark:text-slate-400">Collaborate with your team</p>
              </div>
              <Button variant="outline" size="sm">Invite</Button>
            </div>
            <div className="flex items-center gap-3">
              <span className="text-xl">3️⃣</span>
              <div className="flex-1">
                <p className="font-medium">Create your first project</p>
                <p className="text-sm text-slate-600 dark:text-slate-400">Start managing tasks</p>
              </div>
              <Button variant="outline" size="sm">Create</Button>
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}
