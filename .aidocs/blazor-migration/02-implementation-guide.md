# Picus Picks Blazor Migration - Implementation Guide

## Tech Stack
- .NET 8 Blazor Web App
- MudBlazor for UI components
- Auth0 for authentication
- xUnit + bUnit for testing

## Implementation Order
1. Project Setup
   ```bash
   dotnet new blazor -n PicusPicks.Web
   dotnet add package MudBlazor
   dotnet add package Auth0.AspNetCore.Authentication
   ```

2. Core Infrastructure
   ```csharp
   // Add to Program.cs
   builder.Services.AddMudServices();
   builder.Services.AddAuth0WebAppAuthentication();
   ```

3. Component Priority
   1. MainLayout + Auth
   2. Games display
   3. Pick submission
   4. Admin features

## API Integration
```csharp
public class ApiAuthorizationMessageHandler : DelegatingHandler
{
    private readonly IAuth0ClientProxy _auth0Client;

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken ct)
    {
        var token = await _auth0Client.GetTokenSilentlyAsync();
        request.Headers.Authorization = 
            new AuthenticationHeaderValue("Bearer", token);
        return await base.SendAsync(request, ct);
    }
}
```

## Testing Strategy
1. Unit Tests (xUnit)
   - Services
   - Helper functions
   - Complex logic

2. Component Tests (bUnit)
   - UI components
   - User interactions
   - Authentication states

3. Integration Tests
   - API communication
   - Auth flows
   - End-to-end features