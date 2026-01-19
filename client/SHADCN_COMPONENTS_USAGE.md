# shadcn/ui Components Usage Guide

**⚠️ IMPORTANT: Project Standard**
**Always use shadcn/ui components instead of manually creating custom UI components.**

This document tracks which shadcn/ui components are available and used throughout the project.

## ✅ Available shadcn/ui Components

All components are located in `src/components/ui/`:

1. **alert-dialog.jsx** - AlertDialog, AlertDialogContent, AlertDialogHeader, AlertDialogTitle, AlertDialogDescription, AlertDialogFooter, AlertDialogAction, AlertDialogCancel
2. **button.jsx** - Button (with variants: default, destructive, outline, secondary, ghost, link)
3. **card.jsx** - Card, CardHeader, CardTitle, CardDescription, CardContent, CardFooter
4. **form.jsx** - Form, FormItem, FormLabel, FormControl, FormDescription, FormMessage
5. **input.jsx** - Input
6. **label.jsx** - Label
7. **select.jsx** - Select, SelectTrigger, SelectValue, SelectContent, SelectItem, SelectGroup, SelectLabel, SelectSeparator
8. **table.jsx** - Table, TableHeader, TableBody, TableFooter, TableHead, TableRow, TableCell, TableCaption

## 📋 Component Usage Status

### ✅ Fully Using shadcn/ui Components

- **UserManagement.jsx** - Uses: Button, Input, Card, Select, Table, AlertDialog
- **Login.jsx** - Uses: Button, Input, Label, Form components
- **Register.jsx** - Uses: Button, Input, Label, Form components
- **Dashboard.jsx** - Uses: Card, CardHeader, CardTitle, CardContent, Button
- **Landing.jsx** - Uses: Button, Card, CardContent
- **Header.jsx** - Uses: Button

### 🎯 Project Standard & Best Practices

**⚠️ MANDATORY: Always use shadcn/ui components - Never create custom UI components manually.**

1. **Always use shadcn/ui components** instead of plain HTML elements:
   - Use `<Button>` instead of `<button>`
   - Use `<Card>` instead of plain `<div>` for cards
   - Use `<Select>` instead of `<select>`
   - Use `<Table>` components instead of plain `<table>`
   - Use `<Input>` instead of `<input>`
   - Use `<Label>` instead of `<label>`

2. **Import from `@/components/ui`**:
   ```jsx
   import { Button } from '@/components/ui/button';
   import { Card, CardContent } from '@/components/ui/card';
   ```

3. **Use variants and props** provided by shadcn/ui:
   ```jsx
   <Button variant="outline" size="sm">Click me</Button>
   <Card className="custom-class">
     <CardContent>Content here</CardContent>
   </Card>
   ```

## 🔄 Adding New shadcn/ui Components

**Before creating any UI component manually, check if shadcn/ui has it available!**

To add a new shadcn/ui component:

1. **Using CLI (Recommended)**:
   ```bash
   cd client
   npx shadcn@latest add [component-name]
   ```

2. **Manual Installation** (if CLI doesn't work):
   - Copy component code from [shadcn/ui website](https://ui.shadcn.com)
   - Place in `src/components/ui/[component-name].jsx`
   - Install required Radix UI dependencies if needed

3. **If shadcn/ui doesn't have the component**:
   - Check if you can compose it using existing shadcn/ui components
   - Only create custom components as a last resort
   - Document why a custom component was necessary

## 📦 Required Dependencies

The following Radix UI packages are already installed:
- `@radix-ui/react-alert-dialog` - For AlertDialog
- `@radix-ui/react-select` - For Select

If you add new components, you may need to install additional Radix UI packages.

## 🎨 Styling

All shadcn/ui components use Tailwind CSS and follow the theme defined in:
- `tailwind.config.js` - Theme configuration
- `src/index.css` - CSS variables for theming

Components automatically use the theme colors and can be customized via className props.
