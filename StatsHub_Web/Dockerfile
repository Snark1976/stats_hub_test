FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8081
ENV ASPNETCORE_URLS=http://+:8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["StatsHub_Web/StatsHub_Web.csproj", "StatsHub_Web/"]
RUN dotnet restore "StatsHub_Web/StatsHub_Web.csproj"
COPY . .
WORKDIR "/src/StatsHub_Web"
RUN dotnet build "StatsHub_Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "StatsHub_Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StatsHub_Web.dll"]
