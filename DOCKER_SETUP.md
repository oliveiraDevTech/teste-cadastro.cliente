# Docker Setup - Cadastro de Clientes

## 📋 Visão Geral

O projeto está configurado para rodar em containers Docker com:
- ✅ **Migrations automáticas** ao iniciar a aplicação
- ✅ **Banco de dados** SQLite persistente
- ✅ **Variáveis de ambiente** para configuração dinâmica
- ✅ **RabbitMQ** para mensageria
- ✅ **Health checks** configurados

## 🚀 Como Usar

### 1. Clonar o repositório
```bash
git clone <seu-repositorio>
cd teste-cadastro.cliente
```

### 2. Preparar arquivo de variáveis de ambiente

Copie o arquivo `.env.example` para `.env`:
```bash
cp .env.example .env
```

Edite o `.env` conforme suas necessidades:
```env
JWT_SECRET=sua_chave_secreta_aqui
RABBITMQ_USER=seu_usuario
RABBITMQ_PASSWORD=sua_senha
```

### 3. Construir e iniciar os containers

#### Opção A: Com docker-compose (Recomendado)
```bash
docker-compose up -d --build
```

#### Opção B: Apenas a aplicação
```bash
docker build -t cadastro-clientes-api .
docker run -d \
  --name cadastro-clientes-api \
  -p 5000:5000 \
  -e ConnectionStrings__DefaultConnection="Data Source=/app/data/cliente.db;" \
  -e JWT_SECRET="sua_chave_secreta" \
  -e RABBITMQ_HOST="localhost" \
  -v $(pwd)/data:/app/data \
  -v $(pwd)/logs:/app/logs \
  cadastro-clientes-api
```

## 📊 Verificando a aplicação

### Health check
```bash
curl http://localhost:5000/health
```

### Swagger UI
Acesse: http://localhost:5000

### Logs
```bash
docker logs cadastro-clientes-api
```

## 🔐 Variáveis de Ambiente

### Obrigatórias
| Variável | Padrão | Descrição |
|----------|--------|-----------|
| `JWT_SECRET` | Chave padrão | Chave para assinar JWT tokens |
| `ConnectionStrings__DefaultConnection` | `/app/data/cliente.db` | String de conexão do SQLite |

### Opcionais
| Variável | Padrão | Descrição |
|----------|--------|-----------|
| `JWT_ISSUER` | `CadastroClientesApi` | Emissor do JWT |
| `JWT_AUDIENCE` | `CadastroClientesApp` | Audiência do JWT |
| `JWT_EXPIRATION` | `60` | Expiração do JWT em minutos |
| `RABBITMQ_HOST` | `rabbitmq` | Host do RabbitMQ |
| `RABBITMQ_PORT` | `5672` | Porta do RabbitMQ |
| `RABBITMQ_USER` | `guest` | Usuário RabbitMQ |
| `RABBITMQ_PASSWORD` | `guest` | Senha RabbitMQ |
| `RABBITMQ_VHOST` | `/` | Virtual Host RabbitMQ |

## 🗄️ Banco de Dados

### Migrations
As migrations são executadas **automaticamente** ao iniciar o container:

1. **20250101000000_InitialCreate** - Cria tabela `Clientes`
2. **20250101000001_AddInformacoesFinanceirasAndUsuario** - Cria tabelas `InformacoesFinanceirasClientes` e `Usuarios`

### Arquivos persistentes
O banco de dados é armazenado em:
- **Host**: `./data/cliente.db`
- **Container**: `/app/data/cliente.db`

Para resetar o banco, delete o arquivo:
```bash
rm -f ./data/cliente.db
docker-compose restart api
```

## 🐰 RabbitMQ

### Acessar Management Console
- URL: http://localhost:15672
- Usuário: `guest`
- Senha: `guest`

### Verificar conexão
```bash
docker logs cadastro-clientes-api | grep RabbitMQ
```

## ⚠️ Troubleshooting

### Erro de permissão no banco de dados
```bash
chmod -R 777 ./data
chmod -R 777 ./logs
```

### Porta 5000 já em uso
```bash
docker-compose down
# Ou use outra porta
docker run -p 5001:5000 cadastro-clientes-api
```

### RabbitMQ não conecta
- Verifique se o container está rodando: `docker ps`
- Verifique logs: `docker logs cadastro-rabbitmq`
- A aplicação continua funcionando sem RabbitMQ (com fallback)

### Limpar tudo e recomeçar
```bash
docker-compose down -v  # Remove containers, networks e volumes
rm -rf ./data ./logs
docker-compose up -d --build
```

## 📝 Estrutura de pastas

```
.
├── Dockerfile                 # Configuração Docker
├── docker-compose.yml         # Orquestração de containers
├── .env.example               # Template de variáveis
├── DOCKER_SETUP.md           # Esta documentação
├── data/                      # Banco de dados (persistente)
├── logs/                      # Logs da aplicação
└── Driving.Api/
    ├── Program.cs            # Entrypoint com migrations
    └── appsettings.json      # Configurações
```

## 🔄 Fluxo de Inicialização

1. Container inicia
2. Aplicação lê variáveis de ambiente
3. Aplica migrations pendentes (`Database.Migrate()`)
4. Cria tabelas se não existirem
5. Conecta ao RabbitMQ (com fallback)
6. Inicia servidor na porta 5000

## 📞 Suporte

Para mais detalhes, veja:
- `README.md` - Documentação geral
- `ARCHITECTURE.md` - Arquitetura do projeto
- `DOCUMENTACAO_TECNICA.md` - Documentação técnica
