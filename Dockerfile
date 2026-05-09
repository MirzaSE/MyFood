# Use the .NET SDK image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory inside the container
WORKDIR ./MyFood.Api

# Copy the .csproj file and restore dependencies
COPY . ./
RUN dotnet restore

# Copy the entire API project to the container
COPY . .

RUN ls
# Build the API
RUN dotnet publish -c Release -o out

# Create a runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR ./MyFood.Api
COPY --from=build /MyFood.Api/out .

# Specify the startup command
ENTRYPOINT ["dotnet", "MyFood.Api.dll"]
