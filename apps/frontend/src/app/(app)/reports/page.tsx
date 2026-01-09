'use client';

import { Button } from '@/components/ui/button';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { FileText, BarChart3, PieChart, TrendingUp } from 'lucide-react';

export default function ReportsPage() {
  return (
    <div className="container mx-auto py-6">
      <div className="mb-6">
        <h1 className="text-3xl font-bold">Reports</h1>
        <p className="text-muted-foreground">
          Generate and export custom reports
        </p>
      </div>

      <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
        <Card className="hover:shadow-md transition-shadow cursor-pointer">
          <CardHeader>
            <BarChart3 className="h-8 w-8 text-blue-500 mb-2" />
            <CardTitle>Sprint Report</CardTitle>
            <CardDescription>
              Sprint velocity and burndown
            </CardDescription>
          </CardHeader>
          <CardContent>
            <Button variant="outline" className="w-full">
              Generate
            </Button>
          </CardContent>
        </Card>

        <Card className="hover:shadow-md transition-shadow cursor-pointer">
          <CardHeader>
            <PieChart className="h-8 w-8 text-green-500 mb-2" />
            <CardTitle>Project Report</CardTitle>
            <CardDescription>
              Project progress overview
            </CardDescription>
          </CardHeader>
          <CardContent>
            <Button variant="outline" className="w-full">
              Generate
            </Button>
          </CardContent>
        </Card>

        <Card className="hover:shadow-md transition-shadow cursor-pointer">
          <CardHeader>
            <TrendingUp className="h-8 w-8 text-purple-500 mb-2" />
            <CardTitle>Time Report</CardTitle>
            <CardDescription>
              Time tracking summary
            </CardDescription>
          </CardHeader>
          <CardContent>
            <Button variant="outline" className="w-full">
              Generate
            </Button>
          </CardContent>
        </Card>

        <Card className="hover:shadow-md transition-shadow cursor-pointer">
          <CardHeader>
            <FileText className="h-8 w-8 text-orange-500 mb-2" />
            <CardTitle>Custom Report</CardTitle>
            <CardDescription>
              Build your own report
            </CardDescription>
          </CardHeader>
          <CardContent>
            <Button variant="outline" className="w-full">
              Create
            </Button>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}
