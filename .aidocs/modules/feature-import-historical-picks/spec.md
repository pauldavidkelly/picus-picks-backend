# Import Historical Picks Feature

## User Story
As a User
I want to import my existing picks for this season
So that I can have all my historical data in the system

## Requirements
1. The picks page deadline restriction should be temporarily disabled to allow historical data entry
2. Users should be able to enter picks for past weeks of the current season
3. The system should maintain the original pick deadlines in the database for future use
4. The system should not allow modification of picks after re-enabling the deadline restriction

## Acceptance Criteria
1. Picks page allows entry of picks regardless of game deadline during the import period
2. All historical picks for the current season can be entered manually
3. After re-enabling restrictions, picks cannot be modified after their deadline
4. Original game deadlines are preserved in the database

## Technical Approach
1. Modify the picks validation logic to bypass deadline checks temporarily
2. Ensure the bypass is configurable and can be easily disabled after data import
3. Maintain existing deadline data in the database for future use
4. Add logging for any picks made during this bypass period
