#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
EXPOSE 10000 10001
WORKDIR /app

# Update the existing packages, install dependencies and clean-up lists.
RUN mkdir -p /usr/share/man/man1
RUN apt-get update \
    && apt-get install -y gnupg2 \
         apt-utils \
         libx11-dev \
         libvlc-dev \
         vlc \
    && apt-key adv --keyserver keyserver.ubuntu.com --recv B6391CB2CFBA643D \
    && echo "deb http://download.zeroc.com/Ice/3.7/debian10 stable main" >> /etc/apt/sources.list.d/ice.list \
    && apt-get update \
    && apt-get install --no-install-recommends --no-install-suggests -y \
        zeroc-ice-all-runtime \
        zeroc-ice-all-dev \
    && rm -rf /var/lib/apt/lists/*

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src

# Update the existing packages, install dependencies and clean-up lists.
RUN mkdir -p /usr/share/man/man1
RUN apt-get update \
    && apt-get install -y gnupg2 \
    && apt-key adv --keyserver keyserver.ubuntu.com --recv B6391CB2CFBA643D \
    && echo "deb http://download.zeroc.com/Ice/3.7/debian10 stable main" >> /etc/apt/sources.list.d/ice.list \
    && apt-get update \
    && apt-get install --no-install-recommends --no-install-suggests -y \
        zeroc-ice-all-runtime \
        zeroc-ice-all-dev \
    && rm -rf /var/lib/apt/lists/*

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