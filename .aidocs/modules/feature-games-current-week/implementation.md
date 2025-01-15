# Implementation Notes for Games Current Week Feature

## Changes Made

### 1. Created WeekCalculator Utility
- Created a new static class `WeekCalculator` in `PicusPicks.Web.Utilities`
- Implemented `GetCurrentWeek` method to calculate the current NFL week based on date
- Added private `GetRegularSeasonWeek` helper method for regular season week calculation
- Made the utility reusable for both Games and Picks pages

### 2. Updated Games.razor
- Added using statement for `PicusPicks.Web.Utilities`
- Added `CurrentDateOverride` parameter for testing
- Added `GetCurrentDate` method to handle date overrides
- Modified `OnInitializedAsync` to use `WeekCalculator.GetCurrentWeek`
- Removed hardcoded initial week value

### 3. Updated GamesTests.cs
- Added theory test for initial week selection with various dates
- Added test for week navigation after initial load
- Used `CurrentDateOverride` parameter to test different dates
- Verified correct week selection and data loading

## Design Decisions

1. **Shared Utility Class**
   - Created a reusable `WeekCalculator` to ensure consistent week calculation across pages
   - Made methods static for easy access without dependency injection

2. **Testing Support**
   - Added `CurrentDateOverride` parameter to support testing different dates
   - Kept the parameter optional to maintain normal behavior in production

3. **Week Calculation**
   - Used simple calendar-based week calculation for regular season
   - Maintained playoff week ranges from Picks page implementation

## Next Steps

1. Run all tests to verify changes
2. Perform manual testing
3. Update documentation
4. Submit for review
