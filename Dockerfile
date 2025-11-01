# ---- Estágio 1: Build do Frontend (React/Vite) ----
FROM node:20 AS build-frontend
WORKDIR /app/ui

# Copia os arquivos de dependência e instala
COPY taskmanager-ui/package.json .
COPY taskmanager-ui/package-lock.json .
RUN npm install

# Copia o restante do código do frontend e executa o build
COPY taskmanager-ui/ .
RUN npm run build
# O resultado estará em /app/ui/dist

# ---- Estágio 2: Build do Backend (.NET) ----
# Usando o SDK 9.0, conforme seu arquivo .csproj.
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-backend
WORKDIR /src

# Copia os arquivos de solução e projeto
COPY TaskManagerApi/TaskManagerApi.sln .
COPY TaskManagerApi/TaskManagerApi.csproj TaskManagerApi/

# Restaura as dependências do .NET
RUN dotnet restore TaskManagerApi/TaskManagerApi.csproj

# Copia o restante do código do backend e publica
COPY TaskManagerApi/ .
WORKDIR /src/TaskManagerApi
RUN dotnet publish -c Release -o /app/publish

# ---- Estágio 3: Imagem Final (Runtime do ASP.NET) ----
# Usando o runtime 9.0, conforme seu arquivo .csproj.
# Se você migrar para o .NET 9, mude esta tag para "9.0"
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Copia os artefatos publicados do backend
COPY --from=build-backend /app/publish .

# Copia o frontend compilado (do estágio 1) para a pasta 'wwwroot'
# O app.UseStaticFiles() no seu Program.cs servirá estes arquivos
COPY --from=build-frontend /app/ui/dist ./wwwroot

# O Render define a variável de ambiente PORT (padrão 8080).
# Esta ENV diz ao .NET para escutar em qualquer IP (0.0.0.0) na porta 8080.
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Ponto de entrada para iniciar a API
ENTRYPOINT ["dotnet", "TaskManagerApi.dll"]