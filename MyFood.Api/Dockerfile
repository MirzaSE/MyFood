FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MyFood.Api/MyFood.Api.csproj", "MyFood.Api/"]
COPY ["MyFood.Application/MyFood.Application.csproj", "MyFood.Application/"]
COPY ["MyFood.Domain/MyFood.Domain.csproj", "MyFood.Domain/"]
COPY ["MyFood.Infrastructure/MyFood.Infrastructure.csproj", "MyFood.Infrastructure/"]
RUN dotnet restore "./MyFood.Api/MyFood.Api.csproj"
COPY . .
WORKDIR "/src/MyFood.Api"
RUN dotnet build "./MyFood.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MyFood.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

## Run migrations
#RUN dotnet tool install --global dotnet-ef 
#ENV PATH="$PATH:/root/.dotnet/tools"
#RUN dotnet-ef database update --project src/MyFood.Infrastructure/ --startup-project src/MyFood.Api/

ENTRYPOINT ["dotnet", "MyFood.Api.dll"]#, "--environment=Development"]