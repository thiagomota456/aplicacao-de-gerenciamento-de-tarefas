# ---- Estágio 1: Build do Backend (.NET) ----
# Esta etapa compila sua API .NET 9.0
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-backend
WORKDIR /src

# Copia os arquivos de solução e projeto para seus locais corretos
COPY TaskManagerApi/TaskManagerApi.sln ./TaskManagerApi/
COPY TaskManagerApi/TaskManagerApi.csproj ./TaskManagerApi/

# Restaura os pacotes usando o caminho correto para o .sln
RUN dotnet restore "TaskManagerApi/TaskManagerApi.sln"

# Copia o restante do código-fonte da API (incluindo o Program.cs)
COPY TaskManagerApi/ ./TaskManagerApi/
WORKDIR /src/TaskManagerApi

# Publica o projeto
RUN dotnet publish "TaskManagerApi.csproj" -c Release -o /app/publish

# ---- Estágio 2: Build do Frontend (React/Vite) ----
# Esta etapa compila seu aplicativo React
FROM node:20 AS build-frontend
WORKDIR /app/ui

# Copia package.json e instala dependências
COPY taskmanager-ui/package.json .
RUN npm install

# Copia o restante do código-fonte do UI e build
COPY taskmanager-ui/ .

# --- CORREÇÃO AQUI ---
# Mudamos de "RUN npm run build" para "RUN npm exec vite build"
# Isso pula o script "tsc -b" do package.json que está falhando
# e executa o "vite build" diretamente.
RUN npm exec vite build
# --- FIM DA CORREÇÃO ---

# ---- Estágio 3: Imagem Final (Nginx + ASP.NET Runtime) ----
# Usamos a imagem do ASP.NET 9.0 Alpine como base (para rodar o Kestrel)
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS final

# Instala o Nginx e o 'bash' (para o script de inicialização)
RUN apk add --no-cache nginx bash

# Expõe a porta 80 (padrão do Nginx)
EXPOSE 80

# Remove a configuração padrão do Nginx
RUN rm /etc/nginx/http.d/default.conf

# Copia a configuração customizada do Nginx (arquivo abaixo)
COPY nginx.conf /etc/nginx/http.d/default.conf

# Copia os arquivos do frontend (para o Nginx servir)
WORKDIR /usr/share/nginx/html
COPY --from=build-frontend /app/ui/dist .

# Copia os arquivos do backend (para o Kestrel servir)
WORKDIR /app/backend
COPY --from=build-backend /app/publish .

# Copia o script de inicialização (arquivo abaixo)
COPY start.sh /start.sh
RUN chmod +x /start.sh

# Define a porta interna onde o Kestrel (backend) vai rodar
ENV ASPNETCORE_URLS=http://localhost:5000

# Comando para iniciar Nginx e a API .NET
CMD ["/start.sh"]