FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["RTChatBackend.Api/RTChatBackend.Api.csproj", "RTChatBackend.Api/"]
COPY ["RTChatBackend.Application/RTChatBackend.Application.csproj", "RTChatBackend.Application/"]
COPY ["RTChatBackend.Core/RTChatBackend.Core.csproj", "RTChatBackend.Core/"]
COPY ["RTChatBackend.Infrastructure/RTChatBackend.Infrastructure.csproj", "RTChatBackend.Infrastructure/"]

RUN dotnet restore "RTChatBackend.Api/RTChatBackend.Api.csproj"

COPY . .
WORKDIR "/src/RTChatBackend.Api"
RUN dotnet build "RTChatBackend.Api.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "RTChatBackend.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RTChatBackend.Api.dll"]
