"use client"

import * as React from "react"
import { cn } from "@/lib/utils"

interface BoardLayoutProps {
  children: React.ReactNode
  className?: string
}

export function BoardLayout({ children, className }: BoardLayoutProps) {
  return (
    <div
      className={cn(
        "flex gap-6 overflow-x-auto pb-4",
        // Horizontal scroll
        "snap-x snap-mandatory",
        // Prevent column shrink
        "[&>*]:flex-shrink-0 [&>*]:snap-start",
        className
      )}
    >
      {children}
    </div>
  )
}

interface BoardColumnProps {
  title: string
  count?: number
  children: React.ReactNode
  className?: string
  id?: string
}

export function BoardColumn({
  title,
  count,
  children,
  className,
  id,
}: BoardColumnProps) {
  return (
    <div
      id={id}
      className={cn(
        "w-[280px] flex-shrink-0 snap-start",
        className
      )}
    >
      {/* Column Header */}
      <div className="mb-3 flex items-center justify-between">
        <h3 className="text-sm font-semibold text-gray-900 dark:text-white">
          {title}
        </h3>
        {count !== undefined && (
          <span className="text-xs text-gray-500 dark:text-gray-400">
            {count}
          </span>
        )}
      </div>

      {/* Column Content */}
      <div className="space-y-2">
        {children}
      </div>
    </div>
  )
}
