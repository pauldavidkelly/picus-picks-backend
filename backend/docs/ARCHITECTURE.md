# Picus NFL Picks App - Architecture Overview

This document provides a high-level overview of the Picus NFL Picks app architecture in simple terms.

## System Overview

Picus is a self-hosted NFL picks application that allows users to make predictions about NFL games. The application consists of:

1. A C# Web API Backend: Handles all business logic, data storage, and external API communications
2. A React Frontend: Provides an intuitive user interface for making and viewing picks
3. A PostgreSQL Database: Stores all our application data

## Key Features

### Pick Management
- Users can make money line picks for upcoming games
- Picks are hidden from other users until the game deadline
- After deadline, picks are displayed in a table format:
  - Players' names as column headers
  - Games listed down the side
  - Team badges/icons showing each player's pick
- Future expansion planned for spread and over/under picks

### Dashboard
The user dashboard shows:
- Upcoming games requiring picks
- Recent pick results
- Performance statistics
- League standings

## Technology Stack

### Backend
- C# with .NET Core Web API
- Entity Framework Core for database access
- LINQ for querying data
- Auth0 for authentication

### Frontend
- React for the user interface
- Auth0 React SDK for authentication
- A component library (to be decided) for UI elements

### Database
- PostgreSQL for data storage
- Structured to support:
  - User data and authentication
  - Game schedules and results
  - Player picks and deadlines
  - League information

## Application Structure

Our application follows a simplified clean architecture with three main parts:

### 1. API Layer (Controllers)
Responsibilities:
- Handle HTTP requests
- Validate incoming data
- Route requests to appropriate services
- Format responses

Example Controller Structure:
```csharp
public class PicksController : ControllerBase
{
    private readonly IPickService _pickService;
    
    // Handle getting picks
    [HttpGet]
    public async Task<IActionResult> GetPicks(int weekId)
    
    // Handle submitting picks
    [HttpPost]
    public async Task<IActionResult> SubmitPicks(PickSubmissionDto picks)
}
```

### 2. Service Layer
Responsibilities:
- Implement business logic
- Enforce pick deadlines
- Calculate standings
- Manage data access

Example Service Structure:
```csharp
public class PickService
{
    private readonly ApplicationDbContext _context;
    
    // Business logic for submitting picks
    public async Task<Result> SubmitPicks(int userId, List<Pick> picks)
    {
        // Validate deadline
        // Save picks
        // Return result
    }
}
```

### 3. Data Access Layer
Responsibilities:
- Define database structure
- Handle data operations
- Manage relationships between entities

Example Entity Structure:
```csharp
public class Pick
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int GameId { get; set; }
    public int TeamId { get; set; }
    public DateTime SubmittedAt { get; set; }
}
```

## Database Schema

### Users Table
- Id (Primary Key)
- Auth0Id (for authentication)
- Username
- Role (Admin/Player)
- LeagueId

### Games Table
- Id (Primary Key)
- ESPNGameId
- HomeTeamId
- AwayTeamId
- GameTime
- PickDeadline
- FinalScore
- Week
- Season

### Picks Table
- Id (Primary Key)
- UserId (Foreign Key)
- GameId (Foreign Key)
- SelectedTeamId
- SubmissionTime
- IsCorrect
- Points

### Leagues Table
- Id (Primary Key)
- Name
- CreatedAt
- AdminUserId

### Teams Table
- Id (Primary Key)
- ESPNTeamId
- Name
- Abbreviation
- IconUrl

## Security Considerations

### Pick Visibility
- Database queries filter picks based on game deadlines
- API endpoints enforce visibility rules
- Frontend respects these restrictions in the UI

### Authentication
- Auth0 handles user authentication
- JWT tokens secure API requests
- Role-based access controls protect admin functions

## Future Expansion Considerations

The schema and architecture support future additions:
- Spread betting
- Over/under predictions
- Historical statistics
- Additional league features

However, we're keeping the initial implementation focused on core features to maintain clarity and ease of understanding.