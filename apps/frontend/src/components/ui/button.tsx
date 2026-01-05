import * as React from "react"
import { Slot } from "@radix-ui/react-slot"
import { cva, type VariantProps } from "class-variance-authority"

import { cn } from "@/lib/utils"

/**
 * Button Component
 *
 * ClickUp-inspired button with multiple variants and sizes.
 * Supports hover/active scales, shadows, and full keyboard navigation.
 *
 * @example
 * ```tsx
 * <Button variant="primary" size="md">Click me</Button>
 * <Button variant="secondary" size="lg">Large Button</Button>
 * <Button variant="ghost" size="icon"><Icon /></Button>
 * ```
 */

const buttonVariants = cva(
  "inline-flex items-center justify-center gap-2 whitespace-nowrap rounded-md text-sm font-medium transition-all duration-fast focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-primary/50 disabled:pointer-events-none disabled:opacity-50 [&_svg]:pointer-events-none [&_svg]:size-4 [&_svg]:shrink-0",
  {
    variants: {
      variant: {
        // ClickUp primary - purple gradient with shadow
        primary: "bg-primary text-primary-foreground shadow-sm hover:shadow-md hover:scale-[1.02] active:scale-[0.98]",

        // ClickUp secondary - white with border
        secondary: "bg-white border-2 border-gray-200 text-gray-900 hover:bg-gray-50 hover:border-gray-300",

        // Ghost - transparent background
        ghost: "bg-transparent text-gray-600 hover:bg-gray-100 hover:text-gray-900",

        // Destructive - red for errors
        destructive: "bg-error text-white shadow-sm hover:bg-error/90",

        // Outline - minimal border
        outline: "border-2 border-gray-200 bg-transparent hover:bg-gray-50 hover:text-gray-900",

        // Link - text only
        link: "text-primary underline-offset-4 hover:underline",
      },
      size: {
        sm: "h-9 px-3 text-xs",
        md: "h-10 px-4",
        lg: "h-11 px-6 text-base",
        icon: "h-10 w-10",
      },
    },
    defaultVariants: {
      variant: "primary",
      size: "md",
    },
  }
)

/**
 * Props for Button component.
 *
 * @extends React.ButtonHTMLAttributes<HTMLButtonElement>
 * @property {boolean} [asChild=false] - Render as child component (Radix Slot pattern)
 * @property {'primary'|'secondary'|'ghost'|'destructive'|'outline'|'link'} [variant='primary'] - Visual style variant
 * @property {'sm'|'md'|'lg'|'icon'} [size='md'] - Button size
 */
export interface ButtonProps
  extends React.ButtonHTMLAttributes<HTMLButtonElement>,
    VariantProps<typeof buttonVariants> {
  asChild?: boolean
}

const Button = React.forwardRef<HTMLButtonElement, ButtonProps>(
  ({ className, variant, size, asChild = false, ...props }, ref) => {
    const Comp = asChild ? Slot : "button"
    return (
      <Comp
        className={cn(buttonVariants({ variant, size, className }))}
        ref={ref}
        {...props}
      />
    )
  }
)
Button.displayName = "Button"

export { Button, buttonVariants }
