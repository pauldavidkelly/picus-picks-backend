# Docker Configuration

This directory contains Docker configurations for both ARM64 (Raspberry Pi) and AMD64 (standard x86_64) architectures.

## Directory Structure

```
docker/
├── api/
│   ├── Dockerfile.arm64    # API Dockerfile for ARM64 (Raspberry Pi)
│   └── Dockerfile.amd64    # API Dockerfile for AMD64 (x86_64)
├── web/
│   ├── Dockerfile.arm64    # Web Dockerfile for ARM64 (Raspberry Pi)
│   └── Dockerfile.amd64    # Web Dockerfile for AMD64 (x86_64)
├── nginx/
│   └── nginx.conf         # Nginx configuration for the web frontend
├── docker-compose.arm64.yml  # Docker Compose for ARM64
└── docker-compose.amd64.yml  # Docker Compose for AMD64
```

## Usage

### For Raspberry Pi (ARM64)

```bash
# Build and run the ARM64 version
docker compose -f docker/docker-compose.arm64.yml up -d
```

### For x86_64 Servers (AMD64)

```bash
# Build and run the AMD64 version
docker compose -f docker/docker-compose.amd64.yml up -d
```

## Key Differences

1. **API Images**:
   - ARM64: Uses `mcr.microsoft.com/dotnet/sdk:8.0-jammy-arm64v8` and `mcr.microsoft.com/dotnet/aspnet:8.0-jammy-arm64v8`
   - AMD64: Uses standard `mcr.microsoft.com/dotnet/sdk:8.0` and `mcr.microsoft.com/dotnet/aspnet:8.0`

2. **Web Images**:
   - ARM64: Uses `arm64v8/nginx:alpine` for the web server
   - AMD64: Uses standard `nginx:alpine`

## Environment Variables

Both configurations use the same environment variables:

```bash
DB_CONNECTION=        # Neon.tech database connection string
API_PORT=            # Port for the API (default: 5000)
FRONTEND_PORT=       # Port for the frontend (default: 8080)
AUTH0_DOMAIN=        # Auth0 domain
AUTH0_AUDIENCE=      # Auth0 audience
AUTH0_CLIENT_ID=     # Auth0 client ID
```

## Notes

- Both configurations use Neon.tech for the database, so no local database is needed
- The nginx configuration is shared between both architectures
- Both setups support hot-reloading for development
- SSL/TLS should be configured separately (e.g., using Cloudflare or Let's Encrypt)
