# ECommerce Client - React Application

A modern React application built with Redux Toolkit, Tailwind CSS, and Vite for the ECommerce platform.

## Features

- ✅ User Authentication (Login & Registration)
- ✅ Redux Toolkit for state management
- ✅ Tailwind CSS for responsive, modern UI
- ✅ React Router for navigation
- ✅ Protected routes
- ✅ Form validation
- ✅ API integration with fetch
- ✅ Responsive design (mobile-first)

## Project Structure

```
client/
├── src/
│   ├── components/
│   │   ├── auth/
│   │   │   ├── Login.jsx          # Login component
│   │   │   └── Register.jsx       # Registration component
│   │   ├── common/
│   │   │   └── ProtectedRoute.jsx # Protected route wrapper
│   │   └── dashboard/
│   │       └── Dashboard.jsx      # Dashboard component
│   ├── hooks/
│   │   └── redux.js               # Redux hooks
│   ├── services/
│   │   └── api/
│   │       ├── apiClient.js       # Base API client
│   │       └── authApi.js         # Auth API endpoints
│   ├── store/
│   │   ├── slices/
│   │   │   └── authSlice.js       # Auth Redux slice
│   │   └── store.js               # Redux store configuration
│   ├── App.jsx                    # Main app component
│   ├── main.jsx                   # Entry point
│   └── index.css                  # Global styles
├── index.html
├── package.json
├── vite.config.js
├── tailwind.config.js
└── postcss.config.js
```

## Getting Started

### Prerequisites

- Node.js (v18 or higher)
- npm or yarn
- Backend API running on `http://localhost:5176`

### Installation

1. Navigate to the client directory:
```bash
cd client
```

2. Install dependencies:
```bash
npm install
```

### Development

Start the development server:
```bash
npm run dev
```

The application will be available at `http://localhost:3000`

### Build

Build for production:
```bash
npm run build
```

### Preview Production Build

Preview the production build:
```bash
npm run preview
```

## Configuration

### API Base URL

The API base URL is configured in `src/services/api/apiClient.js`. By default, it points to `http://localhost:5176`.

You can override this by setting the `VITE_API_BASE_URL` environment variable:

```bash
VITE_API_BASE_URL=http://localhost:5176 npm run dev
```

Or create a `.env` file:
```
VITE_API_BASE_URL=http://localhost:5176
```

## Features Overview

### Authentication

- **Login**: Email and password authentication
- **Registration**: User registration with validation
- **Token Management**: Automatic token storage and retrieval
- **Protected Routes**: Routes that require authentication

### State Management

- Redux Toolkit for centralized state management
- Auth slice handles authentication state
- Persistent authentication (localStorage)

### UI/UX

- Responsive design (mobile, tablet, desktop)
- Modern gradient backgrounds
- Form validation with error messages
- Loading states
- User-friendly error handling

## API Endpoints

The application uses the following API endpoints:

- `POST /api/auth/login` - User login
- `POST /api/auth/register` - User registration

## Technologies Used

- **React 18** - UI library
- **Redux Toolkit** - State management
- **React Router** - Routing
- **Tailwind CSS** - Styling
- **Vite** - Build tool
- **Fetch API** - HTTP requests

## Browser Support

- Chrome (latest)
- Firefox (latest)
- Safari (latest)
- Edge (latest)

## License

This project is part of the ECommerce Demo solution.
