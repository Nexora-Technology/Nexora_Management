'use client';

import { use, useQuery } from '@tanstack/react-query';
import { dashboardService } from '@/lib/services/dashboard-service';
import { Button } from '@/components/ui/button';
import { ArrowLeft, Settings } from 'lucide-react';
import Link from 'next/link';
import { DashboardStats } from '@/components/analytics/dashboard-stats';
import { ChartContainer } from '@/components/analytics/chart-container';

export default function DashboardDetailPage({ params }: { params: Promise<{ id: string }> }) {
  const { id } = use(params);
  const { data: dashboard, isLoading } = useQuery({
    queryKey: ['dashboard', id],
    queryFn: () => dashboardService.getDashboard(id),
  });

  if (isLoading) return <div>Loading...</div>;
  if (!dashboard) return <div>Dashboard not found</div>;

  return (
    <div className="container mx-auto py-6">
      <div className="flex items-center justify-between mb-6">
        <div className="flex items-center gap-4">
          <Link href="/dashboards">
            <Button variant="ghost" size="sm">
              <ArrowLeft className="mr-2 h-4 w-4" />
              Back
            </Button>
          </Link>
          <div>
            <h1 className="text-3xl font-bold">{dashboard.name}</h1>
            <p className="text-muted-foreground">
              Workspace analytics dashboard
            </p>
          </div>
        </div>
        <Button variant="outline">
          <Settings className="mr-2 h-4 w-4" />
          Edit
        </Button>
      </div>

      <div className="space-y-6">
        {/* Dashboard Stats */}
        <DashboardStats workspaceId={dashboard.workspaceId} />

        {/* Additional widgets can be added here based on dashboard.layout */}
        <div className="grid gap-4 md:grid-cols-2">
          <ChartContainer title="Recent Activity">
            <div className="h-[200px] flex items-center justify-center text-muted-foreground">
              Activity chart coming soon
            </div>
          </ChartContainer>

          <ChartContainer title="Team Workload">
            <div className="h-[200px] flex items-center justify-center text-muted-foreground">
              Workload chart coming soon
            </div>
          </ChartContainer>
        </div>
      </div>
    </div>
  );
}
