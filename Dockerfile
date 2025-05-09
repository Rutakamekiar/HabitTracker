# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files
COPY HabitTracker.sln ./
COPY HabitTracker.Api/HabitTracker.Api.csproj ./HabitTracker.Api/
COPY HabitTracker.Application/HabitTracker.Application.csproj ./HabitTracker.Application/
COPY HabitTracker.Domain/HabitTracker.Domain.csproj ./HabitTracker.Domain/
COPY HabitTracker.Infrastructure/HabitTracker.Infrastructure.csproj ./HabitTracker.Infrastructure/

# Restore NuGet packages
RUN dotnet restore HabitTracker.sln

# Copy all files
COPY . .

# Build and publish the API project
WORKDIR /src/HabitTracker.Api
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 80
ENTRYPOINT ["dotnet", "HabitTracker.Api.dll"]
