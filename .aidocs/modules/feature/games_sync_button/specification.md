## Games Sync Button Feature

### User Story
As a user of the Picus Picks platform,
I want to be able to sync NFL games data
So that I can ensure I have the latest game information for making picks

### Requirements
1. Add a "Sync Games" button to the Games page
2. Button should trigger the UpsertGames API endpoint
3. Button should show loading state while sync is in progress
4. Display success/error messages after sync attempt
5. Button should be accessible only to authorized users

### Acceptance Criteria
1. Given I am on the Games page
   When I click the "Sync Games" button
   Then the system should call the UpsertGames API endpoint

2. Given I have clicked the "Sync Games" button
   When the sync is in progress
   Then the button should show a loading state

3. Given the sync operation has completed successfully
   When the API returns a success response
   Then the system should display a success message

4. Given the sync operation has failed
   When the API returns an error
   Then the system should display an appropriate error message

5. Given I am not authorized
   When I try to access the Games page
   Then I should be redirected to login 