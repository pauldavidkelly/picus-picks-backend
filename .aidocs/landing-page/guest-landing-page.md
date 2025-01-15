# Guest Landing Page Implementation

## Overview
Added a new landing page for non-authenticated users that shows a welcoming image and login button, while hiding the navbar and top bar.

## Changes Made

### New Components
1. Created `LandingPage.razor`:
   - Uses `AuthorizeView` to show different content for authenticated/non-authenticated users
   - Shows the regular home page for authenticated users
   - Shows a welcoming landing page with image for non-authenticated users
   - Implements login functionality using Auth0

2. Created `LandingPage.razor.css`:
   - Styled the landing page with a clean, modern look
   - Implemented responsive design for the image and content
   - Added hover effects for the login button

### Modified Components
1. Updated `MainLayout.razor`:
   - Added conditional rendering using `AuthorizeView`
   - Only shows navbar and top bar for authenticated users
   - Uses a clean, full-width layout for non-authenticated users

2. Updated `Home.razor`:
   - Removed the `@page "/"` directive since it's now rendered by `LandingPage`

### File Structure Changes
- Moved `app.css` to the correct `wwwroot/css` directory to fix 404 error
- Using existing `home-page.png` from `wwwroot/img` directory

## Technical Notes
- Auth0 authentication is handled at the MVC controller level through `AccountController`
- Used direct link to `Account/Login` endpoint instead of Blazor navigation for proper OAuth flow
- Maintained consistent styling with the rest of the application

## Future Considerations
- Could add more content or features to the landing page
- Might want to add animations for smoother transitions
- Could consider adding a registration option if needed
