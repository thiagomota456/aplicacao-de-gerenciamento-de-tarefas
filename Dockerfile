FROM node:20 AS build-frontend
WORKDIR /app/ui

COPY taskmanager-ui/package.json .
RUN npm install

COPY taskmanager-ui/ .
RUN npm run build

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-backend
WORKDIR /src

COPY TaskManagerApi/TaskManagerApi.sln .
COPY TaskManagerApi/TaskManagerApi.csproj TaskManagerApi/

RUN dotnet restore TaskManagerApi/TaskManagerApi.csproj

COPY TaskManagerApi/ .
WORKDIR /src/TaskManagerApi
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build-backend /app/publish .
COPY --from=build-frontend /app/ui/dist ./wwwroot

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "TaskManagerApi.dll"]