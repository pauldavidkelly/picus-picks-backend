# Default to Current Week on Picks Page

## User Story
As a user of the Picks page,
I want the page to automatically open on the current NFL week when I visit it,
So that I don't have to manually navigate to the current week every time.

## Requirements
1. When opening the Picks page, determine the current NFL week based on the date
2. Initialize the page with the correct week selected
3. Support both regular season and playoff weeks:
   - Regular Season: Weeks 1-18
   - Wild Card: Week 19
   - Divisional: Week 20
   - Conference Championships: Week 21
   - Super Bowl: Week 22
4. Default to Week 1 if outside the NFL season

## Acceptance Criteria
1. Given it's January 15, 2025
   When I open the Picks page
   Then it should default to Week 19 (Wild Card)

2. Given it's January 22, 2025
   When I open the Picks page
   Then it should default to Week 20 (Divisional)

3. Given it's January 29, 2025
   When I open the Picks page
   Then it should default to Week 21 (Conference Championships)

4. Given it's February 12, 2025
   When I open the Picks page
   Then it should default to Week 22 (Super Bowl)

5. Given it's September 15, 2024
   When I open the Picks page
   Then it should default to Week 2 (Regular Season)

6. Given it's July 1, 2024 (off-season)
   When I open the Picks page
   Then it should default to Week 1
