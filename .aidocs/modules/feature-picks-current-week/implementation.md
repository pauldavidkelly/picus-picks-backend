# Implementation Details

## Overview
This implementation adds automatic week selection to the Picks page based on the current date. The solution uses a simple month/day-based approach to determine the current NFL week without complex date calculations.

## Changes Made

### 1. Picks.razor
- Removed hardcoded `_selectedWeek = 1` initialization
- Added date-based week selection logic in `OnInitializedAsync`
- Used simple switch expression to determine week:
  ```csharp
  _selectedWeek = currentDate.Month switch
  {
      1 when currentDate.Day < 19 => 19,  // Wild Card
      1 when currentDate.Day < 26 => 20,  // Divisional
      1 => 21,  // Conference Championships
      2 when currentDate.Day < 15 => 22,  // Super Bowl
      9 or 10 or 11 or 12 => (currentDate.Day / 7) + 1,  // Regular season
      _ => 1  // Default to week 1
  };
  ```

## Design Decisions
1. **Simple Date Logic**: 
   - Used month and day checks instead of complex date calculations
   - Makes code easy to understand and maintain
   - Avoids dependency on external NFL schedule data

2. **Default Behavior**:
   - Defaults to Week 1 outside of season months
   - Provides predictable behavior in edge cases

## Testing Notes
- Current date (2025-01-15) should load Week 19 (Wild Card)
- All existing week navigation functionality remains unchanged
- Pick submission and status display work as before

## Future Considerations
- Could enhance with more precise date ranges if needed
- Might want to add configuration for season dates
- Could add caching of the current week
