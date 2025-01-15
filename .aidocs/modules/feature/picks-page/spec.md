# Picks Page Design Specification

## Layout Structure

### 1. Week Navigation Bar

┌──────────────────────────────────────────────────────────┐
│ < PREV WEEK 3 - SEPTEMBER 21-25, 2023 NEXT > │
└──────────────────────────────────────────────────────────┘
- Fixed position at top
- Background: NFL Navy (#013369)
- Text: White
- Week selector with arrow navigation
- Current date range displayed

### 2. Pick Status Summary
┌──────────────────────────────────────────────────────────┐
│ YOUR PICKS: 10/16 COMPLETE │
│ [███████░░░░░░░░] 62% │
│ │
│ ⚠️ 6 GAMES NEED PICKS - NEXT DEADLINE: 2HR 15MIN │
└──────────────────────────────────────────────────────────┘
- Progress bar uses NFL Red (#D50A0A)
- Warning text uses team primary color when deadline < 3 hours
- Animated countdown for next deadline

### 3. Game Cards Grid
┌─────────────────────────────────────────┐
│ THURSDAY NIGHT FOOTBALL │
│ September 21 - 8:15 PM ET │
├─────────────────────────────────────────┤
│ ┌─────────┐ ┌─────────┐ │
│ │ GIANTS │ AT │ 49ERS │ │
│ │ [LOGO] │ │ [LOGO] │ │
│ └─────────┘ └─────────┘ │
│ │
│ [NYG Button] [SF Button] │
│ │
│ Pick Deadline: 8:00 PM ET │
└─────────────────────────────────────────┘

#### Card States:

1. **Unpicked Game**
- Team buttons use 15% opacity of team's primary color
- Hover: 30% opacity
- Selected: 100% opacity with white text

2. **Picked Game**
┌─────────────────────────────────────────┐
│ YOUR PICK │
│ ┌─────────┐ ┌─────────┐ │
│ │ GIANTS │ AT │ 49ERS │ │
│ │ [LOGO] │ │ [LOGO] │ │
│ └─────────┘ └─────────┘ │
│ │
│ [UNSELECTED] [SELECTED ✓] │
│ │
│ Pick Locked In - Can Change: 1HR 45MIN │
└─────────────────────────────────────────┘
- Selected team button: 100% team primary color
- Unselected team: Greyed out (20% opacity)

3. **Locked Game (Past Deadline)**
┌─────────────────────────────────────────┐
│ GAME IN PROGRESS │
│ ┌─────────┐ ┌─────────┐ │
│ │ GIANTS │ 14-21 │ 49ERS │ │
│ │ [LOGO] │ Q3 │ [LOGO] │ │
│ └─────────┘ └─────────┘ │
│ │
│ Your Pick: 49ers │
│ League Picks Visible Below │
└─────────────────────────────────────────┘
- Live score displayed
- Quarter/time remaining
- All buttons disabled
- Score uses team primary colors

### 4. League Picks Section (For Started Games)
┌─────────────────────────────────────────┐
│ LEAGUE PICKS - SF @ NYG │
├─────────────────────────────────────────┤
│ PLAYER PICK RECORD │
│ JohnDoe 49ers 10-6 │
│ JaneSmith Giants 12-4 │
│ BobJones 49ers 8-8 │
└─────────────────────────────────────────┘
- Only visible for games past deadline
- Pick text uses team's primary color
- Sortable columns
- Hover effect on rows

## Technical Specifications

### CSS Variables
css
:root {
/ NFL Brand Colors /
--nfl-navy: #013369;
--nfl-red: #D50A0A;
--nfl-white: #FFFFFF;
/ Card Properties /
--card-radius: 8px;
--card-shadow: 0 2px 4px rgba(0,0,0,0.1);
--card-padding: 16px;
/ Typography /
--heading-font: 'NFL EndZone Sans', sans-serif;
--body-font: 'Proxima Nova', sans-serif;
}
css
/ Mobile First /
@media (min-width: 768px) {
/ Tablet styles /
.game-grid {
grid-template-columns: repeat(2, 1fr);
}
}
@media (min-width: 1024px) {
/ Desktop styles /
.game-grid {
grid-template-columns: repeat(3, 1fr);
}
}
css
.team-button {
transition: all 0.3s ease;
}
.team-button.selected {
transform: scale(1.05);
}
css
@keyframes pulse {
0% { opacity: 1; }
50% { opacity: 0.6; }
100% { opacity: 1; }
}
.urgent-deadline {
animation: pulse 2s infinite;
}


