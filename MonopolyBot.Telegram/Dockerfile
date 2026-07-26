USER $APP_UID
WORKDIR /app


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MonopolyBot.Telegram/MonopolyBot.Telegram.csproj", "MonopolyBot.Telegram/"]
COPY ["MonopolyBot.Application/MonopolyBot.Application.csproj", "MonopolyBot.Application/"]
COPY ["MonopolyBot.Core/MonopolyBot.Core.csproj", "MonopolyBot.Core/"]
COPY ["MonopolyBot.DataAccess.Postgres/MonopolyBot.DataAccess.Postgres.csproj", "MonopolyBot.DataAccess.Postgres/"]
COPY ["MonopolyBot.DataAccess.ApiClients/MonopolyBot.DataAccess.ApiClients.csproj", "MonopolyBot.DataAccess.ApiClients/"]
RUN dotnet restore "./MonopolyBot.Telegram/MonopolyBot.Telegram.csproj"
COPY . .
WORKDIR "/src/MonopolyBot.Telegram"
RUN dotnet build "./MonopolyBot.Telegram.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MonopolyBot.Telegram.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MonopolyBot.Telegram.dll"]