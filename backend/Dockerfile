# Use .NET 8 images
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "./Lab2dotnet3.csproj"
RUN dotnet build "./Lab2dotnet3.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./Lab2dotnet3.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Lab2dotnet3.dll"]
