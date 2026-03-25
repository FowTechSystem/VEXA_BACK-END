# Imagem de runtime pequena: multi-stage (SDK só no build).
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["CRMVEXA.csproj", "./"]
RUN dotnet restore "CRMVEXA.csproj"

COPY . .
RUN dotnet publish "CRMVEXA.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080
ENTRYPOINT ["dotnet", "CRMVEXA.dll"]
