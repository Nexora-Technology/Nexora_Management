"use client"

import * as React from "react"
import { cn } from "@/lib/utils"
import { SidebarNav } from "./sidebar-nav"

interface AppSidebarProps {
  collapsed?: boolean
}

export function AppSidebar({ collapsed = false }: AppSidebarProps) {
  return (
    <aside
      className={cn(
        "flex-shrink-0 border-r border-gray-200 bg-white transition-all duration-200 dark:bg-gray-800 dark:border-gray-700",
        collapsed ? "w-16" : "w-60"
      )}
    >
      {/* Navigation */}
      <nav className="flex h-full flex-col overflow-y-auto py-4">
        <SidebarNav collapsed={collapsed} />
      </nav>
    </aside>
  )
}
