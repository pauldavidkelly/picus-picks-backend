# Test Plan for Show Correct Picks Count Feature

## Component Tests

### Picks Page Tests
1. `CalculateCorrectPicks_WithCompletedGames_ReturnsCorrectCount`
   - Setup game data with known outcomes
   - Verify calculation returns correct count

2. `CalculateCorrectPicks_WithNoCompletedGames_ReturnsZero`
   - Setup game data with no completed games
   - Verify count is 0

3. `CalculateCorrectPicks_WithMixedGameStates_OnlyCountsCompletedGames`
   - Setup mix of completed and incomplete games
   - Verify only completed games are counted

4. `ProgressDisplay_ShowsBothTotalAndCorrectPicks`
   - Setup game data with known total and correct picks
   - Verify both numbers display correctly

5. `Display_UpdatesWhenGameCompletes`
   - Setup game data
   - Change game state to completed
   - Verify display updates accordingly

## Manual Testing Scenarios
1. View picks page with:
   - No picks made
   - Some picks made, no games completed
   - Some picks made, some games completed
   - All picks made, all games completed

2. Verify display updates when:
   - A game completes
   - A new pick is made
