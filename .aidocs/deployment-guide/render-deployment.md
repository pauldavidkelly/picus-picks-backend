# Picus Picks Render Deployment Guide

This guide will walk you through deploying Picus Picks to Render.com - a modern cloud platform with a generous free tier that's perfect for small to medium-sized applications.

## Prerequisites

- A [Render account](https://render.com) (free)
- Your code pushed to a Git repository (GitHub/GitLab)
- Git installed locally

## Step 1: Database Setup with Neon.tech

1. Create a [Neon account](https://neon.tech) (free tier available)
2. Click "Create Project"
3. Configure your project:
   - Name: `picus-picks`
   - Region: Choose closest to your users
   - Postgres Version: Latest stable (e.g., 15)
   - Click "Create Project"

Save the database credentials shown - you'll need these later!

> 💡 Pro Tip: Neon offers some cool features like:
> - Autoscaling compute and storage
> - Branching for development and testing
> - Automatic backups
> - Serverless driver for better performance

### Database Connection String

Your connection string will look like this:
```
postgres://[user]:[password]@[hostname]/[database]
```

You can find this in the Neon dashboard under "Connection Details". Make sure to:
1. Use the "Pooled" connection string for better performance
2. Keep the connection string secure - we'll add it to Render's environment variables later

### Database Restoration

You can restore your database to Neon in two ways:

#### Option 1: Using psql (Recommended for smaller databases)

1. Get your database connection info from Neon dashboard
2. If your backup is a SQL file:
```bash
psql "postgres://user:password@hostname/database" < your_backup.sql
```

3. If your backup is a custom format (`.backup`):
```bash
# For backups with role/owner issues, use these flags:
pg_restore \
  --no-owner \                    # Skip commands to set ownership of objects
  --no-privileges \               # Skip restoration of access privileges (grant/revoke)
  --no-comments \                 # Don't output commands to restore comments
  --clean \                       # Clean (drop) database objects before recreating
  --if-exists \                   # Use IF EXISTS when dropping objects
  -d "postgres://user:password@hostname/database" \
  your_backup.backup
```

> 💡 Pro Tips for Database Restoration:
> - Neon creates the `public` schema automatically, so you might see (and can ignore) the "schema public already exists" error
> - The default Neon user is the owner of all objects, so we skip owner/privilege restoration
> - If you still get errors, you can create a clean backup from your source database:
>   ```bash
>   pg_dump \
>     --no-owner \
>     --no-privileges \
>     --no-comments \
>     -Fc \
>     your_source_database > clean_backup.backup
>   ```

#### Option 2: Using pg_dump over HTTPS (For larger databases)

1. Create a publicly accessible URL for your backup file
2. Use Neon's import feature:
   - Go to your project
   - Click "Import Data"
   - Paste your backup URL
   - Choose your target database
   - Click "Import"

#### Troubleshooting Restoration

If you get errors during restoration:
1. Check if your backup format matches the restore method
2. Verify your connection string is correct
3. Ensure you have enough storage quota in your plan
4. For large restores, consider using Neon's HTTPS import feature

## Step 2: Web Service Setup

### Creating the Dockerfile

Before setting up the web service on Render, you'll need a Dockerfile in your repository root. Create a file named `Dockerfile` (no extension) with the following content:

```dockerfile
# Use the official .NET SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["src/Picus.Api/Picus.Api.csproj", "Picus.Api/"]
RUN dotnet restore "Picus.Api/Picus.Api.csproj"

# Copy the rest of the source code
COPY ["src/Picus.Api/", "Picus.Api/"]

# Build the application
RUN dotnet publish "Picus.Api/Picus.Api.csproj" -c Release -o /app/publish

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "Picus.Api.dll"]
```

> 💡 Pro Tips for the Dockerfile:
> - We use a multi-stage build to keep the final image small
> - The SDK image is only used for building, while the smaller ASP.NET runtime image is used for deployment
> - We expose port 8080 as required by Render
> - Environment variables can be overridden in Render's dashboard

### Setting up the Web Service

1. Click "New +" and select "Web Service"
2. Connect your Git repository
3. Configure the service:
   - Name: `picus-picks-api`
   - Environment: `Docker`
   - Branch: `main` (or your deployment branch)
   - Region: Same as your database
   - Instance Type: Free (512MB)
   - Build Command: (leave empty, Docker will handle this)
   - Start Command: (leave empty, Docker will handle this)

### Configuration Setup

#### Step 1: Clean Up appsettings.json

1. In your API's `appsettings.json`, keep only non-sensitive defaults:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "FeatureFlags": {
    "BypassPickDeadlines": false
  }
}
```

2. In your Frontend's `appsettings.json`, keep only non-sensitive defaults:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "FeatureFlags": {
    "BypassPickDeadlines": false
  }
}
```

#### Step 2: Set Up Render Environment Variables

##### For API Service:
```
# Database (Get this from Neon dashboard)
ConnectionStrings__DefaultConnection=postgres://user:password@hostname/database

# Auth0 Settings (Get these from Auth0 dashboard)
Auth0__Domain=your-auth0-domain
Auth0__Audience=your-auth0-audience

# Sports DB API (Your API credentials)
SportsDbApi__Url=your-api-url
SportsDbApi__ApiKey=your-api-key

# Application Settings
AllowedEmails__Entries=["user1@example.com","user2@example.com"]

# CORS (Your frontend URL in Render)
ALLOWED_ORIGINS=https://picus-picks-web.onrender.com
```

##### For Frontend Service:
```
# API Configuration (Your API service URL in Render)
ApiBaseUrl=https://your-api-service.onrender.com

# Auth0 Configuration (Get these from Auth0 dashboard)
Auth0__Domain=your-auth0-domain
Auth0__ClientId=your-auth0-client-id
Auth0__ClientSecret=your-auth0-client-secret
Auth0__Audience=https://api.picuspicks.com
Auth0__CallbackPath=/callback
```

> 💡 Pro Tips:
> - NEVER put sensitive values in appsettings.json
> - Use Render's environment variables for anything that:
>   - Is sensitive (API keys, secrets)
>   - Changes between environments (URLs, feature flags)
>   - Needs to be changed without redeployment
> - Use double underscores (`__`) in Render for nested settings
> - Your code doesn't need to change - ASP.NET Core will automatically use environment variables over appsettings.json values

## Step 3: Frontend Web Service Setup

1. Click "New +" and select "Static Site"
2. Connect your Git repository
3. Configure the service:
   - Name: `picus-picks-web`
   - Branch: `main` (or your deployment branch)
   - Build Command: `dotnet publish -c Release -o publish`
   - Publish Directory: `publish/wwwroot`
   - Environment: `.NET`
   - Instance Type: Free

### Update API Configuration

Make sure to add your frontend URL to the CORS configuration in your API's environment variables:
```
ALLOWED_ORIGINS=https://picus-picks-web.onrender.com
```

### Blazor Configuration Tips

1. Update your `wwwroot/index.html` base href if needed:
```html
<base href="/" />
```

2. Ensure your `Program.cs` has the correct service URLs:
```csharp
builder.Services.AddScoped(sp => 
    new HttpClient { 
        BaseAddress = new Uri(builder.Configuration["API_URL"] ?? 
            throw new InvalidOperationException("API_URL not configured"))
    });
```

3. For Auth0 configuration, update your `appsettings.json`:
```json
{
  "Auth0": {
    "Authority": "https://{your-auth0-domain}",
    "ClientId": "{your-auth0-client-id}",
    "Audience": "{your-auth0-audience}"
  }
}
```

4. Add a `staticwebapp.config.json` in your `wwwroot` folder:
```json
{
  "navigationFallback": {
    "rewrite": "/index.html",
    "exclude": ["/images/*.{png,jpg,gif}", "/css/*", "/js/*"]
  },
  "mimeTypes": {
    ".json": "text/json",
    ".wasm": "application/wasm"
  }
}
```

This ensures proper routing for your Blazor WASM application.

## Step 4: Custom Domain Setup with Hover.com

### Hover.com DNS Setup

1. Log into your [Hover account](https://hover.com/signin)
2. Click on your domain name
3. Go to the "DNS" tab
4. Remove any existing A or CNAME records that might conflict
5. Click "Add Record" for each required record

> 💡 Pro Tip: Hover's DNS changes usually propagate within 15-30 minutes, much faster than the typical 24-48 hours!

### API Custom Domain

1. In your Render dashboard, go to your API service (`picus-picks-api`)
2. Click on "Settings" and scroll to the "Custom Domains" section
3. Click "Add Custom Domain"
4. Enter your domain (e.g., `api.picuspicks.com`)
5. In Hover's DNS settings, add:
   ```
   Type: CNAME
   Hostname: api
   Target: [your-render-service].onrender.com
   TTL: Automatic (or 3600 if asked)
   ```
   
> 💡 Pro Tip: In Hover, the "Hostname" field is what other providers call "Name" or "Subdomain"

### Frontend Custom Domain

1. In your Render dashboard, go to your frontend service (`picus-picks-web`)
2. Click on "Settings" and scroll to the "Custom Domains" section
3. Click "Add Custom Domain"
4. Enter your domain (e.g., `picuspicks.com` or `www.picuspicks.com`)
5. In Hover's DNS settings, add:
   ```
   # For apex domain (picuspicks.com):
   Type: A
   Hostname: @ (leave empty for apex domain)
   Target: [Render's IP]
   TTL: Automatic

   # For www subdomain:
   Type: CNAME
   Hostname: www
   Target: [your-render-service].onrender.com
   TTL: Automatic
   ```

### Setting up Forwards in Hover (Optional)

To redirect www to non-www (or vice versa):
1. In Hover, go to the "Forwards" tab
2. Click "Add Forward"
3. Choose the subdomain to forward (e.g., www)
4. Select "Forward Type: Standard 301"
5. Enter your target domain (e.g., https://picuspicks.com)
6. Check "Forward SSL"

> 💡 Pro Tips for Hover:
> - Hover's "Connect" feature isn't needed for Render - use DNS records instead
> - If you see "This record already exists" when adding DNS records, delete the existing record first
> - Hover automatically adds MX records for email - don't delete these unless you're sure
> - Use Hover's "Test DNS" button to verify your records

### Update Environment Variables

After setting up custom domains, update your environment variables:

1. In the API service, update the CORS settings:
```
ALLOWED_ORIGINS=https://picuspicks.com,https://www.picuspicks.com
```

2. In the Frontend service, update the API URL:
```
ApiBaseUrl=https://api.picuspicks.com
```

3. Update Auth0 settings:
   - Go to your Auth0 dashboard
   - Add your custom domains to the "Allowed Callback URLs"
   - Add your custom domains to the "Allowed Logout URLs"
   - Add your custom domains to the "Allowed Web Origins"

> 💡 Pro Tips:
> - Always use HTTPS for your custom domains
> - Set up both www and non-www versions of your domain
> - Consider setting up redirects (e.g., www to non-www)
> - Update any documentation or client applications with the new URLs

### Troubleshooting Custom Domains with Hover

If you encounter issues:
1. In Hover:
   - Check the DNS tab for any conflicting records
   - Use Hover's "Test DNS" feature
   - Verify you haven't accidentally created duplicate records
   - Make sure you're not using Hover's "Connect" or "Forward" features unless intended

2. General checks:
   - Use `dig api.yourdomain.com` to check DNS propagation
   - Try Hover's DNS propagation tool at hover.com/dns
   - Check SSL certificate status in Render dashboard
   - Clear browser cache and DNS cache
   - Test the domain with [SSL Labs](https://www.ssllabs.com/ssltest/)

3. Common Hover-specific issues:
   - If adding a CNAME record fails, ensure there's no A record for the same hostname
   - If the apex domain (naked domain) isn't working, double-check the A record has no hostname
   - If using email with your domain, verify MX records weren't accidentally removed

> 💡 Pro Tip: Hover's support is excellent! If you're stuck, their support team is available via chat or email and they're very familiar with Render setups.

## Step 5: Deployment

1. Ensure your repository has a `Dockerfile` at the root:

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["PicusPicks.Api/PicusPicks.Api.csproj", "PicusPicks.Api/"]
RUN dotnet restore "PicusPicks.Api/PicusPicks.Api.csproj"
COPY . .
WORKDIR "/src/PicusPicks.Api"
RUN dotnet build "PicusPicks.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PicusPicks.Api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PicusPicks.Api.dll"]
```

2. Push your changes to your repository
3. Render will automatically detect the changes and deploy

## Monitoring and Logs

- View logs: Dashboard → Your Service → Logs
- Monitor metrics: Dashboard → Your Service → Metrics
- Set up alerts: Dashboard → Your Service → Events

## Cost Saving Tips

1. Your free instance will spin down after 15 minutes of inactivity
2. First request after inactivity will take longer (cold start)
3. Keep your database size under 1GB to stay in free tier
4. Monitor bandwidth usage (included in free tier)

## Troubleshooting

1. Deployment Failures:
   - Check build logs in Render dashboard
   - Verify Dockerfile is correct
   - Check environment variables are set

2. Database Connection Issues:
   - Verify connection string
   - Check if database is in same region
   - Ensure IP allowlist is configured (if enabled)

3. Performance Issues:
   - Consider upgrading from free tier if needed
   - Optimize database queries
   - Implement caching where possible

## Upgrading Later

If you need more resources, Render makes it easy to upgrade:
- Individual services can be upgraded independently
- No downtime during upgrades
- Pay only for what you use
- Can scale back down if needed

Need help? Render's support is actually helpful (unlike some cloud providers who shall remain nameless... *cough* Azure *cough*) 😉
