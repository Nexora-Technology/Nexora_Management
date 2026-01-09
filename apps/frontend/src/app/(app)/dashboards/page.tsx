'use client';

import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { dashboardService, DashboardDto } from '@/lib/services/dashboard-service';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Plus, LayoutGrid, Trash2, Edit } from 'lucide-react';
import { useWorkspace } from '@/features/workspaces/workspace-provider';
import Link from 'next/link';

export default function DashboardsPage() {
  const { currentWorkspace } = useWorkspace();
  const queryClient = useQueryClient();

  const { data: dashboards, isLoading } = useQuery({
    queryKey: ['dashboards', currentWorkspace?.id],
    queryFn: () => dashboardService.getDashboards(currentWorkspace!.id),
    enabled: !!currentWorkspace?.id,
  });

  const deleteMutation = useMutation({
    mutationFn: (id: string) => dashboardService.deleteDashboard(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['dashboards'] });
    },
  });

  if (!currentWorkspace) {
    return <div>Please select a workspace</div>;
  }

  return (
    <div className="container mx-auto py-6">
      <div className="flex items-center justify-between mb-6">
        <div>
          <h1 className="text-3xl font-bold">Dashboards</h1>
          <p className="text-muted-foreground">
            Manage your analytics dashboards
          </p>
        </div>
        <Link href="/dashboards/new">
          <Button>
            <Plus className="mr-2 h-4 w-4" />
            New Dashboard
          </Button>
        </Link>
      </div>

      {isLoading ? (
        <div>Loading...</div>
      ) : dashboards && dashboards.length > 0 ? (
        <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
          {dashboards.map((dashboard) => (
            <Card key={dashboard.id} className="hover:shadow-md transition-shadow">
              <CardHeader>
                <div className="flex items-start justify-between">
                  <div className="flex items-center gap-2">
                    <LayoutGrid className="h-5 w-5 text-muted-foreground" />
                    <CardTitle>{dashboard.name}</CardTitle>
                  </div>
                  <div className="flex gap-2">
                    <Link href={`/dashboards/${dashboard.id}`}>
                      <Button variant="ghost" size="sm">
                        <Edit className="h-4 w-4" />
                      </Button>
                    </Link>
                    <Button
                      variant="ghost"
                      size="sm"
                      onClick={() => deleteMutation.mutate(dashboard.id)}
                    >
                      <Trash2 className="h-4 w-4" />
                    </Button>
                  </div>
                </div>
                <CardDescription>
                  {dashboard.isTemplate ? 'Template dashboard' : 'Custom dashboard'}
                </CardDescription>
              </CardHeader>
              <CardContent>
                <Link href={`/dashboards/${dashboard.id}`}>
                  <Button variant="outline" className="w-full">
                    View Dashboard
                  </Button>
                </Link>
              </CardContent>
            </Card>
          ))}
        </div>
      ) : (
        <Card>
          <CardContent className="flex flex-col items-center justify-center py-12">
            <LayoutGrid className="h-12 w-12 text-muted-foreground mb-4" />
            <h3 className="text-lg font-semibold mb-2">No dashboards yet</h3>
            <p className="text-muted-foreground mb-4">
              Create your first dashboard to track analytics
            </p>
            <Link href="/dashboards/new">
              <Button>
                <Plus className="mr-2 h-4 w-4" />
                Create Dashboard
              </Button>
            </Link>
          </CardContent>
        </Card>
      )}
    </div>
  );
}
