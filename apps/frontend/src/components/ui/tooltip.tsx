import * as React from "react"
import { cn } from "@/lib/utils"

const TooltipContext = React.createContext<{
  open: boolean
  setOpen: (open: boolean) => void
}>({
  open: false,
  setOpen: () => {},
})

const TooltipProvider = ({
  children,
  delayDuration = 200,
}: {
  children: React.ReactNode
  delayDuration?: number
}) => {
  return (
    <TooltipContext.Provider value={{ open: false, setOpen: () => {} }}>
      {children}
    </TooltipContext.Provider>
  )
}

const Tooltip = ({
  children,
  open: controlledOpen,
  onOpenChange,
  delayDuration = 200,
}: {
  children: React.ReactNode
  open?: boolean
  onOpenChange?: (open: boolean) => void
  delayDuration?: number
}) => {
  const [uncontrolledOpen, setUncontrolledOpen] = React.useState(false)
  const timeoutRef = React.useRef<NodeJS.Timeout | undefined>(undefined)
  const open = controlledOpen ?? uncontrolledOpen
  const setOpen = onOpenChange ?? setUncontrolledOpen

  const handleMouseEnter = () => {
    if (timeoutRef.current) clearTimeout(timeoutRef.current)
    timeoutRef.current = setTimeout(() => setOpen(true), delayDuration)
  }

  const handleMouseLeave = () => {
    if (timeoutRef.current) clearTimeout(timeoutRef.current)
    setOpen(false)
  }

  return (
    <TooltipContext.Provider value={{ open, setOpen }}>
      <div onMouseEnter={handleMouseEnter} onMouseLeave={handleMouseLeave}>
        {children}
      </div>
    </TooltipContext.Provider>
  )
}

interface TooltipTriggerProps extends React.HTMLAttributes<HTMLDivElement> {
  asChild?: boolean
}

const TooltipTrigger = React.forwardRef<
  HTMLDivElement,
  TooltipTriggerProps
>(({ className, children, asChild = false, ...props }, ref) => {
  if (asChild && React.isValidElement(children)) {
    return React.cloneElement(children as React.ReactElement<Record<string, unknown>>, {
      ...props,
      ref
    })
  }

  return (
    <div ref={ref} className={cn(className)} {...props}>
      {children}
    </div>
  )
})
TooltipTrigger.displayName = "TooltipTrigger"

const TooltipContent = React.forwardRef<
  HTMLDivElement,
  React.HTMLAttributes<HTMLDivElement>
>(({ className, children, ...props }, ref) => {
  const { open } = React.useContext(TooltipContext)

  if (!open) return null

  return (
    <div
      ref={ref}
      className={cn(
        "z-50 overflow-hidden rounded-md bg-primary px-3 py-1.5 text-xs text-primary-foreground animate-in fade-in-0 zoom-in-95",
        className
      )}
      {...props}
    >
      {children}
    </div>
  )
})
TooltipContent.displayName = "TooltipContent"

export { Tooltip, TooltipProvider, TooltipTrigger, TooltipContent }
