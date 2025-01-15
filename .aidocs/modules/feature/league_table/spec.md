# League Table Feature Specification

## User Story
As a user of Picus Picks
I want to see a league table showing all users' performance
So that I can compare my prediction success rate with others

## Requirements
1. Create a standalone league table component
2. Display the component on the home page after user login
3. Show the following information for each user:
   - Username/Display name
   - Total number of correct picks
   - Total number of picks made
   - Success rate as a percentage
4. Sort users by success rate (highest to lowest)
5. Update the table automatically when new picks are made/scored

## Acceptance Criteria
1. The league table is visible on the home page after login
2. Each user's row shows:
   - Display name
   - Correct picks count
   - Total picks count
   - Success rate percentage (formatted to 2 decimal places)
3. Table is sorted by success rate in descending order
4. Table updates when:
   - A game is completed and picks are scored
5. Table handles edge cases:
   - Users with no picks (show 0%)
   - Null/missing data
   - Large numbers of users (pagination)
