FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY BaseCoreApi/BaseCoreApi.csproj BaseCoreApi/
RUN dotnet restore BaseCoreApi/BaseCoreApi.csproj
COPY . .
WORKDIR /src/BaseCoreApi
RUN dotnet build BaseCoreApi.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish BaseCoreApi.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "BaseCoreApi.dll"]
