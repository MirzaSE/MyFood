# Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["MyFood.Api/MyFood.Api.csproj", "MyFood.Api/"]
COPY ["MyFood.Application/MyFood.Application.csproj", "MyFood.Application/"]
COPY ["MyFood.Domain/MyFood.Domain.csproj", "MyFood.Domain/"]
COPY ["MyFood.Infrastructure/MyFood.Infrastructure.csproj", "MyFood.Infrastructure/"]

RUN dotnet restore "MyFood.Api/MyFood.Api.csproj"

COPY . .
WORKDIR "/src/MyFood.Api"
RUN dotnet build "MyFood.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "MyFood.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "MyFood.Api.dll"]