# MyFood - React Frontend

A modern React application for managing food items with authentication.

## Features

- **User Authentication**: Login and registration with JWT tokens
- **Food Management**: Create, read, update, and delete food items
- **Modern UI**: Built with Tailwind CSS for a clean, responsive design
- **Real-time Updates**: Table automatically updates after CRUD operations
- **Type-Safe**: Full TypeScript support

## Tech Stack

- **React 18+** - UI library
- **Vite** - Build tool and dev server
- **TypeScript** - Type safety
- **Tailwind CSS v4** - Styling
- **Axios** - HTTP client
- **React Hook Form** - Form handling
- **React Router** - Client-side routing
- **Lucide React** - Icons

## Prerequisites

- Node.js (v16 or higher)
- npm or yarn
- Backend API running on `http://localhost:8080`

## Setup

1. Install dependencies:
```bash
npm install
```

2. Configure the API endpoint in `.env.local`:
```
VITE_API_URL=http://localhost:8080
```

3. Start the development server:
```bash
npm run dev
```

The application will be available at `http://localhost:5173`

## Usage

### Login/Registration
- Open the app and you'll be redirected to the login page
- Click the "Register" tab to create a new account
- Enter username, email, and password
- Once registered, you can login with your credentials

### Food Management
- After login, you'll see the food management page
- **View**: All foods are displayed in a table
- **Add**: Click "Add Food" button to create a new food item
- **Edit**: Click the pencil icon to edit an existing food item
- **Delete**: Click the trash icon to delete a food item (requires confirmation)

## Available Scripts

```bash
# Start development server
npm run dev

# Build for production
npm run build

# Preview production build locally
npm run preview

# Type check
npm run type-check
```

## Project Structure

```
client/
├── src/
│   ├── components/       # Reusable React components
│   │   ├── FoodModal.tsx
│   │   ├── FoodTable.tsx
│   │   └── Navbar.tsx
│   ├── context/          # React Context for global state
│   │   └── AuthContext.tsx
│   ├── pages/            # Page-level components
│   │   ├── LoginPage.tsx
│   │   └── FoodPage.tsx
│   ├── services/         # API and business logic
│   │   ├── api.ts
│   │   ├── authService.ts
│   │   └── foodService.ts
│   ├── types/            # TypeScript type definitions
│   │   └── index.ts
│   ├── App.tsx           # Main app component with routing
│   ├── App.css
│   ├── main.tsx
│   └── index.css
├── public/               # Static assets
├── package.json
├── tsconfig.json
├── vite.config.ts
├── postcss.config.js
├── tailwind.config.js
└── .env.local           # Environment variables
```

## API Endpoints

The app connects to:
- **Base URL**: `http://localhost:8080/api`
- **Auth**: `/authenticate/login`, `/authenticate/register`
- **Foods**: `/v1/foods` (GET, POST), `/v1/foods/{id}` (GET, PUT, DELETE)

All food endpoints require JWT authentication via the `Authorization: Bearer {token}` header.

## Authentication

- JWT tokens are stored in `localStorage`
- Tokens are automatically included in all API requests
- On 401 response, the user is redirected to login
- Logout clears the token and redirects to login page

## Error Handling

- API errors are displayed as toast messages
- Form validation errors are shown inline
- Network errors are handled gracefully
- 401 responses automatically redirect to login

## Development Notes

- TypeScript strict mode is enabled
- Tailwind CSS v4 is used with PostCSS
- React Router v6 is used for navigation
- Type-only imports are enforced for types

## Building for Production

```bash
npm run build
```

This creates an optimized production build in the `dist` folder.

## Troubleshooting

### Cannot connect to API
- Ensure the backend server is running on port 8080
- Check the `VITE_API_URL` in `.env.local`
- Check browser console for CORS errors

### Login not working
- Verify backend is running
- Check credentials are correct
- Look for error messages in the notification area

### Foods not loading
- Ensure you're logged in
- Check browser DevTools Network tab
- Verify backend is running and accessible

## Future Improvements

- Add pagination to food list
- Add search/filter functionality
- Add user profile page
- Add food categories
- Add nutritional information
- Add image uploads for foods
- Add recipe suggestions
