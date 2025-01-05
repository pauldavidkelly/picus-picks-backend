# Migration Fixes Documentation

## Issue: Migration and Model Snapshot Sync Problems
Date: 2025-01-05

### Problem Description
The model snapshot became out of sync with the migrations, causing EF Core to attempt to recreate the entire database schema when adding new migrations.

### Solution Steps
1. Removed corrupted model snapshot file
2. Let EF Core regenerate the snapshot correctly
3. Verified database and models were in sync
4. Successfully tested with a new migration

### Standard Process for Reverting Migrations
When you need to revert a migration, follow these steps in order:

1. Update the database to the last known good migration:
```bash
dotnet ef database update [LastGoodMigrationName]
```

2. Remove the unwanted migration files:
```bash
dotnet ef migrations remove
```

Note: You might need to run the remove command multiple times if you have multiple migrations to remove.

### Important Notes
- Always revert the database first before removing migration files
- Migration files are needed to properly revert database changes
- After reverting, make sure your model code matches the database state
- Keep your migrations small and focused for easier reverting if needed

### Why This Order Matters
The migration files contain both the "up" and "down" scripts needed to apply and revert changes. If you remove the migration files first, EF Core won't know how to revert the database changes properly.

### Testing After Fixes
To verify everything is working:
1. Create a test migration
2. Verify it applies correctly
3. Revert using the process above
4. Verify the database is in the correct state
