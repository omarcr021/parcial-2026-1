FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["parcial-2026-1.csproj", "./"]
RUN dotnet restore "parcial-2026-1.csproj"
COPY . .
RUN dotnet publish "parcial-2026-1.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "parcial-2026-1.dll"]
