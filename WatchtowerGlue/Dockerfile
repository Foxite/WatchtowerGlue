﻿FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["WatchtowerGlue/WatchtowerGlue.csproj", "WatchtowerGlue/"]
RUN dotnet restore "WatchtowerGlue/WatchtowerGlue.csproj"
COPY . .
WORKDIR "/src/WatchtowerGlue"
RUN dotnet build "WatchtowerGlue.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WatchtowerGlue.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["./WatchtowerGlue"]
