# Show Correct Picks Count Feature

## User Story
As a user viewing the Picks page
I want to see how many of my picks for the current week were correct
So that I can track my performance at a glance

## Requirements
1. Display the count of correct picks for the current week
2. Only include completed games in the count
3. Show this information alongside the existing progress bar
4. Update the count automatically when games complete

## Acceptance Criteria
1. Given I am on the Picks page
   When there are completed games for the current week
   Then I should see the number of correct picks I made for those games

2. Given I am on the Picks page
   When a game completes and my pick was correct
   Then the correct picks count should increment

3. Given I am on the Picks page
   When there are no completed games for the current week
   Then the correct picks count should show 0

4. Given I am on the Picks page
   When viewing the picks progress
   Then I should see both total picks made and correct picks in an easy to read format
