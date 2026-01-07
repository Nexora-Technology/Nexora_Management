"use client"

import * as React from "react"
import Link from "next/link"
import type { Route } from "next"
import { usePathname } from "next/navigation"
import {
  Home,
  CheckSquare,
  Folder,
  Users,
  Calendar,
  Settings,
  ChevronRight,
  Target,
  FileText,
} from "lucide-react"
import { cn } from "@/lib/utils"

const navItems = [
  {
    title: "Home",
    href: "/",
    icon: Home,
  },
  {
    title: "Spaces",
    href: "/spaces",
    icon: Folder,
  },
  {
    title: "Goals",
    href: "/goals",
    icon: Target,
  },
  {
    title: "Documents",
    href: "/documents",
    icon: FileText,
  },
  {
    title: "Team",
    href: "/team",
    icon: Users,
  },
  {
    title: "Calendar",
    href: "/calendar",
    icon: Calendar,
  },
  {
    title: "Settings",
    href: "/settings",
    icon: Settings,
  },
]

interface SidebarNavProps {
  collapsed?: boolean
}

export function SidebarNav({ collapsed = false }: SidebarNavProps) {
  const pathname = usePathname()

  return (
    <div className="space-y-1 px-3">
      {navItems.map((item) => {
        const isActive = pathname === item.href
        const Icon = item.icon

        return (
          <Link
            key={item.href}
            href={item.href as unknown as Route}
            className={cn(
              "flex items-center gap-3 rounded-lg px-3 py-2 text-sm font-medium transition-all",
              "hover:bg-gray-100 dark:hover:bg-gray-700",
              isActive
                ? "bg-primary/10 text-primary"
                : "text-gray-600 dark:text-gray-400",
              collapsed && "justify-center px-0"
            )}
          >
            <Icon className="h-5 w-5 flex-shrink-0" />
            {!collapsed && (
              <>
                <span className="flex-1">{item.title}</span>
                {isActive && <ChevronRight className="h-4 w-4" />}
              </>
            )}
          </Link>
        )
      })}
    </div>
  )
}
