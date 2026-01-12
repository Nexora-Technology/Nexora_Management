'use client';

import * as React from 'react';
import { useQuery } from '@tanstack/react-query';
import { BarChart3, TrendingUp, Users, CheckCircle, Clock } from 'lucide-react';
import { useWorkspace } from '@/features/workspaces';
import { cn } from '@/lib/utils';

export default function AnalyticsPage() {
  const { currentWorkspace, isLoading: workspaceLoading } = useWorkspace();

  const { data: analytics, isLoading: analyticsLoading } = useQuery({
    queryKey: ['analytics', currentWorkspace?.id],
    queryFn: async () => {
      if (!currentWorkspace) return null;
      const response = await fetch(`/api/workspaces/${currentWorkspace.id}/analytics`);
      if (!response.ok) return null;
      return response.json();
    },
    enabled: !!currentWorkspace,
  });

  const isLoading = workspaceLoading || analyticsLoading;

  // Show no workspace state
  if (!currentWorkspace) {
    return (
      <div className="flex h-full items-center justify-center bg-gray-50 dark:bg-gray-900">
        <div className="text-center max-w-md">
          <div className="mb-4">
            <BarChart3 className="h-16 w-16 text-gray-300 dark:text-gray-600 mx-auto" />
          </div>
          <h2 className="text-2xl font-bold text-gray-900 dark:text-gray-100 mb-2">
            No Workspace Selected
          </h2>
          <p className="text-gray-600 dark:text-gray-400 mb-6">
            Select a workspace to view analytics and insights.
          </p>
        </div>
      </div>
    );
  }

  return (
    <div className="container mx-auto p-6">
      <div className="mb-6">
        <h1 className="text-3xl font-bold text-gray-900 dark:text-gray-100">Analytics</h1>
        <p className="text-gray-600 dark:text-gray-400 mt-1">
          Track your team&apos;s performance and progress
        </p>
      </div>

      {isLoading ? (
        <div className="flex items-center justify-center py-12">
          <div className="text-sm text-gray-500 dark:text-gray-400">Loading analytics...</div>
        </div>
      ) : (
        <div className="space-y-6">
          {/* Stats Cards */}
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
            <StatCard
              icon={<CheckCircle className="h-5 w-5" />}
              title="Completed Tasks"
              value={analytics?.completedTasks ?? 0}
              change="+12%"
              trend="up"
            />
            <StatCard
              icon={<Clock className="h-5 w-5" />}
              title="In Progress"
              value={analytics?.inProgressTasks ?? 0}
              change="+5%"
              trend="up"
            />
            <StatCard
              icon={<Users className="h-5 w-5" />}
              title="Team Members"
              value={analytics?.teamMembers ?? 0}
              change="+2"
              trend="up"
            />
            <StatCard
              icon={<TrendingUp className="h-5 w-5" />}
              title="Productivity"
              value="87%"
              change="+3%"
              trend="up"
            />
          </div>

          {/* Placeholder for charts */}
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
            <div className="p-6 rounded-lg border border-gray-200 dark:border-gray-700 bg-white dark:bg-gray-800">
              <h3 className="text-lg font-semibold text-gray-900 dark:text-gray-100 mb-4">
                Task Completion Trend
              </h3>
              <div className="h-64 flex items-center justify-center text-gray-400 dark:text-gray-500">
                <div className="text-center">
                  <BarChart3 className="h-12 w-12 mx-auto mb-2 opacity-50" />
                  <p className="text-sm">Chart visualization coming soon</p>
                </div>
              </div>
            </div>

            <div className="p-6 rounded-lg border border-gray-200 dark:border-gray-700 bg-white dark:bg-gray-800">
              <h3 className="text-lg font-semibold text-gray-900 dark:text-gray-100 mb-4">
                Team Performance
              </h3>
              <div className="h-64 flex items-center justify-center text-gray-400 dark:text-gray-500">
                <div className="text-center">
                  <Users className="h-12 w-12 mx-auto mb-2 opacity-50" />
                  <p className="text-sm">Team metrics coming soon</p>
                </div>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}

function StatCard({
  icon,
  title,
  value,
  change,
  trend,
}: {
  icon: React.ReactNode;
  title: string;
  value: string | number;
  change: string;
  trend: 'up' | 'down';
}) {
  return (
    <div
      className={cn(
        'p-4 rounded-lg border border-gray-200 dark:border-gray-700',
        'bg-white dark:bg-gray-800'
      )}
    >
      <div className="flex items-center justify-between mb-2">
        <div className={cn('text-primary', '[&>svg]:h-5 [&>svg]:w-5')}>{icon}</div>
        {trend === 'up' && (
          <span className="text-xs font-medium text-green-600 dark:text-green-400 flex items-center gap-1">
            <TrendingUp className="h-3 w-3" />
            {change}
          </span>
        )}
      </div>
      <div>
        <p className="text-2xl font-bold text-gray-900 dark:text-gray-100">{value}</p>
        <p className="text-sm text-gray-600 dark:text-gray-400">{title}</p>
      </div>
    </div>
  );
}
