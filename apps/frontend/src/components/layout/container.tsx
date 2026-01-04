"use client"

import * as React from "react"
import { cn } from "@/lib/utils"

interface ContainerProps {
  children: React.ReactNode
  size?: "sm" | "md" | "lg" | "xl" | "full"
  className?: string
}

export function Container({
  children,
  size = "lg",
  className,
}: ContainerProps) {
  const sizeClasses = {
    sm: "max-w-3xl", // 768px
    md: "max-w-4xl", // 896px
    lg: "max-w-6xl", // 1152px (ClickUp default)
    xl: "max-w-7xl", // 1280px
    full: "max-w-full",
  }

  return (
    <div
      className={cn(
        "mx-auto px-4 sm:px-6 lg:px-8",
        sizeClasses[size],
        className
      )}
    >
      {children}
    </div>
  )
}
