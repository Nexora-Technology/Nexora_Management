'use client';

import { useQuery } from '@tanstack/react-query';
import { analyticsService, DashboardStatsDto } from '@/lib/services/analytics-service';
import { StatsCard } from './stats-card';
import { CheckCircle2, Clock, AlertCircle, Users, FolderOpen, TrendingUp } from 'lucide-react';

interface DashboardStatsProps {
  workspaceId: string;
}

export function DashboardStats({ workspaceId }: DashboardStatsProps) {
  const { data: stats, isLoading, error } = useQuery({
    queryKey: ['analytics', 'dashboard', workspaceId],
    queryFn: () => analyticsService.getDashboardStats(workspaceId),
    enabled: !!workspaceId,
  });

  if (isLoading) return <div>Loading...</div>;
  if (error) return <div>Error loading stats</div>;

  if (!stats) return null;

  return (
    <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
      <StatsCard
        title="Total Tasks"
        value={stats.totalTasks}
        description={`${stats.completedTasks} completed`}
      />
      <StatsCard
        title="In Progress"
        value={stats.inProgressTasks}
        description={`${stats.overdueTasks} overdue`}
      />
      <StatsCard
        title="Completion Rate"
        value={`${stats.completionPercentage.toFixed(1)}%`}
        description={`${stats.totalTasks - stats.completedTasks} remaining`}
      />
      <StatsCard
        title="Team Members"
        value={stats.activeMembers}
        description={`${stats.totalTeamMembers} total`}
      />
    </div>
  );
}
