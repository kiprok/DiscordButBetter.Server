FROM mcr.microsoft.com/dotnet/sdk:9.0 AS backend
WORKDIR /src
COPY ["DiscordButBetter.Server/DiscordButBetter.Server.csproj", "./"]
RUN dotnet restore "DiscordButBetter.Server.csproj"
COPY ["DiscordButBetter.Server/", "."]
RUN dotnet publish "DiscordButBetter.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM node:20 AS frontend
WORKDIR /src
COPY ["DiscordButBetter.Client/package.json", "./"]
COPY ["DiscordButBetter.Client/package-lock.json", "./"]
RUN npm install
COPY ["DiscordButBetter.Client/", "./"]
RUN npm run build

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=backend /app/publish .
COPY --from=frontend /src/dist ./wwwroot
ENTRYPOINT ["dotnet", "DiscordButBetter.Server.dll"]
