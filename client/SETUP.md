# Quick Setup Guide

## Step 1: Install Dependencies

```bash
cd client
npm install
```

## Step 2: Start Development Server

```bash
npm run dev
```

The app will start on `http://localhost:3000`

## Step 3: Ensure Backend is Running

Make sure your .NET API is running on `http://localhost:5176` (or update the API_BASE_URL in `src/services/api/apiClient.js`)

## Step 4: Test the Application

1. Navigate to `http://localhost:3000`
2. You'll be redirected to the login page
3. Click "Sign up" to create a new account
4. Fill in the registration form
5. After successful registration, you'll be logged in and redirected to the dashboard
6. You can logout and login again with your credentials

## Troubleshooting

### API Connection Issues

If you're getting API connection errors:
1. Verify the backend API is running
2. Check the API URL in `src/services/api/apiClient.js`
3. Check CORS settings on the backend (should allow `http://localhost:3000`)

### Port Already in Use

If port 3000 is already in use, Vite will automatically use the next available port. Check the terminal output for the actual port.

### Build Errors

If you encounter build errors:
1. Delete `node_modules` folder
2. Delete `package-lock.json`
3. Run `npm install` again
