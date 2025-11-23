# Stage de build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copia apenas o .csproj primeiro para aproveitar cache de restore
COPY *.csproj ./
RUN dotnet restore

# Copia todo o código e publica
COPY . .
RUN dotnet publish -c Release -o /app/publish --no-restore

# Stage de runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TaskManagerApi.dll"]