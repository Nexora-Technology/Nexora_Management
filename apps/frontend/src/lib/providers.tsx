"use client";

import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { ReactQueryDevtools } from "@tanstack/react-query-devtools";
import { useState, type ReactNode } from "react";
import { AuthProvider } from "@/features/auth/providers/auth-provider";
import { NotificationCenter, type Notification } from "@/features/notifications/NotificationCenter";

interface ProvidersProps {
  children: ReactNode;
}

export function Providers({ children }: ProvidersProps) {
  const [queryClient] = useState(
    () =>
      new QueryClient({
        defaultOptions: {
          queries: {
            staleTime: 60 * 1000, // 1 minute
            refetchOnWindowFocus: false,
          },
        },
      })
  );

  // TODO: Replace with actual notifications from API
  const mockNotifications: Notification[] = [];
  const unreadCount = 0;

  return (
    <QueryClientProvider client={queryClient}>
      <AuthProvider>
        {children}
        <NotificationCenter
          notifications={mockNotifications}
          unreadCount={unreadCount}
        />
        <ReactQueryDevtools initialIsOpen={false} />
      </AuthProvider>
    </QueryClientProvider>
  );
}
