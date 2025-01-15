# League Table Test Plan

## Unit Tests

### LeagueTableService Tests
1. `GetLeagueTableStats_ReturnsCorrectStats`
   - Given multiple users with picks
   - When GetLeagueTableStats is called
   - Then returns correct stats for each user sorted by success rate

2. `GetLeagueTableStats_HandlesNoUsers`
   - Given no users in database
   - When GetLeagueTableStats is called
   - Then returns empty list

3. `GetLeagueTableStats_HandlesUsersWithNoPicks`
   - Given users exist but have no picks
   - When GetLeagueTableStats is called
   - Then returns users with 0 picks and 0% success rate

4. `GetLeagueTableStats_CalculatesPercentageCorrectly`
   - Given user with 7 correct picks out of 10 total
   - When GetLeagueTableStats is called
   - Then returns 70.00% success rate

### LeagueTable Component Tests
1. `LeagueTable_DisplaysUserStats`
   - Given list of user stats
   - When component renders
   - Then displays all stats correctly formatted

2. `LeagueTable_SortsUsersBySuccessRate`
   - Given unsorted list of user stats
   - When component renders
   - Then displays users in descending order by success rate

3. `LeagueTable_HandlesEmptyData`
   - Given empty stats list
   - When component renders
   - Then displays appropriate "no data" message

## Integration Tests

1. `LeagueTable_LoadsOnHomePage`
   - Given authenticated user
   - When navigating to home page
   - Then league table component loads and displays data
