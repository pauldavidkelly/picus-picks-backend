# Implementation Details

## Overview
Added a feature to display the number of correct picks made for the current week on the Picks page.

## Implementation Steps

1. Add a computed property to calculate correct picks:
```csharp
private int CorrectPicksCount => _games?
    .Where(g => g.IsCompleted)
    .Join(_userPicks,
        game => game.Id,
        pick => pick.GameId,
        (game, pick) => IsPickCorrect(game, pick))
    .Count(isCorrect => isCorrect) ?? 0;
```

2. Update the pick status display to show both total picks and correct picks:
```razor
<div class="status-text">
    YOUR PICKS: @_pickStatus.PicksMade/@_pickStatus.TotalGames COMPLETE
    (@CorrectPicksCount CORRECT)
</div>
```

## Testing
The existing `IsPickCorrect` method is already being used for displaying pick results on individual game cards, so we can be confident in its reliability for this new use case.

## UI/UX Considerations
- Added the correct picks count in parentheses after the existing picks status
- Keeps the UI clean and simple while providing the additional information
- The count updates automatically when games complete since it's computed from the existing game data

## Changes Made

### 1. Updated Picks.razor
- Added a computed property `CorrectPicksCount` that calculates the number of correct picks by:
  - Filtering for completed games only
  - Matching user picks with the winning team
  - Handling null cases appropriately
- Modified the status text to show correct picks count: `YOUR PICKS: X/Y COMPLETE (Z CORRECT)`
- Added null checks to prevent errors when data is loading

### 2. Added Unit Tests
Added three new tests in `PicksTests.cs`:
1. `CorrectPicksCount_WithNoCompletedGames_ReturnsZero`: Verifies count is 0 when no games are completed
2. `CorrectPicksCount_WithCompletedGames_ReturnsCorrectCount`: Verifies count is correct when all games are completed
3. `CorrectPicksCount_WithMixedGameStates_OnlyCountsCompletedGames`: Verifies only completed games are counted

### Technical Details
- Uses `GameDTO.WinningTeam` to determine the winning team for each game
- Handles async loading of data properly with null checks
- Maintains existing pick visibility rules
- No changes required to the backend API

## Testing Notes
- All tests pass successfully
- Tested with various game states (completed, in progress, not started)
- Verified correct handling of null data during loading
- Confirmed proper display in the UI
