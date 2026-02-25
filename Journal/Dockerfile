FROM mcr.microsoft.com/dotnet/aspnet:8.0-bookworm-slim AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
ENV DOTNET_GCLatencyMode=0
ENV DOTNET_GCHeapHardLimitPercent=70
ENV DOTNET_GCConserveMemory=9

FROM mcr.microsoft.com/dotnet/sdk:8.0-bookworm-slim AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Journal/Journal.csproj", "Journal/"]
RUN dotnet restore "Journal/Journal.csproj" -r linux-arm64
COPY . .
WORKDIR "/src/Journal"

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Journal.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    --no-restore \
    -r linux-arm64 \
    --self-contained false \
    /p:UseAppHost=false \
    /p:PublishTrimmed=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8080
RUN mkdir -p /app/data && chown -R 1654:1654 /app/data
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENTRYPOINT ["dotnet", "Journal.dll", "--urls", "http://0.0.0.0:8080"]
