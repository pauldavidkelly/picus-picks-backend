# Test Plan for Default Week Selection

## Unit Tests
1. Test_InitialWeek_WildCard
   - Set date to January 15, 2025
   - Verify selected week is 19

2. Test_InitialWeek_Divisional
   - Set date to January 22, 2025
   - Verify selected week is 20

3. Test_InitialWeek_Conference
   - Set date to January 29, 2025
   - Verify selected week is 21

4. Test_InitialWeek_SuperBowl
   - Set date to February 12, 2025
   - Verify selected week is 22

5. Test_InitialWeek_RegularSeason
   - Set date to September 15, 2024
   - Verify selected week is 2

6. Test_InitialWeek_OffSeason
   - Set date to July 1, 2024
   - Verify selected week is 1

## Integration Tests
1. Test_PicksPage_LoadsCorrectGames
   - Set date to January 15, 2025
   - Navigate to Picks page
   - Verify games loaded are Wild Card games
   - Verify week display shows "WEEK 19"

2. Test_PicksPage_WeekNavigation
   - Set date to January 15, 2025
   - Navigate to Picks page
   - Verify "PREV" button is enabled
   - Verify "NEXT" button is enabled
   - Click "NEXT" button
   - Verify week changes to 20 (Divisional)

## Notes
- These tests will need to mock the current date to ensure consistent behavior
- Integration tests should verify the entire page loads correctly with the default week
- All existing week navigation functionality should continue to work as before
