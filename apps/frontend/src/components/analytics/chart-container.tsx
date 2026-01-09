'use client';

import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';

interface ChartContainerProps {
  title: string;
  isLoading?: boolean;
  error?: string;
  children: React.ReactNode;
}

export function ChartContainer({ title, isLoading, error, children }: ChartContainerProps) {
  return (
    <Card className="w-full">
      <CardHeader>
        <CardTitle>{title}</CardTitle>
      </CardHeader>
      <CardContent>
        {isLoading && (
          <div className="flex items-center justify-center h-[200px] text-muted-foreground">
            Loading...
          </div>
        )}
        {error && (
          <div className="flex items-center justify-center h-[200px] text-destructive">
            {error}
          </div>
        )}
        {!isLoading && !error && children}
      </CardContent>
    </Card>
  );
}
