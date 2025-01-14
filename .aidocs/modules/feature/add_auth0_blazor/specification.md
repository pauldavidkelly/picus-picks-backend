# Add Auth0 Authentication to Blazor Web App

## User Story
As a user of the Picus Picks platform
I want to be able to securely authenticate using Auth0
So that I can access my personalized picks and league information

## Requirements
1. Implement Auth0 authentication in the Blazor web app
2. Configure secure user authentication flow
3. Protect routes that require authentication
4. Display user information when logged in
5. Provide login/logout functionality
6. Ensure proper token management and security
7. Handle authentication errors gracefully

## Acceptance Criteria
1. Users can log in using Auth0
   - Login button is visible when not authenticated
   - Clicking login redirects to Auth0 login page
   - After successful login, user is redirected back to the app

2. Users can log out
   - Logout button is visible when authenticated
   - Clicking logout ends the session
   - User is redirected to home page after logout

3. Protected routes are secure
   - Unauthenticated users are redirected to login
   - Authenticated users can access protected routes
   - Authentication state persists across page refreshes

4. User information is displayed
   - User's name is shown when logged in
   - User's profile picture (if available) is displayed
   - Auth0 user metadata is properly handled

5. Error handling
   - Failed login attempts show appropriate error messages
   - Token expiration is handled gracefully
   - Network errors during authentication are handled appropriately
