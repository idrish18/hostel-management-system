# Frontend Setup Guide

This document covers the initial setup of the Hostel Management System frontend application.

## ✅ Setup Completed

### 1. Environment Variables (.env.local)
Created `.env.local` file with the following configuration:
```env
NEXT_PUBLIC_API_URL=http://localhost:5000/api
NEXT_PUBLIC_APP_URL=http://localhost:3000
NODE_ENV=development
```

**Usage**: These variables are automatically available in the frontend and can be accessed via `process.env.NEXT_PUBLIC_*`

---

### 2. ESLint Configuration (.eslintrc.json)
Configured ESLint with:
- Next.js core web vitals rules
- TypeScript support
- React Hooks validation
- Unused variables detection
- Console warnings for debugging

**Scripts**:
```bash
npm run lint           # Check for linting issues
npm run lint:fix      # Automatically fix issues
```

---

### 3. Prettier Configuration (.prettierrc.json)
Configured code formatter with:
- 2-space indentation
- Single quotes
- 100 character line width
- Trailing commas in ES5
- Tailwind CSS class sorting plugin

**.prettierignore** file includes common ignore patterns (node_modules, .next, etc.)

**Scripts**:
```bash
npm run format        # Format all files
npm run format:check  # Check formatting without changing
```

---

### 4. Shadcn/UI Setup
- **components.json**: Configuration for shadcn/ui component library
- **components/ui/button.tsx**: Example base component (modify as needed)
- **lib/utils.ts**: Utility functions including `cn()` for Tailwind class merging

**To add new shadcn/ui components** (after npm install):
```bash
npx shadcn-ui@latest add [component-name]
```

Examples:
```bash
npx shadcn-ui@latest add card
npx shadcn-ui@latest add form
npx shadcn-ui@latest add input
npx shadcn-ui@latest add dialog
```

---

### 5. Folder Structure Created

```
frontend/
├── app/                    # Next.js app directory
├── components/
│   ├── ui/               # shadcn/ui components
│   ├── auth/             # Authentication components
│   └── shared/           # Shared components (optional)
├── lib/
│   ├── utils.ts          # Utility functions (cn, formatDate, etc.)
│   └── api-client.ts     # API client with auth headers
├── hooks/
│   └── useApi.ts         # Custom React Query hooks
├── types/
│   └── index.ts          # TypeScript type definitions
├── public/               # Static assets
├── .env.local           # Environment variables
├── .eslintrc.json       # ESLint configuration
├── .prettierrc.json     # Prettier configuration
├── .prettierignore      # Prettier ignore patterns
├── components.json      # shadcn/ui configuration
└── package.json         # Dependencies & scripts
```

---

### 6. Dependencies Installed

#### Core
- `next` 16.2.2 - React framework
- `react` 19.2.4 - UI library
- `react-dom` 19.2.4 - DOM rendering

#### Form & Validation
- `react-hook-form` ^7.50.0 - Form state management
- `@hookform/resolvers` ^3.3.4 - Form validation resolvers
- `zod` ^3.22.4 - Schema validation

#### State Management & API
- `@tanstack/react-query` ^5.28.0 - Server state management
- `class-variance-authority` ^0.7.0 - Component variant generator
- `clsx` ^2.0.0 - Classname utility
- `tailwind-merge` ^2.2.0 - Tailwind class merging
- `sonner` ^1.3.1 - Toast notifications

#### Dev Dependencies
- `eslint` ^9 - Code linter
- `prettier` ^3.1.1 - Code formatter
- `prettier-plugin-tailwindcss` ^0.5.11 - Tailwind class sorting

---

## 🚀 Next Steps

### 1. Install Dependencies
```bash
npm install
```

### 2. Start Development Server
```bash
npm run dev
```
Open [http://localhost:3000](http://localhost:3000)

### 3. Create Authentication Pages
- [ ] Login page at `app/auth/login/page.tsx`
- [ ] Register page at `app/auth/register/page.tsx`
- [ ] Create auth context for state management

### 4. Setup API Integration
- Import and use `apiClient` from `lib/api-client.ts`
- Use `useApi` hook for queries and `useApiMutation` for mutations
- Token is automatically added to headers

### 5. Add More shadcn/ui Components
```bash
# Common components to add:
npx shadcn-ui@latest add card
npx shadcn-ui@latest add form
npx shadcn-ui@latest add input
npx shadcn-ui@latest add select
npx shadcn-ui@latest add dialog
npx shadcn-ui@latest add table
npx shadcn-ui@latest add tabs
npx shadcn-ui@latest add badge
npx shadcn-ui@latest add alert
```

### 6. Project Configuration Tips

#### TypeScript Paths
The `tsconfig.json` should include alias paths for easier imports:
```json
{
  "compilerOptions": {
    "paths": {
      "@/*": ["./*"],
      "@/components": ["./components"],
      "@/lib": ["./lib"],
      "@/hooks": ["./hooks"],
      "@/types": ["./types"]
    }
  }
}
```

#### ESLint Rules
Override specific rules in `.eslintrc.json` if needed:
```json
{
  "rules": {
    "react/react-in-jsx-scope": "off",
    "@typescript-eslint/no-unused-vars": ["warn", { "argsIgnorePattern": "^_" }]
  }
}
```

---

## 📋 Useful Commands

```bash
# Development
npm run dev              # Start dev server
npm run build            # Production build
npm run start            # Start production server

# Code Quality
npm run lint             # Check for lint issues
npm run lint:fix         # Fix lint issues
npm run format           # Format all files
npm run format:check     # Check formatting

# Component Library
npx shadcn-ui@latest add [component]  # Add shadcn component
```

---

## 🔐 API Client Usage

### Setup
The API client is configured in `lib/api-client.ts` with:
- Base URL from `NEXT_PUBLIC_API_URL`
- Automatic Authorization header with JWT token
- Error handling and token expiration management

### Basic Usage
```typescript
import { apiClient } from '@/lib/api-client';

// GET request
const response = await apiClient.get('/students');

// POST request
const response = await apiClient.post('/students', { 
  name: 'John Doe',
  email: 'john@example.com' 
});

// PUT/PATCH/DELETE
await apiClient.put('/students/1', { name: 'Jane' });
await apiClient.patch('/students/1', { email: 'jane@example.com' });
await apiClient.delete('/students/1');
```

### With React Query
```typescript
import { useApi, useApiMutation } from '@/hooks/useApi';

// Query
const { data, isLoading, error } = useApi<Student[]>(['students'], '/students');

// Mutation
const createStudent = useApiMutation('/students', 'post', ['students']);
createStudent.mutate({ name: 'John' });
```

---

## 🎨 Shadcn/UI Usage

### Basic Button
```tsx
import { Button } from '@/components/ui/button';

export function MyComponent() {
  return (
    <>
      <Button>Default</Button>
      <Button variant="outline">Outline</Button>
      <Button variant="destructive">Delete</Button>
      <Button size="sm">Small</Button>
    </>
  );
}
```

### Form with Zod Validation
```tsx
'use client';

import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';

const schema = z.object({
  email: z.string().email(),
  password: z.string().min(8),
});

type FormData = z.infer<typeof schema>;

export function LoginForm() {
  const form = useForm<FormData>({
    resolver: zodResolver(schema),
  });

  return (
    <form onSubmit={form.handleSubmit((data) => {
      // Handle submission
    })}>
      <input {...form.register('email')} />
      {form.formState.errors.email && <span>{form.formState.errors.email.message}</span>}
    </form>
  );
}
```

---

## ⚠️ Important Notes

1. **Token Management**: JWT tokens are stored in localStorage and automatically added to API headers
2. **Token Expiration**: If a 401 response is received, the token is cleared and user is redirected to login
3. **CORS**: Ensure backend allows requests from the frontend URL
4. **Environment Variables**: All `NEXT_PUBLIC_*` variables are exposed to the browser
5. **Build**: Run `npm run build` before production deployment

---

## 🐛 Troubleshooting

### ESLint Issues
If ESLint won't run, check:
1. Node version (should be 18+)
2. `.eslintrc.json` syntax
3. Run `npm install` to ensure dependencies are installed

### Prettier Conflicts
If Prettier conflicts with ESLint:
1. Ensure both `.eslintrc.json` and `.prettierrc.json` are valid JSON
2. Run `npm run lint:fix && npm run format`

### API Connection Issues
1. Verify `NEXT_PUBLIC_API_URL` in `.env.local`
2. Check backend is running on the correct port
3. Verify CORS is enabled on backend

---

For more help, refer to the project README.md in the root directory.
