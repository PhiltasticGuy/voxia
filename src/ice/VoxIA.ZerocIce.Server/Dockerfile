#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/ice/VoxIA.ZerocIce.Server/VoxIA.ZerocIce.Server.csproj", "src/ice/VoxIA.ZerocIce.Server/"]
COPY ["src/ice/VoxIA.ZerocIce.Core/VoxIA.ZerocIce.Core.csproj", "src/ice/VoxIA.ZerocIce.Core/"]
COPY ["src/mobile/VoxIA.Mobile/VoxIA.Mobile/VoxIA.Mobile.csproj", "src/mobile/VoxIA.Mobile/VoxIA.Mobile/"]
RUN dotnet restore "src/ice/VoxIA.ZerocIce.Server/VoxIA.ZerocIce.Server.csproj"
COPY . .
WORKDIR "/src/src/ice/VoxIA.ZerocIce.Server"
RUN dotnet build "VoxIA.ZerocIce.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "VoxIA.ZerocIce.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VoxIA.ZerocIce.Server.dll"]