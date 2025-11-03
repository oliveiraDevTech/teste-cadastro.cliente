# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copiar arquivos de projeto
COPY ["Driving.Api/Driving.Api.csproj", "Driving.Api/"]
COPY ["Core.Application/Core.Application.csproj", "Core.Application/"]
COPY ["Core.Domain/Core.Domain.csproj", "Core.Domain/"]
COPY ["Core.Infra/Core.Infra.csproj", "Core.Infra/"]
COPY ["Driven.SqlLite/Driven.SqlLite.csproj", "Driven.SqlLite/"]
COPY ["Driven.RabbitMQ/Driven.RabbitMQ.csproj", "Driven.RabbitMQ/"]
COPY ["Test.XUnit/Test.XUnit.csproj", "Test.XUnit/"]

# Restaurar dependências
RUN dotnet restore "Driving.Api/Driving.Api.csproj"

# Copiar código fonte completo
COPY . .

# Build
RUN dotnet build "Driving.Api/Driving.Api.csproj" -c Release -o /app/build

# Etapa 2: Publish
FROM build AS publish
RUN dotnet publish "Driving.Api/Driving.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app

# Instalar ferramentas úteis
RUN apt-get update && apt-get install -y \
    curl \
    && rm -rf /var/lib/apt/lists/*

# Copiar artefatos publicados
COPY --from=publish /app/publish .

# Criar diretório de logs
RUN mkdir -p /app/logs && \
    chmod -R 777 /app/logs

# Criar diretório para banco de dados
RUN mkdir -p /app/data && \
    chmod -R 777 /app/data

# Variáveis de ambiente padrão
ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS=http://+:5000 \
    ASPNETCORE_HTTP_PORT=5000 \
    ConnectionStrings__DefaultConnection="Data Source=/app/data/cliente.db;" \
    JWT_SECRET="sua_chave_super_secreta_com_minimo_32_caracteres_para_producao" \
    RABBITMQ_HOST=rabbitmq \
    RABBITMQ_PORT=5672 \
    RABBITMQ_USER=guest \
    RABBITMQ_PASSWORD=guest

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=40s --retries=3 \
    CMD curl -f http://localhost:5000/health || exit 1

# Expose
EXPOSE 5000

# Entrypoint
ENTRYPOINT ["dotnet", "Driving.Api.dll"]
