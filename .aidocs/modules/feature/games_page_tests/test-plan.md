# Games Page Test Plan

## GamesService Tests

### GetGamesByWeekAndSeasonAsync Tests
1. Returns games successfully when API call succeeds
   - Arrange: Mock HttpClient to return successful response with sample games
   - Act: Call GetGamesByWeekAndSeasonAsync
   - Assert: Verify returned games match expected data

2. Handles missing authentication token
   - Arrange: Mock HttpContextAccessor to return null token
   - Act: Call GetGamesByWeekAndSeasonAsync
   - Assert: Verify InvalidOperationException is thrown

3. Handles HTTP error responses
   - Arrange: Mock HttpClient to return error status code
   - Act: Call GetGamesByWeekAndSeasonAsync
   - Assert: Verify HttpRequestException is thrown

### SyncGamesAsync Tests
1. Successfully syncs games
   - Arrange: Mock HttpClient to return successful response
   - Act: Call SyncGamesAsync
   - Assert: Verify returned games match expected data

2. Handles authentication errors
   - Arrange: Mock HttpClient to return 401 status
   - Act: Call SyncGamesAsync
   - Assert: Verify appropriate exception is thrown

## Games Page Component Tests

### Initial Load Tests
1. Shows loading state initially
   - Arrange: Create component with mocked GamesService
   - Act: Render component
   - Assert: Verify loading spinner is displayed

2. Displays games after loading
   - Arrange: Mock GamesService to return sample games
   - Act: Render component
   - Assert: Verify games are displayed correctly

### Week Selection Tests
1. Updates games when week changes
   - Arrange: Mock GamesService and render component
   - Act: Change selected week
   - Assert: Verify GamesService is called with new week

### Error Handling Tests
1. Displays error message on load failure
   - Arrange: Mock GamesService to throw error
   - Act: Render component
   - Assert: Verify error message is displayed

2. Displays error message on sync failure
   - Arrange: Mock SyncGamesAsync to throw error
   - Act: Click sync button
   - Assert: Verify error message is displayed

### Game Display Tests
1. Displays game information correctly
   - Arrange: Mock service with sample game data
   - Act: Render component
   - Assert: Verify all game details are displayed correctly
   - Check team names, logos, scores, times, and location

## Test Data Requirements
- Sample game data with various states (completed/upcoming)
- Sample team data with all required fields
- Various error responses from API
- Authentication token scenarios 