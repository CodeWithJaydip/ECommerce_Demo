# shadcn/ui Setup Guide

## ✅ Setup Complete

shadcn/ui has been successfully integrated into your React project!

## Installed Packages

- `class-variance-authority` - For variant-based styling
- `clsx` - For conditional class names
- `tailwind-merge` - For merging Tailwind classes
- `lucide-react` - Icon library
- `tailwindcss-animate` - Animation utilities

## Configuration Files

1. **`components.json`** - shadcn/ui configuration
   - Style: default
   - Tailwind CSS variables enabled
   - Path aliases configured

2. **`tailwind.config.js`** - Updated with shadcn/ui theme
   - CSS variables for theming
   - Dark mode support
   - Animation utilities

3. **`src/index.css`** - CSS variables defined
   - Light and dark theme variables
   - Base styles

4. **`src/lib/utils.js`** - Utility functions
   - `cn()` function for merging classes

## Adding Components

You can now add shadcn/ui components using the CLI or manually:

### Using CLI (Recommended)
```bash
npx shadcn@latest add button
npx shadcn@latest add input
npx shadcn@latest add card
```

### Manual Installation
1. Copy component code from [shadcn/ui website](https://ui.shadcn.com)
2. Place in `src/components/ui/`
3. Import and use in your components

## Usage Example

```jsx
import { Button } from "@/components/ui/button"
import { Card } from "@/components/ui/card"
import { cn } from "@/lib/utils"

function MyComponent() {
  return (
    <Card className={cn("p-6")}>
      <Button>Click me</Button>
    </Card>
  )
}
```

## Path Aliases

The following aliases are configured:
- `@/components` → `src/components`
- `@/lib` → `src/lib`
- `@/hooks` → `src/hooks`
- `@/components/ui` → `src/components/ui`

Note: You may need to configure these in `vite.config.js` if not already done.

## Documentation

- [shadcn/ui Documentation](https://ui.shadcn.com)
- [Components](https://ui.shadcn.com/docs/components)
- [Theming](https://ui.shadcn.com/docs/theming)

## Next Steps

1. Add components you need using `npx shadcn@latest add [component-name]`
2. Start using shadcn/ui components in your forms and UI
3. Customize the theme colors in `tailwind.config.js` and `index.css`
