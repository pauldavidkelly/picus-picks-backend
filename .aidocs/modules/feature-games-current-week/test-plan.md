# Test Plan for Games Current Week Feature

## Unit Tests

### Initial Week Selection Tests
1. Test that Wild Card week (19) is selected on January 15th
2. Test that Divisional week (20) is selected on January 22nd
3. Test that Conference week (21) is selected on January 29th
4. Test that Super Bowl week (22) is selected on February 12th
5. Test that Regular Season Week 1 is selected on September 5th
6. Test that Regular Season Week 2 is selected on September 12th
7. Test that Regular Season Week 3 is selected on September 19th
8. Test that Regular Season Week 4 is selected on September 26th
9. Test that off-season dates default to Week 1

### Navigation Tests
1. Test that week can be changed after initial load
2. Test that navigation loads correct games data

## Manual Tests
1. Load Games page and verify correct week is selected based on current date
2. Verify week navigation still works:
   - Previous week button
   - Next week button
   - Week dropdown
3. Verify games data loads correctly for selected week
4. Verify page refresh maintains correct week selection
