FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/AI.Agent.API/AI.Agent.API.csproj", "src/AI.Agent.API/"]
COPY ["src/AI.Agent.Application/AI.Agent.Application.csproj", "src/AI.Agent.Application/"]
COPY ["src/AI.Agent.Domain/AI.Agent.Domain.csproj", "src/AI.Agent.Domain/"]
COPY ["src/AI.Agent.Infrastructure/AI.Agent.Infrastructure.csproj", "src/AI.Agent.Infrastructure/"]
RUN dotnet restore "src/AI.Agent.API/AI.Agent.API.csproj"
COPY . .
WORKDIR "/src/src/AI.Agent.API"
RUN dotnet build "AI.Agent.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AI.Agent.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AI.Agent.API.dll"] 