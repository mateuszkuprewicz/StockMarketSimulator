FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["StockSimulator.WebAPI/StockSimulator.WebAPI.csproj", "StockSimulator.WebAPI/"]
RUN dotnet restore "StockSimulator.WebAPI/StockSimulator.WebAPI.csproj"

COPY . .
WORKDIR "/src/StockSimulator.WebAPI"
RUN dotnet build "StockSimulator.WebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "StockSimulator.WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StockSimulator.WebAPI.dll"]
