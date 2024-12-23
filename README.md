# Picus NFL Picks

A modern NFL picks application that allows users to make predictions on NFL games, compete in leagues, and track their performance. Built with .NET Core and React, this application provides a clean, user-friendly interface for managing your NFL picks.

## Features

- Secure authentication with Auth0
- Real-time game data from ESPN API
- League management and standings
- Pick submission with deadline enforcement
- Performance statistics and tracking
- Modern, responsive user interface

## Technology Stack

### Backend
- .NET Core Web API
- PostgreSQL database
- Entity Framework Core
- Auth0 authentication

### Frontend
- React
- Tailwind CSS
- shadcn/ui components
- TypeScript

## Getting Started

Detailed setup instructions can be found in the following documentation:

- [Development Guide](docs/HOW-TO.md)
- [Architecture Overview](docs/ARCHITECTURE.md)
- [Development Plan](docs/PLAN.md)

## Project Structure

```
picus-picks/
├── backend/               # .NET Core Web API project
│   ├── src/              # Source code
│   └── tests/            # Unit and integration tests
├── frontend/             # React application
│   ├── src/              # Source code
│   └── tests/            # Unit and E2E tests
├── docs/                 # Project documentation
└── .github/              # GitHub workflows and templates
```

## Development Status

This project is currently in active development. Check the [Development Plan](docs/PLAN.md) for the current status and upcoming features.

## Contributing

While this is primarily a learning project, suggestions and feedback are welcome. Please create an issue to discuss any changes you'd like to propose.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.