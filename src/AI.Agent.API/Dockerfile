# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0-preview AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["src/AI.Agent.API/AI.Agent.API.csproj", "src/AI.Agent.API/"]
COPY ["src/AI.Agent.Application/AI.Agent.Application.csproj", "src/AI.Agent.Application/"]
COPY ["src/AI.Agent.Domain/AI.Agent.Domain.csproj", "src/AI.Agent.Domain/"]
COPY ["src/AI.Agent.Infrastructure/AI.Agent.Infrastructure.csproj", "src/AI.Agent.Infrastructure/"]
RUN dotnet restore "src/AI.Agent.API/AI.Agent.API.csproj"

# Copy the rest of the code
COPY . .

# Build and publish
WORKDIR "/src/src/AI.Agent.API"
RUN dotnet build "AI.Agent.API.csproj" -c Release -o /app/build
RUN dotnet publish "AI.Agent.API.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0-preview
WORKDIR /app
COPY --from=build /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

# Expose ports
EXPOSE 80
EXPOSE 443

# Start the application
ENTRYPOINT ["dotnet", "AI.Agent.API.dll"] 