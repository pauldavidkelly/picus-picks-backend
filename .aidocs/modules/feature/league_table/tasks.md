# League Table Implementation Tasks

## Backend Tasks
1. [x] Create LeagueTableStats DTO
   - Add properties for username, correct picks, total picks, success rate
   - Add data annotations for validation/formatting

2. [x] Create LeagueTableService
   - Add GetLeagueTableStats method
   - Implement stats calculation logic
   - Add sorting by success rate

3. [x] Add LeagueTableService to DI container
   - Register in Program.cs
   - Configure scoped service

## Frontend Tasks
1. [x] Create LeagueTable.razor component
   - Add table structure
   - Add sorting functionality
   - Implement error handling
   - Add loading state

2. [x] Update Home.razor
   - Add LeagueTable component
   - Handle component lifecycle

3. [x] Add CSS styling
   - Style table layout
   - Add responsive design
   - Style success rate indicators

## Testing Tasks
1. [x] Implement LeagueTableService unit tests
   - Test stats calculation
   - Test edge cases
   - Test sorting

2. [x] Implement LeagueTable component tests
   - Test rendering
   - Test sorting
   - Test empty states

3. [x] Add integration tests
   - Test home page integration
