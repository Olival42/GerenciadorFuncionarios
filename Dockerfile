FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore
RUN dotnet build -c Release --no-restore

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GerenciadorFuncionarios.dll"]