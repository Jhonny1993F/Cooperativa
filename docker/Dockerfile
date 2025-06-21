FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Cooperativa/Cooperativa.csproj", "Cooperativa/"]
RUN dotnet restore "Cooperativa/Cooperativa.csproj"
COPY . .
WORKDIR "/src/Cooperativa"
RUN dotnet build "Cooperativa.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Cooperativa.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Cooperativa.dll"]