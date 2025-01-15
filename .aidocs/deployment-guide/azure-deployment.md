# Picus Picks Azure Deployment Guide

This guide will walk you through deploying the Picus Picks application to Azure, including setting up the database, web services, and configuring all necessary components.

## Prerequisites

- Azure Account with active subscription
- Azure CLI installed
- PostgreSQL backup file
- Git repository access
- Visual Studio 2022 or VS Code

## Step 1: Database Setup

### Create Azure Database for PostgreSQL Flexible Server

1. Log into Azure Portal
2. Create new PostgreSQL Flexible Server:
   ```bash
   az postgres flexible-server create \
     --name picus-picks-db \
     --resource-group picus-picks-rg \
     --location eastus \
     --admin-user your_admin_username \
     --admin-password your_secure_password \
     --sku-name Standard_B1ms \
     --tier Burstable
   ```

3. Configure Firewall Rules:
   ```bash
   az postgres flexible-server firewall-rule create \
     --name AllowAzureServices \
     --resource-group picus-picks-rg \
     --server-name picus-picks-db \
     --start-ip-address 0.0.0.0 \
     --end-ip-address 255.255.255.255
   ```

### Restore Database

1. Upload your backup file to Azure Storage:
   ```bash
   az storage blob upload \
     --account-name your_storage_account \
     --container-name backups \
     --name backup.sql \
     --file path/to/your/backup.sql
   ```

2. Restore database using psql or pgAdmin

## Step 2: Azure App Service Setup

### Create App Services

1. Create App Service for API:
   ```bash
   az appservice plan create \
     --name picus-picks-plan \
     --resource-group picus-picks-rg \
     --sku B1 \
     --is-linux

   az webapp create \
     --name picus-picks-api \
     --resource-group picus-picks-rg \
     --plan picus-picks-plan \
     --runtime "DOTNET|8.0"
   ```

2. Create App Service for Web:
   ```bash
   az webapp create \
     --name picus-picks-web \
     --resource-group picus-picks-rg \
     --plan picus-picks-plan \
     --runtime "DOTNET|8.0"
   ```

## Step 3: Configuration

### Configure Environment Variables

1. API App Settings:
   ```bash
   az webapp config appsettings set \
     --name picus-picks-api \
     --resource-group picus-picks-rg \
     --settings \
     ConnectionStrings__DefaultConnection="Host=picus-picks-db.postgres.database.azure.com;Database=picuspicks;Username=your_admin_username;Password=your_secure_password" \
     Auth0__Domain="your-auth0-domain" \
     Auth0__Audience="your-auth0-audience" \
     SportsDbApi__Url="your-api-url" \
     SportsDbApi__ApiKey="your-api-key"
   ```

2. Web App Settings:
   ```bash
   az webapp config appsettings set \
     --name picus-picks-web \
     --resource-group picus-picks-rg \
     --settings \
     ApiBaseUrl="https://picus-picks-api.azurewebsites.net"
   ```

## Step 4: Continuous Deployment Setup

1. Create `.github/workflows/deploy.yml`:
```yaml
name: Deploy to Azure

on:
  push:
    branches: [ main ]

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v2
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v1
      with:
        dotnet-version: '8.0.x'
    
    - name: Build API
      run: |
        dotnet restore src/Picus.Api
        dotnet build src/Picus.Api --configuration Release
        dotnet publish src/Picus.Api -c Release -o api-publish
    
    - name: Build Web
      run: |
        dotnet restore src/PicusPicks.Web
        dotnet build src/PicusPicks.Web --configuration Release
        dotnet publish src/PicusPicks.Web -c Release -o web-publish
    
    - name: Deploy API
      uses: azure/webapps-deploy@v2
      with:
        app-name: 'picus-picks-api'
        publish-profile: ${{ secrets.AZURE_API_PUBLISH_PROFILE }}
        package: './api-publish'
    
    - name: Deploy Web
      uses: azure/webapps-deploy@v2
      with:
        app-name: 'picus-picks-web'
        publish-profile: ${{ secrets.AZURE_WEB_PUBLISH_PROFILE }}
        package: './web-publish'
```

2. Configure GitHub Secrets:
   - Get publish profiles from Azure Portal for both apps
   - Add them as secrets in GitHub:
     - `AZURE_API_PUBLISH_PROFILE`
     - `AZURE_WEB_PUBLISH_PROFILE`

## Step 5: Initial Deployment

1. Commit and push your changes:
   ```bash
   git add .
   git commit -m "Setup deployment configuration"
   git push origin main
   ```

2. Monitor deployment in GitHub Actions tab

## Step 6: Verify Deployment

1. Check API health endpoint:
   ```
   https://picus-picks-api.azurewebsites.net/health
   ```

2. Access web application:
   ```
   https://picus-picks-web.azurewebsites.net
   ```

## Troubleshooting

### Common Issues

1. Database Connection Issues:
   - Verify firewall rules
   - Check connection string
   - Ensure database server is running

2. Auth0 Issues:
   - Verify Auth0 configuration
   - Check allowed callback URLs
   - Verify client credentials

3. Deployment Failures:
   - Check GitHub Actions logs
   - Verify publish profiles are correct
   - Check app service logs in Azure Portal

### Useful Commands

Monitor API logs:
```bash
az webapp log tail --name picus-picks-api --resource-group picus-picks-rg
```

Restart web app:
```bash
az webapp restart --name picus-picks-web --resource-group picus-picks-rg
```

## Cost Management

- B1 App Service Plan: ~$13/month
- PostgreSQL Flexible Server (B1ms): ~$15-20/month
- Estimated total: $30-35/month

You can reduce costs by:
1. Using Free tier for development/testing
2. Scaling down during non-peak hours
3. Setting up spending alerts

## Security Considerations

1. Enable Azure AD authentication
2. Set up SSL certificates
3. Configure CORS properly
4. Use managed identities where possible
5. Regular security patches and updates

## Monitoring Setup

1. Enable Application Insights
2. Set up alert rules
3. Configure performance metrics
4. Setup logging

Need help with any of these steps? Just ask! 😊
