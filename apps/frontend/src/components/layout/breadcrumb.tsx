"use client"

import * as React from "react"
import Link from "next/link"
import type { Route } from "next"
import { ChevronRight } from "lucide-react"
import { cn } from "@/lib/utils"

interface BreadcrumbItem {
  label: string
  href?: string
}

interface BreadcrumbProps {
  items: BreadcrumbItem[]
  className?: string
}

export function Breadcrumb({ items, className }: BreadcrumbProps) {
  return (
    <nav
      className={cn(
        "flex items-center gap-2 text-sm",
        "text-gray-500 dark:text-gray-400",
        className
      )}
      aria-label="Breadcrumb"
    >
      {items.map((item, index) => (
        <React.Fragment key={index}>
          {index > 0 && (
            <ChevronRight className="h-4 w-4 flex-shrink-0" />
          )}
          {item.href ? (
            <Link
              href={item.href as unknown as Route}
              className="hover:text-gray-900 dark:hover:text-gray-200"
            >
              {item.label}
            </Link>
          ) : (
            <span className="text-gray-900 dark:text-gray-200">
              {item.label}
            </span>
          )}
        </React.Fragment>
      ))}
    </nav>
  )
}

