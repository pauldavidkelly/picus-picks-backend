# Requirements for the MVP version of Picus

## User Story
As a User
I want to be able to make NFL picks with my friends
So that we can see who has the best picks

## Requirements
1. Users should be able to create an account
2. Users should be able to login
3. Users should be able to logout
4. Users should be able to view their picks
    a. Users can view past picks and upcoming picks
5. Users should be able to view the picks of their league
    a. Users can view the picks of their league after a deadline for the game
6. Users should be able to view the league standings for their league
7. Users should be able to view upcoming and past NFL games with scores for completed games
8. Import this seasons picks from Excel

## Acceptance Criteria
1. Users should be able to create an account
    a. Users should be able to use a Social Account to login
2. Users should be able to login
    a. When a user is logged in they are taken to the league view
3. Users should be able to logout
    a. When a user logs out they are taken to the login page
4. Users should be able to view their picks
    a. Users can view past picks and upcoming picks
5. Users should be able to view the picks of their league
    a. Users can view the picks of their league after a deadline for the game
6. Users should be able to view the league standings for their league
7. Users should be able to view upcoming and past NFL games with scores for completed games
8. Historical picks should be available for this season

## Notes
Still to do:
- [X] Create league standings view
- [X] Home page shows league view on login
- [X] Create login page with background image and login button
- [X] Create weekly league picks grid view
    - [ ] For completed weeks/games have a win-loss row for each player that week
- [X] Create tab on picks page to switch between user picks and league picks
- [X] Get Glenn to login so we have his user account
- [X] Import picks from Excel
    - [X] Paul's picks
    - [X] Glenn's picks
- [X] Add weeks pick total on picks page for selected week.
- [ ] Rename repo to remove backend from the name
- [ ] Picks gridneeds actual name
    - [ ] From profile or do we need to update Auth0 claims?
- [ ] Deploy to Azure