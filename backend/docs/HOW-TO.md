# Picus NFL Picks App - Development Guide

This comprehensive guide walks through the process of setting up, developing, testing, and deploying the Picus NFL Picks application. We'll explore not just what commands to run, but why we're running them and how they work in different environments.

## Initial Project Setup

Before beginning development, ensure you have the following tools installed:
- Node.js (for frontend development)
- .NET SDK (for backend development)
- PostgreSQL (for database)
- Visual Studio Code (recommended IDE)

## Frontend Setup

Our frontend setup process creates a modern React application with Next.js and configures it with necessary styling tools. Each step builds upon the previous one to create a robust development environment.

### Creating the Next.js Application

First, we create a new Next.js application with TypeScript support. Open your terminal in your project's frontend directory and run:

```powershell
npx create-next-app@latest frontend
```

When prompted, select the following options:
- Would you like to use TypeScript? → Yes 
- Would you like to use ESLint? → Yes 
- Would you like to use Tailwind CSS? → Yes 
- Would you like to use `src/` directory? → Yes 
- Would you like to use App Router? → Yes 
- Would you like to customize the default import alias (@/*)? → Yes 

### Installing Essential Dependencies

After the Next.js application is created, we need to install several utility packages that help with styling and UI components. Navigate to your frontend directory and run:

```powershell
cd frontend
npm install @radix-ui/react-icons
npm install clsx
npm install class-variance-authority
npm install tailwind-merge
```

### Setting up shadcn/ui

shadcn/ui provides our component library and design system. To set it up, run:

```powershell
npx shadcn-ui@latest init
```

When prompted, choose these options and here's why each matters:
1. Would you like to use TypeScript? → Yes
   (Enables better code checking and autocompletion)

2. Which style would you like to use? → New York
   (Provides a bold, modern look that works well for sports applications)

3. Which color would you like to use as base color? → Slate
   (A professional blue-gray that works well with sports content)

4. Where is your global CSS file? → app/globals.css
   (Matches our Next.js app structure)

5. Would you like to use CSS variables for colors? → Yes
   (Makes theme customization easier)

6. Where is your tailwind.config.js located? → tailwind.config.js
   (Default location for our styling configuration)

7. Configure the import alias for components? → @/components
   (Makes importing components more straightforward)

8. Configure the import alias for utilities? → @/lib/utils
   (Keeps our utility functions organized)

When prompted about React 19 compatibility, choose --legacy-peer-deps for the most stable setup.

## Backend Setup

### Setting up Entity Framework Core

First, we'll install the required NuGet packages for Entity Framework Core with PostgreSQL support. Navigate to your backend project directory and run:

```powershell
cd backend/src/Picus.Api
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
```

These packages provide:
- Entity Framework Core: Our main ORM framework
- EF Core Design: Tools for migrations and reverse engineering
- Npgsql PostgreSQL: PostgreSQL database provider for EF Core

### Creating the Data Models

Create a new Models folder in your project and add the following model classes based on our schema. Each model represents a table in our database and follows best practices for entity design:

```powershell
mkdir Models
cd Models
```

Create the following files with their respective classes. We'll implement these one by one, starting with the foundation models (Team, User) and building up to the more complex ones (Picks, Games) that depend on them.

Here's the recommended implementation order:
1. Team.cs - Foundation model for NFL teams
2. User.cs - Core user information model
3. League.cs - League/pool management model
4. Game.cs - Game scheduling and results model
5. Pick.cs - User predictions model

### Setting up the Database Context

After creating our models, we'll need to create an ApplicationDbContext class that will manage our database interactions:

```powershell
mkdir Data
cd Data
```

The DbContext will:
- Define DbSet properties for each model
- Configure model relationships
- Set up any required model configurations
- Handle connection string management

### Creating the Initial Migration

Once our models and DbContext are ready, we'll create and apply the initial migration:

```powershell
# Create the migration
dotnet ef migrations add InitialCreate

# Apply the migration to create the database
dotnet ef database update
```

### Database Connection Configuration

Configure the database connection in appsettings.json:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=picus;Username=your_username;Password=your_password"
  }
}
```

Remember to replace the username and password with your actual PostgreSQL credentials.
### Setting up Entity Framework Core

When building our backend, we start by setting up Entity Framework Core, which will handle our database operations. First, install the required NuGet packages by running these commands in your backend project directory:

```powershell
cd backend/src/Picus.Api
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
```

These packages serve different purposes in our application:
- Microsoft.EntityFrameworkCore provides the core ORM functionality
- EntityFrameworkCore.Design enables design-time tools like migrations
- Npgsql.EntityFrameworkCore.PostgreSQL allows us to work with PostgreSQL

### Implementing Data Models

Our data models form the foundation of the application. We implement them in a specific order to maintain clear dependencies between related entities. Create a Models directory in your project:

```powershell
mkdir Models
cd Models
```

Let's start with the Team model, which will store NFL team information. Create Team.cs with this content:

```csharp
using System;
using System.ComponentModel.DataAnnotations;

namespace Picus.Api.Models
{
    public class Team
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ESPNTeamId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(10)]
        public string Abbreviation { get; set; }

        [Required]
        [MaxLength(7)]
        public string PrimaryColor { get; set; }

        [MaxLength(7)]
        public string SecondaryColor { get; set; }

        [MaxLength(7)]
        public string TertiaryColor { get; set; }

        [MaxLength(255)]
        public string IconUrl { get; set; }

        [MaxLength(255)]
        public string BannerUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
```

Let's examine the key design decisions in our Team model:

The identity fields ensure each team is uniquely identifiable:
- Id serves as our primary key for database operations
- ESPNTeamId enables integration with the ESPN API for game data

The display fields help present team information consistently:
- Name stores the full team name (e.g., "San Francisco 49ers")
- Abbreviation stores the short form (e.g., "SF")
- IconUrl and BannerUrl store paths to team imagery for different display contexts

The color fields maintain team branding:
- PrimaryColor stores the main team color as a hex code
- SecondaryColor and TertiaryColor allow for complete team color schemes
- All color fields use a MaxLength of 7 to accommodate hex codes (#RRGGBB)

The audit fields help with tracking:
- CreatedAt captures when the record was first created
- UpdatedAt tracks modifications to team data

This model design supports both the data storage needs and the UI requirements of our application. The carefully chosen field types and constraints help maintain data integrity while providing flexibility for displaying team information in various contexts.

## Running the Application

After completing the setup, you can start the development server:

```powershell
# Start the frontend development server
cd frontend
npm run dev

# In a separate terminal, start the backend server (once implemented)
cd backend/src/Picus.Api
dotnet run
```