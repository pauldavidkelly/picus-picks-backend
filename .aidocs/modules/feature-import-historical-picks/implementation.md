# Implementation Details for Historical Picks Import

## Overview
To allow historical picks to be entered, we need to modify the deadline checking logic in the Picks.razor page. We'll do this by adding a configuration setting that can temporarily disable the deadline checks.

## Changes Required

### 1. Add Configuration Setting
Add a new configuration setting in appsettings.json:
```json
{
  "FeatureFlags": {
    "BypassPickDeadlines": false
  }
}
```

### 2. Modify Picks.razor
1. Inject IConfiguration to access the feature flag
2. Modify IsGameLocked method to check the feature flag
3. Add a visual indicator when deadline bypass is active

### 3. Implementation Steps
1. Add configuration injection
2. Update IsGameLocked to check configuration
3. Add visual indicator for bypass mode
4. Add logging for picks made during bypass

## Code Changes

### Picks.razor Changes
- Add IConfiguration injection
- Modify IsGameLocked to check bypass setting
- Add visual indicator when bypass is active

### Testing
- Unit tests for bypass functionality
- Integration tests for full workflow
- Manual testing of historical pick entry
