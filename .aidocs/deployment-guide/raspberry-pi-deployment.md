# Picus Picks Raspberry Pi Deployment Guide

This guide will walk you through deploying Picus Picks on a Raspberry Pi running RaspOS. We'll use Docker Compose to run the services and Neon.tech for the database. 

## Prerequisites

- Raspberry Pi running RaspOS
- Docker and Docker Compose installed
- Git installed
- Basic knowledge of terminal commands
- A Neon.tech account (free tier is fine!)
- A domain name (optional, but recommended)

## Step 1: Database Setup on Neon.tech

1. Log into [Neon.tech](https://neon.tech)
2. Create a new project:
   - Name: `picus-picks`
   - Region: Choose closest to your Pi
   - Postgres Version: Latest stable (15)

3. Save your connection string, it'll look like:
```
postgres://user:password@ep-something.region.aws.neon.tech/database
```

> 💡 Pro Tip: Neon's free tier is perfect for this setup - you get auto-scaling, backups, and no server maintenance!

## Step 2: Initial Server Setup

1. SSH into your Raspberry Pi:
```bash
ssh pi@your-raspberry-pi-ip
```

2. Create a deployment directory:
```bash
mkdir -p ~/docker/picus-picks
cd ~/docker/picus-picks
```

3. Clone your repository:
```bash
git clone https://github.com/yourusername/picus-picks-backend.git .
```

## Step 3: Environment Setup

1. Create a `.env.production` file:
```bash
touch .env.production
chmod 600 .env.production  # Restrict file permissions
```

2. Add your environment variables (adjust values accordingly):
```bash
# Database (from Neon.tech)
DB_CONNECTION=postgres://user:password@ep-something.region.aws.neon.tech/database

# API Settings
API_PORT=5000
ASPNETCORE_ENVIRONMENT=Production
ALLOWED_ORIGINS=http://localhost:8080,http://your-pi-ip:8080

# Auth0 Settings
AUTH0_DOMAIN=your-auth0-domain
AUTH0_AUDIENCE=your-auth0-audience
AUTH0_CLIENT_ID=your-auth0-client-id

# TheSportsDB Settings
THESPORTSDB_API_KEY=your-api-key

# Access Control
ALLOWED_EMAILS=user1@example.com,user2@example.com

# Frontend Settings
FRONTEND_PORT=8080
API_BASE_URL=http://your-pi-ip:5000
```

## Step 4: Docker Compose Configuration

Create a `docker-compose.yml` file:

```yaml
version: '3.8'

services:
  api:
    build:
      context: .
      dockerfile: Dockerfile
    restart: unless-stopped
    ports:
      - "${API_PORT}:80"
    environment:
      - ConnectionStrings__DefaultConnection=${DB_CONNECTION}
      - Auth0__Domain=${AUTH0_DOMAIN}
      - Auth0__Audience=${AUTH0_AUDIENCE}
      - ALLOWED_ORIGINS=${ALLOWED_ORIGINS}
      - ASPNETCORE_ENVIRONMENT=${ASPNETCORE_ENVIRONMENT}

  web:
    build:
      context: .
      dockerfile: Dockerfile.web
    restart: unless-stopped
    ports:
      - "${FRONTEND_PORT}:80"
    environment:
      - ApiBaseUrl=${API_BASE_URL}
      - Auth0__Domain=${AUTH0_DOMAIN}
      - Auth0__ClientId=${AUTH0_CLIENT_ID}
    depends_on:
      - api
```

## Step 5: Update Dockerfiles for ARM Architecture

1. Update the API Dockerfile to use ARM-compatible images:
```dockerfile
# Use ARM-compatible SDK image
FROM mcr.microsoft.com/dotnet/sdk:8.0-jammy-arm64v8 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["src/Picus.Api/Picus.Api.csproj", "Picus.Api/"]
RUN dotnet restore "Picus.Api/Picus.Api.csproj"

# Copy everything else and build
COPY ["src/Picus.Api/", "Picus.Api/"]
RUN dotnet publish "Picus.Api/Picus.Api.csproj" -c Release -o /app/publish

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy-arm64v8
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "Picus.Api.dll"]
```

2. Update the Web Dockerfile to use ARM-compatible images:
```dockerfile
# Use ARM-compatible SDK image
FROM mcr.microsoft.com/dotnet/sdk:8.0-jammy-arm64v8 AS build
WORKDIR /src

# Copy csproj files and restore
COPY ["src/PicusPicks.Web/PicusPicks.Web.csproj", "PicusPicks.Web/"]
COPY ["src/Picus.Api/Picus.Api.csproj", "Picus.Api/"]
RUN dotnet restore "PicusPicks.Web/PicusPicks.Web.csproj"

# Copy everything else and build
COPY ["src/PicusPicks.Web/", "PicusPicks.Web/"]
COPY ["src/Picus.Api/", "Picus.Api/"]
RUN dotnet publish "PicusPicks.Web/PicusPicks.Web.csproj" -c Release -o /app/publish

# Use ARM-compatible nginx
FROM arm64v8/nginx:alpine
WORKDIR /usr/share/nginx/html
COPY --from=build /app/publish/wwwroot .
COPY nginx.conf /etc/nginx/nginx.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
```

## Step 6: Deployment

1. Build and start the containers:
```bash
docker-compose up -d --build
```

2. Check if containers are running:
```bash
docker-compose ps
```

3. View logs if needed:
```bash
docker-compose logs -f
```

## Step 7: Testing

1. Test the API:
```bash
curl http://localhost:5000/health
```

2. Test the frontend by visiting:
```
http://your-pi-ip:8080
```

## Troubleshooting

1. If containers won't start:
```bash
docker-compose logs [service_name]
```

2. If database connection fails:
   - Check your Neon.tech dashboard for connection status
   - Verify your connection string in .env.production
   - Check if your IP is allowed in Neon's connection settings
   - Try connecting with psql to test:
     ```bash
     psql "your-neon-connection-string"
     ```

3. If the web interface is unreachable:
```bash
# Check nginx logs
docker-compose logs web
```

4. To restart everything:
```bash
docker-compose down
docker-compose up -d
```

## Maintenance

1. To update the application:
```bash
# Pull latest code
git pull

# Rebuild and restart containers
docker-compose down
docker-compose up -d --build
```

2. Database backups:
   - Handled automatically by Neon.tech! 🎉
   - You can also create manual backups in Neon's dashboard

3. To view resource usage:
```bash
docker stats
```

> 💡 Pro Tips:
> - Monitor your Neon.tech usage in their dashboard
> - The Raspberry Pi has limited resources, so monitor CPU and memory usage
> - Use `docker system prune` periodically to clean up unused Docker resources
> - Consider setting up a reverse proxy (like Traefik) if hosting multiple services

## Security Notes

1. Firewall rules (if using ufw):
```bash
sudo ufw allow 8080/tcp  # Frontend
sudo ufw allow 5000/tcp  # API
```

2. SSL/TLS:
   - Consider using Cloudflare's free SSL
   - Or set up Let's Encrypt with certbot
   - Update nginx.conf accordingly

Remember to:
- Keep your RaspOS and Docker images updated
- Monitor your Neon.tech dashboard
- Keep your .env.production file secure

Need help? Check the logs first, they're your best friend! And remember, Neon.tech has your back for all things database! 😊
