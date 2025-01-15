# Games Current Week Feature

## User Story
As a user, I want the Games page to automatically open to the current NFL week based on the date, so I don't have to manually navigate to the current week each time I visit.

## Requirements
1. When the Games page loads, it should automatically select the current NFL week based on the current date
2. The week selection should follow the same logic as the Picks page:
   - Regular Season (September-December):
     - Days 1-7: Week 1
     - Days 8-14: Week 2
     - Days 15-21: Week 3
     - Days 22-28: Week 4
     - Days 29+: Week 5
   - Playoffs (January-February):
     - January 1-18: Wild Card (Week 19)
     - January 19-25: Divisional (Week 20)
     - January 26+: Conference Championships (Week 21)
     - February 1-14: Super Bowl (Week 22)
   - Off-season: Default to Week 1

## Acceptance Criteria
1. The Games page opens to the correct week based on the current date
2. The week calculation logic matches the Picks page exactly
3. All existing Games page functionality remains unchanged
4. Unit tests verify the correct week is selected for different dates
5. The week can still be manually changed after initial load
