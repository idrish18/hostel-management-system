# Smart Hostel Management System - Dockerfile
# Multi-stage build for optimized production image

# ============= BUILD STAGE =============
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Copy project file
COPY ["new hms.csproj", "."]

# Restore dependencies
RUN dotnet restore "new hms.csproj"

# Copy entire source code
COPY . .

# Build the application in Release mode
RUN dotnet build "new hms.csproj" -c Release -o /app/build

# ============= PUBLISH STAGE =============
FROM build AS publish

# Publish the application
RUN dotnet publish "new hms.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ============= RUNTIME STAGE =============
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy published files from publish stage
COPY --from=publish /app/publish .

# Expose ports
EXPOSE 5000
EXPOSE 5001

# Set environment variables
ENV ASPNETCORE_URLS=http://+:5000;https://+:5001
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=40s --retries=3 \
    CMD curl -f http://localhost:5000/health || exit 1

# Run the application
ENTRYPOINT ["dotnet", "SmartHostelManagementSystem.dll"]
