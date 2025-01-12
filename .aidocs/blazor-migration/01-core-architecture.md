# Picus Picks Blazor Migration - Core Architecture

## Component Structure
```
App
├── Layout
│   ├── NavBar (Auth status, user info)
│   └── MainLayout
├── Pages
│   ├── Home
│   ├── Games
│   │   ├── GamesGrid
│   │   ├── GameCard
│   │   └── WeekSelector
│   └── Picks
│       ├── PicksList
│       └── PickDetails
└── Shared
    ├── Authentication
    └── Loading

```

## Core Features
1. Authentication (Auth0)
   - Login/Logout
   - Token management
   - Protected routes

2. Games Management
   - Display weekly games grid
   - Week selection
   - Game status updates
   - Admin: Sync games with external API

3. Picks System
   - Make picks for games
   - View/edit existing picks
   - Weekly stats
   
## Data Flow
1. Authentication Flow:
   - Auth0 login → Token stored → Automatic token refresh
   - Token added to API requests via DelegatingHandler

2. Games Flow:
   - Load games by week
   - Real-time status updates
   - Pick submission validation

## State Management
Use Blazor's built-in state management:
- AuthenticationStateProvider for auth state
- Cascading parameters for shared state
- Local component state for UI
