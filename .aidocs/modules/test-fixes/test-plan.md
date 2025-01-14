# Test Plan

## Test Cases

### GamesService Tests
1. ✅ GetGamesByWeekAndSeasonAsync_ReturnsGames_WhenApiCallSucceeds
   - Verifies successful game retrieval
   - Checks correct URL construction
   - Validates response parsing

2. ✅ GetGamesByWeekAndSeasonAsync_ThrowsException_WhenNoAuthToken
   - Verifies error handling when no auth token is available
   - Checks appropriate exception type

3. ✅ GetGamesByWeekAndSeasonAsync_ThrowsException_WhenApiReturnsError
   - Verifies error handling for API errors
   - Checks appropriate exception propagation

4. ✅ SyncGamesAsync_ReturnsGames_WhenApiCallSucceeds
   - Verifies successful game synchronization
   - Checks correct URL and HTTP method
   - Validates response handling

5. ✅ SyncGamesAsync_ThrowsException_WhenUnauthorized
   - Verifies unauthorized error handling
   - Checks appropriate exception type

### Account Controller Tests
1. ✅ Login_ShouldChallenge_WithAuth0AndRedirectUri
   - Verifies correct authentication scheme
   - Validates redirect URI handling

2. ✅ LogOut_ShouldSignOut_FromBothAuth0AndCookies
   - Verifies sign out from both schemes
   - Checks correct redirect handling

## Test Status
- Total Tests: 16
- Passing: 16
- Failed: 0
- Skipped: 0 