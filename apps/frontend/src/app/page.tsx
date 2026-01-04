"use client";

import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { useRouter } from "next/navigation";
import Link from "next/link";

export default function Home() {
  const router = useRouter();
  return (
    <div className="min-h-screen bg-gradient-to-br from-sky-50 via-white to-teal-50 dark:from-slate-900 dark:via-slate-800 dark:to-slate-900">
      {/* Header */}
      <div className="container mx-auto px-4 py-4">
        <div className="flex justify-between items-center">
          <h1 className="text-2xl font-bold bg-gradient-to-r from-sky-500 to-teal-500 bg-clip-text text-transparent">
            Nexora
          </h1>
          <div className="flex gap-3">
            <Button variant="ghost" onClick={() => router.push("/login")}>
              Sign In
            </Button>
            <Button onClick={() => router.push("/register")}>
              Get Started
            </Button>
          </div>
        </div>
      </div>

      <div className="container mx-auto px-4 py-8">
        {/* Hero Section */}
        <div className="text-center mb-16">
          <h1 className="text-6xl font-extrabold mb-6 bg-gradient-to-r from-sky-500 to-teal-500 bg-clip-text text-transparent">
            Welcome to Nexora
          </h1>
          <p className="text-xl text-slate-600 dark:text-slate-300 max-w-2xl mx-auto mb-8">
            Team management, task tracking, and project coordination platform for modern teams.
          </p>
          <div className="flex gap-4 justify-center">
            <Button size="lg" className="bg-gradient-to-r from-sky-500 to-teal-500 hover:from-sky-600 hover:to-teal-600 text-white font-semibold" onClick={() => router.push("/dashboard")}>
              Get Started
            </Button>
            <Button size="lg" variant="outline" onClick={() => router.push("/workspaces")}>
              View Workspaces
            </Button>
          </div>
        </div>

        {/* Features Grid */}
        <div className="grid md:grid-cols-3 gap-6 max-w-6xl mx-auto">
          <Card className="border-slate-200 dark:border-slate-700 hover:shadow-lg transition-shadow">
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <span className="text-2xl">📋</span>
                Task Management
              </CardTitle>
              <CardDescription>
                Organize tasks with boards, lists, and timelines. Track progress with customizable workflows.
              </CardDescription>
            </CardHeader>
          </Card>

          <Card className="border-slate-200 dark:border-slate-700 hover:shadow-lg transition-shadow">
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <span className="text-2xl">👥</span>
                Team Collaboration
              </CardTitle>
              <CardDescription>
                Work together seamlessly with real-time updates, comments, and notifications.
              </CardDescription>
            </CardHeader>
          </Card>

          <Card className="border-slate-200 dark:border-slate-700 hover:shadow-lg transition-shadow">
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <span className="text-2xl">📊</span>
                Analytics & Insights
              </CardTitle>
              <CardDescription>
                Gain visibility into team performance with powerful dashboards and reports.
              </CardDescription>
            </CardHeader>
          </Card>
        </div>

        {/* Tech Stack Section */}
        <div className="mt-16 text-center">
          <h2 className="text-2xl font-semibold mb-4 text-slate-800 dark:text-slate-200">
            Built with Modern Technologies
          </h2>
          <div className="flex flex-wrap gap-3 justify-center">
            {["Next.js 15", "React 19", "TypeScript", "Tailwind CSS", "shadcn/ui", "React Query", "Zustand", "SignalR"].map((tech) => (
              <span
                key={tech}
                className="px-4 py-2 bg-white dark:bg-slate-800 border border-slate-200 dark:border-slate-700 rounded-lg text-sm font-medium text-slate-700 dark:text-slate-300"
              >
                {tech}
              </span>
            ))}
          </div>
        </div>

        {/* Status Section */}
        <div className="mt-16 max-w-2xl mx-auto">
          <Card className="bg-gradient-to-r from-sky-500 to-teal-500 text-white border-0">
            <CardHeader>
              <CardTitle className="text-2xl">🚀 Platform Ready</CardTitle>
              <CardDescription className="text-sky-50">
                Frontend setup complete! The development environment is configured and ready to go.
              </CardDescription>
            </CardHeader>
            <CardContent>
              <div className="space-y-2 text-sm">
                <div className="flex items-center gap-2">
                  <span className="text-xl">✅</span>
                  <span>Next.js 15 with App Router initialized</span>
                </div>
                <div className="flex items-center gap-2">
                  <span className="text-xl">✅</span>
                  <span>Design system configured with Nexora colors</span>
                </div>
                <div className="flex items-center gap-2">
                  <span className="text-xl">✅</span>
                  <span>shadcn/ui components installed</span>
                </div>
                <div className="flex items-center gap-2">
                  <span className="text-xl">✅</span>
                  <span>State management & data fetching ready</span>
                </div>
              </div>
            </CardContent>
          </Card>
        </div>
      </div>
    </div>
  );
}
