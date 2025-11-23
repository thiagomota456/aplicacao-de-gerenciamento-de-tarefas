# Stage de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia apenas os .csproj primeiro para aproveitar cache de restore
COPY TaskManagerApi/*.csproj ./TaskManagerApi/
RUN dotnet restore ./TaskManagerApi/TaskManagerApi.csproj

# Copia todo o código e publica
COPY . .
WORKDIR /src/TaskManagerApi
RUN dotnet publish -c Release -o /app/publish --no-restore

# Stage de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TaskManagerApi.dll"]