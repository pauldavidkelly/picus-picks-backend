# Test Plan for Historical Picks Import

## Unit Tests

### DeadlineBypass Tests
1. Test that picks can be made when deadline bypass is enabled
2. Test that picks are rejected when deadline bypass is disabled and deadline has passed
3. Test that original deadlines are preserved in database

### Configuration Tests
1. Test that deadline bypass can be enabled via configuration
2. Test that deadline bypass can be disabled via configuration

## Integration Tests
1. Test full workflow of making picks for past games with bypass enabled
2. Test that picks are properly stored with original deadline data
3. Test that after disabling bypass, normal deadline restrictions work as expected

## Manual Testing
1. Verify ability to enter picks for all past weeks of current season
2. Verify that UI properly allows pick entry regardless of deadline when bypass enabled
3. Verify that after disabling bypass, normal deadline restrictions are enforced
4. Verify that all imported picks are visible in the picks history view
