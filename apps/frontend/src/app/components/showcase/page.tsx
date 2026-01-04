"use client"

import { useState } from "react"
import { Button } from "@/components/ui/button"
import { Badge } from "@/components/ui/badge"
import { Input } from "@/components/ui/input"
import { Textarea } from "@/components/ui/textarea"
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar"
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from "@/components/ui/tooltip"
import { CheckCircle2, Clock, AlertCircle, User, Info } from "lucide-react"

export default function ComponentShowcasePage() {
  const [isDark, setIsDark] = useState(false)

  return (
    <TooltipProvider>
      <div className={isDark ? "dark" : ""}>
        <div className="min-h-screen bg-white dark:bg-gray-950 p-8">
          {/* Header */}
          <div className="max-w-7xl mx-auto mb-8">
            <div className="flex items-center justify-between">
              <div>
                <h1 className="text-3xl font-bold text-gray-900 dark:text-white">
                  ClickUp Design System - Component Showcase
                </h1>
                <p className="text-gray-600 dark:text-gray-400 mt-2">
                  Phase 02: Core UI Components
                </p>
              </div>
              <Button
                variant="outline"
                onClick={() => setIsDark(!isDark)}
                className="gap-2"
              >
                {isDark ? "☀️ Light" : "🌙 Dark"}
              </Button>
            </div>
          </div>

          <div className="max-w-7xl mx-auto space-y-12">
            {/* Buttons Section */}
            <section className="border border-gray-200 dark:border-gray-800 rounded-lg p-6">
              <h2 className="text-xl font-semibold text-gray-900 dark:text-white mb-4">
                Buttons
              </h2>
              <p className="text-gray-600 dark:text-gray-400 mb-6 text-sm">
                All button variants with scale transforms on hover/active
              </p>

              <div className="space-y-6">
                {/* Variants */}
                <div>
                  <h3 className="text-sm font-medium text-gray-700 dark:text-gray-300 mb-3">
                    Variants
                  </h3>
                  <div className="flex flex-wrap gap-3">
                    <Button variant="primary">Primary</Button>
                    <Button variant="secondary">Secondary</Button>
                    <Button variant="ghost">Ghost</Button>
                    <Button variant="destructive">Destructive</Button>
                    <Button variant="outline">Outline</Button>
                    <Button variant="link">Link</Button>
                  </div>
                </div>

                {/* Sizes */}
                <div>
                  <h3 className="text-sm font-medium text-gray-700 dark:text-gray-300 mb-3">
                    Sizes
                  </h3>
                  <div className="flex flex-wrap items-center gap-3">
                    <Button variant="primary" size="sm">Small</Button>
                    <Button variant="primary" size="md">Medium</Button>
                    <Button variant="primary" size="lg">Large</Button>
                    <Button variant="primary" size="icon">
                      <User className="h-4 w-4" />
                    </Button>
                  </div>
                </div>

                {/* With Icons */}
                <div>
                  <h3 className="text-sm font-medium text-gray-700 dark:text-gray-300 mb-3">
                    With Icons
                  </h3>
                  <div className="flex flex-wrap gap-3">
                    <Button variant="primary" className="gap-2">
                      <CheckCircle2 className="h-4 w-4" />
                      Complete
                    </Button>
                    <Button variant="secondary" className="gap-2">
                      <Clock className="h-4 w-4" />
                      In Progress
                    </Button>
                    <Button variant="destructive" className="gap-2">
                      <AlertCircle className="h-4 w-4" />
                      Delete
                    </Button>
                  </div>
                </div>

                {/* Disabled */}
                <div>
                  <h3 className="text-sm font-medium text-gray-700 dark:text-gray-300 mb-3">
                    Disabled
                  </h3>
                  <div className="flex flex-wrap gap-3">
                    <Button variant="primary" disabled>Primary</Button>
                    <Button variant="secondary" disabled>Secondary</Button>
                    <Button variant="ghost" disabled>Ghost</Button>
                  </div>
                </div>
              </div>
            </section>

            {/* Badges Section */}
            <section className="border border-gray-200 dark:border-gray-800 rounded-lg p-6">
              <h2 className="text-xl font-semibold text-gray-900 dark:text-white mb-4">
                Badges
              </h2>
              <p className="text-gray-600 dark:text-gray-400 mb-6 text-sm">
                Status indicators for task management
              </p>

              <div className="space-y-6">
                {/* Status Variants */}
                <div>
                  <h3 className="text-sm font-medium text-gray-700 dark:text-gray-300 mb-3">
                    Status Variants
                  </h3>
                  <div className="flex flex-wrap gap-3">
                    <Badge status="complete" icon={<CheckCircle2 className="h-3 w-3" />}>
                      Complete
                    </Badge>
                    <Badge status="inProgress" icon={<Clock className="h-3 w-3" />}>
                      In Progress
                    </Badge>
                    <Badge status="overdue" icon={<AlertCircle className="h-3 w-3" />}>
                      Overdue
                    </Badge>
                    <Badge status="neutral">Neutral</Badge>
                    <Badge status="default">Default</Badge>
                  </div>
                </div>

                {/* Sizes */}
                <div>
                  <h3 className="text-sm font-medium text-gray-700 dark:text-gray-300 mb-3">
                    Sizes
                  </h3>
                  <div className="flex flex-wrap items-center gap-3">
                    <Badge status="complete" size="sm">Small</Badge>
                    <Badge status="complete" size="md">Medium</Badge>
                    <Badge status="complete" size="lg">Large</Badge>
                  </div>
                </div>

                {/* With Icons */}
                <div>
                  <h3 className="text-sm font-medium text-gray-700 dark:text-gray-300 mb-3">
                    With Icons
                  </h3>
                  <div className="flex flex-wrap gap-3">
                    <Badge status="complete" icon={<CheckCircle2 className="h-3 w-3" />}>
                      Done
                    </Badge>
                    <Badge status="inProgress" icon={<Clock className="h-3 w-3" />}>
                      Working
                    </Badge>
                    <Badge status="overdue" icon={<AlertCircle className="h-3 w-3" />}>
                      Urgent
                    </Badge>
                  </div>
                </div>
              </div>
            </section>

            {/* Form Elements Section */}
            <section className="border border-gray-200 dark:border-gray-800 rounded-lg p-6">
              <h2 className="text-xl font-semibold text-gray-900 dark:text-white mb-4">
                Form Elements
              </h2>
              <p className="text-gray-600 dark:text-gray-400 mb-6 text-sm">
                2px borders with purple focus ring and error states
              </p>

              <div className="space-y-6 max-w-lg">
                {/* Input - Normal */}
                <div>
                  <h3 className="text-sm font-medium text-gray-700 dark:text-gray-300 mb-3">
                    Input - Normal State
                  </h3>
                  <Input placeholder="Enter your name..." />
                </div>

                {/* Input - Focus */}
                <div>
                  <h3 className="text-sm font-medium text-gray-700 dark:text-gray-300 mb-3">
                    Input - With Value
                  </h3>
                  <Input defaultValue="John Doe" />
                </div>

                {/* Input - Error */}
                <div>
                  <h3 className="text-sm font-medium text-gray-700 dark:text-gray-300 mb-3">
                    Input - Error State
                  </h3>
                  <Input error placeholder="This field has an error" />
                  <p className="text-error text-sm mt-1">This field is required</p>
                </div>

                {/* Textarea - Normal */}
                <div>
                  <h3 className="text-sm font-medium text-gray-700 dark:text-gray-300 mb-3">
                    Textarea - Normal State
                  </h3>
                  <Textarea placeholder="Enter a description..." />
                </div>

                {/* Textarea - Error */}
                <div>
                  <h3 className="text-sm font-medium text-gray-700 dark:text-gray-300 mb-3">
                    Textarea - Error State
                  </h3>
                  <Textarea error placeholder="This field has an error" />
                  <p className="text-error text-sm mt-1">Description is required</p>
                </div>
              </div>
            </section>

            {/* Avatars Section */}
            <section className="border border-gray-200 dark:border-gray-800 rounded-lg p-6">
              <h2 className="text-xl font-semibold text-gray-900 dark:text-white mb-4">
                Avatars
              </h2>
              <p className="text-gray-600 dark:text-gray-400 mb-6 text-sm">
                User profile displays with initials and hash-based colors
              </p>

              <div className="space-y-6">
                {/* Sizes */}
                <div>
                  <h3 className="text-sm font-medium text-gray-700 dark:text-gray-300 mb-3">
                    Different Names (Hash-based Colors)
                  </h3>
                  <div className="flex flex-wrap items-center gap-4">
                    <Avatar>
                      <AvatarFallback name="Alice Johnson" />
                    </Avatar>
                    <Avatar>
                      <AvatarFallback name="Bob Smith" />
                    </Avatar>
                    <Avatar>
                      <AvatarFallback name="Charlie Brown" />
                    </Avatar>
                    <Avatar>
                      <AvatarFallback name="Diana Prince" />
                    </Avatar>
                    <Avatar>
                      <AvatarFallback name="Eve Williams" />
                    </Avatar>
                    <Avatar>
                      <AvatarFallback name="Frank Miller" />
                    </Avatar>
                    <Avatar>
                      <AvatarFallback name="Grace Lee" />
                    </Avatar>
                    <Avatar>
                      <AvatarFallback name="Henry Davis" />
                    </Avatar>
                  </div>
                </div>

                {/* With Image */}
                <div>
                  <h3 className="text-sm font-medium text-gray-700 dark:text-gray-300 mb-3">
                    With Image (Fallback to Initials)
                  </h3>
                  <div className="flex flex-wrap items-center gap-4">
                    <Avatar>
                      <AvatarImage src="https://i.pravatar.cc/150?u=1" />
                      <AvatarFallback name="User One" />
                    </Avatar>
                    <Avatar>
                      <AvatarFallback name="No Image" />
                    </Avatar>
                  </div>
                </div>
              </div>
            </section>

            {/* Tooltips Section */}
            <section className="border border-gray-200 dark:border-gray-800 rounded-lg p-6">
              <h2 className="text-xl font-semibold text-gray-900 dark:text-white mb-4">
                Tooltips
              </h2>
              <p className="text-gray-600 dark:text-gray-400 mb-6 text-sm">
                Dark theme tooltips with 200ms delay
              </p>

              <div className="space-y-6">
                <div>
                  <h3 className="text-sm font-medium text-gray-700 dark:text-gray-300 mb-3">
                    Basic Tooltip
                  </h3>
                  <div className="flex flex-wrap gap-4">
                    <Tooltip delayDuration={200}>
                      <TooltipTrigger asChild>
                        <Button variant="outline" className="gap-2">
                          <Info className="h-4 w-4" />
                          Hover me
                        </Button>
                      </TooltipTrigger>
                      <TooltipContent>
                        This is a helpful tooltip
                      </TooltipContent>
                    </Tooltip>

                    <Tooltip delayDuration={200}>
                      <TooltipTrigger asChild>
                        <Badge status="neutral" className="cursor-help">
                          Status
                        </Badge>
                      </TooltipTrigger>
                      <TooltipContent>
                        Task is waiting to start
                      </TooltipContent>
                    </Tooltip>
                  </div>
                </div>

                <div>
                  <h3 className="text-sm font-medium text-gray-700 dark:text-gray-300 mb-3">
                    With Different Content
                  </h3>
                  <div className="flex flex-wrap gap-4">
                    <Tooltip delayDuration={200}>
                      <TooltipTrigger>
                        <Avatar>
                          <AvatarFallback name="Tooltip Test" />
                        </Avatar>
                      </TooltipTrigger>
                      <TooltipContent>
                        <p className="font-medium">John Doe</p>
                        <p className="text-gray-300">john@example.com</p>
                      </TooltipContent>
                    </Tooltip>
                  </div>
                </div>
              </div>
            </section>

            {/* Combination Example */}
            <section className="border border-gray-200 dark:border-gray-800 rounded-lg p-6">
              <h2 className="text-xl font-semibold text-gray-900 dark:text-white mb-4">
                Real-world Example: Task Card
              </h2>
              <p className="text-gray-600 dark:text-gray-400 mb-6 text-sm">
                Components working together
              </p>

              <div className="bg-gray-50 dark:bg-gray-900 rounded-lg p-4 max-w-md">
                <div className="flex items-start justify-between mb-3">
                  <div className="flex items-center gap-2">
                    <Avatar className="h-6 w-6">
                      <AvatarFallback name="Task Owner" />
                    </Avatar>
                    <span className="text-sm text-gray-600 dark:text-gray-400">
                      John Doe
                    </span>
                  </div>
                  <Badge status="inProgress" size="sm" icon={<Clock className="h-3 w-3" />}>
                    In Progress
                  </Badge>
                </div>

                <h3 className="font-semibold text-gray-900 dark:text-white mb-2">
                  Design new landing page
                </h3>

                <p className="text-sm text-gray-600 dark:text-gray-400 mb-4">
                  Create a modern landing page with hero section and features
                </p>

                <div className="flex items-center justify-between">
                  <Tooltip delayDuration={200}>
                    <TooltipTrigger asChild>
                      <Button variant="ghost" size="sm" className="gap-1.5">
                        <Info className="h-3.5 w-3.5" />
                        Details
                      </Button>
                    </TooltipTrigger>
                    <TooltipContent>
                      Due: Jan 10, 2026
                    </TooltipContent>
                  </Tooltip>

                  <div className="flex gap-2">
                    <Button size="sm" variant="secondary">Edit</Button>
                    <Button size="sm" variant="primary">Complete</Button>
                  </div>
                </div>
              </div>
            </section>

            {/* Footer */}
            <footer className="text-center text-sm text-gray-500 dark:text-gray-400 py-8">
              <p>ClickUp Design System Phase 02 - Component Showcase</p>
              <p className="mt-1">Test in both light and dark modes</p>
            </footer>
          </div>
        </div>
      </div>
    </TooltipProvider>
  )
}
