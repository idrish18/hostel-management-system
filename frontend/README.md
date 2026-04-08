# Hostel Management System - Frontend

This is the Next.js frontend application for the Hostel Management System, providing a complete web interface for managing hostels, rooms, students, complaints, and fees.

## 📋 PROJECT SETUP CHECKLIST

### ✅ 1. INITIAL SETUP
- [x] Initialize Next.js (App Router + TypeScript)
- [x] Setup Tailwind CSS and shadcn
- [x] Setup folder structure (app, components, lib, hooks, types)
- [x] Configure environment variables (API base URL)
- [x] Setup ESLint & Prettier

### 🔐 2. AUTHENTICATION
- [ ] Create Login page
- [ ] Create Register page
- [ ] Integrate JWT login API
- [ ] Store token (cookie or localStorage)
- [ ] Setup API client with Authorization header
- [ ] Implement logout functionality
- [ ] Create auth context/store (user + role)
- [ ] Protect routes using middleware
- [ ] Role-based access (Admin / Student / Worker)
- [ ] Handle token expiration

### 🧭 3. ROUTING & LAYOUT
- [ ] Setup route groups (auth, dashboard)
- [ ] Create dashboard layout (sidebar + navbar)
- [ ] Implement protected routes
- [ ] Add navigation menu
- [ ] Add active route highlighting
- [ ] Make layout responsive (mobile support)

### 📊 4. DASHBOARD
- [ ] Dashboard summary cards (students, rooms, fees, complaints)
- [ ] Recent complaints widget
- [ ] Alerts section
- [ ] Fetch data from dashboard APIs
- [ ] Add loading & error states

### 🏢 5. HOSTEL MANAGEMENT
- [ ] Hostel list page
- [ ] Create hostel form
- [ ] Edit hostel form
- [ ] Delete hostel action
- [ ] Connect all hostel APIs
- [ ] Add validation & error handling

### 🚪 6. ROOM MANAGEMENT
- [ ] Rooms list by hostel
- [ ] Available rooms filter
- [ ] Room details view
- [ ] Create room form
- [ ] Update room form
- [ ] Delete room action
- [ ] Show occupancy metrics
- [ ] Connect all room APIs

### 🎓 7. STUDENT MANAGEMENT
- [ ] Students list page
- [ ] Student details view
- [ ] Unassigned students filter
- [ ] Students by room view
- [ ] Assign room functionality
- [ ] Unassign room functionality
- [ ] Connect all student APIs

### 📝 8. COMPLAINT MANAGEMENT
- [ ] Create complaint form (student)
- [ ] Complaint list (admin)
- [ ] Filter by status
- [ ] Complaint details view
- [ ] Update complaint status
- [ ] Delete complaint
- [ ] Show timestamps & days open
- [ ] Connect all complaint APIs

### 💰 9. FEE MANAGEMENT
- [ ] Record fee form
- [ ] Fee list (student & admin)
- [ ] Pending fees view
- [ ] Overdue fees view
- [ ] Fee details page
- [ ] Record payment flow
- [ ] Receipt view
- [ ] Status indicators (Paid/Pending/Overdue)
- [ ] Connect all fee APIs

### 🔁 10. API & STATE MANAGEMENT
- [ ] Setup API service layer
- [ ] Configure React Query
- [ ] Create query hooks (per feature)
- [ ] Create mutation hooks
- [ ] Handle loading states
- [ ] Handle error states
- [ ] Cache & refetch strategy

### 🎨 11. UI COMPONENTS
- [ ] Reusable table component
- [ ] Reusable form components
- [ ] Modal component
- [ ] Button & input components
- [ ] Status badges (success/warning/error)
- [ ] Loader/spinner
- [ ] Toast notifications

### 📋 12. FORMS & VALIDATION
- [ ] Setup React Hook Form
- [ ] Setup Zod validation
- [ ] Validate all forms (auth, hostel, room, etc.)
- [ ] Show inline validation errors
- [ ] Handle submission states

### ⚡ 13. PERFORMANCE
- [ ] Use server components where possible
- [ ] Lazy load heavy pages
- [ ] Implement pagination (if needed)
- [ ] Add debounced search (optional)
- [ ] Optimize re-renders

### 🔔 14. OPTIONAL FEATURES
- [ ] Global search
- [ ] Filters & sorting
- [ ] Export to CSV/Excel
- [ ] Charts for analytics
- [ ] Notifications system
- [ ] Dark mode

### 🧪 15. TESTING
- [ ] Test login flow
- [ ] Test protected routes
- [ ] Test CRUD operations
- [ ] Test API error handling
- [ ] Test role-based access

### 🚀 16. DEPLOYMENT
- [ ] Build production app
- [ ] Configure environment variables
- [ ] Deploy to Vercel
- [ ] Connect backend API
- [ ] Test production build
- [ ] Enable HTTPS

---

## Project Structure

```
frontend/
├── app/
│   ├── layout.tsx
│   ├── page.tsx
│   └── globals.css
├── components/
│   └── ui/
├── lib/
├── hooks/
├── types/
└── public/
```

## Getting Started

### Prerequisites
- Node.js 18+ 
- npm or yarn

### Installation

```bash
# Install dependencies
npm install

# Setup environment variables
cp .env.example .env.local

# Run development server
npm run dev
```

Open [http://localhost:3000](http://localhost:3000) to view the app.

## Available Scripts

```bash
npm run dev      # Start development server
npm run build    # Build for production
npm run start    # Start production server
npm run lint     # Run ESLint
```

## Tech Stack

- **Framework**: Next.js 15+ (App Router)
- **Language**: TypeScript
- **Styling**: Tailwind CSS
- **UI Components**: shadcn/ui
- **Forms**: React Hook Form
- **Validation**: Zod
- **State Management**: React Query / TanStack Query
- **HTTP Client**: Fetch API / Axios

## Environment Variables

Create a `.env.local` file in the frontend directory:

```env
NEXT_PUBLIC_API_URL=http://localhost:5000/api
NEXT_PUBLIC_APP_URL=http://localhost:3000
```

## Development Guidelines

- Follow TypeScript best practices
- Use functional components with hooks
- Keep components small and reusable
- Use Tailwind utility classes for styling
- Document complex logic with comments
- Test components and features thoroughly
