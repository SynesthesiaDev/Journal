FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
ENV DOTNET_GCLatencyMode=0
ENV DOTNET_GCHeapHardLimitPercent=70
ENV DOTNET_GCConserveMemory=9

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Journal/Journal.csproj", "Journal/"]
RUN dotnet restore "Journal/Journal.csproj" -r linux-musl-arm64
COPY . .
WORKDIR "/src/Journal"

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Journal.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    --no-restore \
    --self-contained false \
    /p:UseAppHost=false \
    /p:PublishTrimmed=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Journal.dll", "--urls", "http://0.0.0.0:8080"]
