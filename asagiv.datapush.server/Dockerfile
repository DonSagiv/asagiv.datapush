#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["asagiv.datapush.server/asagiv.datapush.server.csproj", "asagiv.datapush.server/"]
COPY ["asagiv.datapush.server.common/asagiv.datapush.server.common.csproj", "asagiv.datapush.server.common/"]
COPY ["asagiv.datapush.common/asagiv.datapush.common.csproj", "asagiv.datapush.common/"]
RUN dotnet restore "asagiv.datapush.server/asagiv.datapush.server.csproj"
COPY . .
WORKDIR "/src/asagiv.datapush.server"
RUN dotnet build "asagiv.datapush.server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "asagiv.datapush.server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "asagiv.datapush.server.dll"]