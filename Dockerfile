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
