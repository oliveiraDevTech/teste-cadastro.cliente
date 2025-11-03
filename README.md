# Cadastro de Clientes API

Plataforma robusta para gestão de clientes com autenticação JWT, processamento de crédito e sistema de mensageria assíncrona.

## 📋 Índice

- [Visão Geral](#visão-geral)
- [Arquitetura](#arquitetura)
- [Tecnologias](#tecnologias)
- [Instalação](#instalação)
- [Configuração](#configuração)
- [Como Usar](#como-usar)
- [Estrutura do Projeto](#estrutura-do-projeto)
- [Endpoints da API](#endpoints-da-api)
- [Autenticação](#autenticação)
- [Modelos de Dados](#modelos-de-dados)
- [Desenvolvimento](#desenvolvimento)
- [Testes](#testes)
- [Deploy](#deploy)
- [Troubleshooting](#troubleshooting)

---

## 🎯 Visão Geral

O **Cadastro de Clientes API** é uma aplicação backend desenvolvida em **.NET 8** que oferece:

- **Gestão Completa de Clientes**: Criar, listar, buscar, atualizar e deletar clientes
- **Análise de Crédito**: Scoring e ranking de crédito com elegibilidade para cartão de crédito
- **Autenticação JWT**: Segurança baseada em tokens com expiração configurável
- **Processamento Assíncrono**: RabbitMQ para operações em background
- **Persistência de Dados**: SQLite para desenvolvimento, facilmente migrável para SQL Server
- **Logging e Cache**: Infraestrutura completa para monitoramento e performance
- **API bem documentada**: Swagger/OpenAPI integrado

### Funcionalidades Principais

✅ **Autenticação & Autorização**
- Login e registro de usuários
- Tokens JWT com expiração
- Bloqueio automático após tentativas falhadas
- Confirmação de email

✅ **Gestão de Clientes**
- Cadastro com validação completa (CPF, Email, Telefone, CEP)
- Atualização de dados pessoais
- Pesquisa por nome com paginação
- Soft delete com manutenção de histórico

✅ **Análise de Crédito**
- Score de crédito (0-1000)
- Ranking de crédito (1-5)
- Elegibilidade automática para cartão de crédito
- Histórico de atualizações

✅ **Infraestrutura Avançada**
- RabbitMQ com fallback gracioso
- Cache em memória
- Logging estruturado
- Envio de emails
- Migrations automáticas

---

## 🏗️ Arquitetura

O projeto segue a **Arquitetura em Camadas (Layered Architecture)** com separação clara de responsabilidades:

```
┌─────────────────────────────────────────────────────┐
│              Driving.Api (API REST)                 │
│         Controllers & HTTP Request Pipeline         │
└──────────────────────┬──────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────┐
│         Core.Application (Casos de Uso)             │
│    Services, DTOs, Mappers, Validators              │
└──────────────────────┬──────────────────────────────┘
                       │
        ┌──────────────┼──────────────┐
        │              │              │
┌───────▼────┐  ┌──────▼──────┐  ┌───▼─────────┐
│Core.Domain │  │Core.Infra   │  │ Repositories│
│  Entities  │  │ (Email,     │  │ (Database)  │
│ & Validação│  │ Logging,    │  │             │
│            │  │ Cache)      │  │             │
└────────────┘  └─────────────┘  └─────────────┘
                       │
        ┌──────────────┼──────────────┐
        │              │              │
┌───────▼──────┐ ┌─────▼──────┐ ┌───▼──────────┐
│Driven.SqlLite│ │Driven.      │ │Configuração  │
│(Persistência)│ │RabbitMQ     │ │ & Secrets    │
│  EF Core     │ │(Mensageria) │ │              │
└──────────────┘ └─────────────┘ └──────────────┘
```

### Padrões Implementados

- **Domain-Driven Design (DDD)**: Entidades ricas com lógica de negócio
- **Repository Pattern**: Abstração da camada de dados
- **Dependency Injection**: Configuração centralizada em DependencyInjection.cs
- **DTO Pattern**: Separação entre modelos de domínio e API
- **Service Layer**: Orquestração de casos de uso

---

## 🛠️ Tecnologias

### Backend
- **Runtime**: .NET 8
- **Framework**: ASP.NET Core 8
- **ORM**: Entity Framework Core 8
- **Autenticação**: JWT Bearer
- **Banco de Dados**: SQLite (desenvolvimento) / SQL Server (produção)

### Infraestrutura
- **Mensageria**: RabbitMQ com fallback gracioso
- **Cache**: In-memory (IMemoryCache)
- **Email**: SMTP (configurável)
- **Logging**: Serilog
- **API Documentation**: Swagger/OpenAPI

### Testes
- **Framework**: XUnit
- **Mocking**: Moq

### DevOps
- **Containerização**: Docker
- **CI/CD**: Pronto para GitHub Actions

---

## 💾 Instalação

### Pré-requisitos

- **.NET 8 SDK** ou superior
- **Visual Studio 2022** (ou VS Code)
- **RabbitMQ** (opcional, mas recomendado)
- **Git**

### Passos

1. **Clone o repositório**
   ```bash
   git clone <repository-url>
   cd Cliente
   ```

2. **Restaure as dependências**
   ```bash
   dotnet restore
   ```

3. **Configure o banco de dados**
   ```bash
   cd Driven.SqlLite
   dotnet ef database update
   cd ..
   ```

4. **Inicie a aplicação**
   ```bash
   cd Driving.Api
   dotnet run
   ```

5. **Acesse a API**
   - Swagger UI: http://localhost:5000 (ou a porta configurada)
   - Health Check: http://localhost:5000/health

---

## ⚙️ Configuração

### Arquivo de Configuração: `appsettings.json`

```json
{
  "Jwt": {
    "Secret": "sua_chave_super_secreta_com_minimo_32_caracteres_para_producao",
    "Issuer": "CadastroClientesApi",
    "Audience": "CadastroClientesApp",
    "ExpirationMinutes": 60
  },
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=cadastro_clientes.db;"
  },
  "RabbitMQ": {
    "HostName": "localhost",
    "Port": 5672,
    "UserName": "guest",
    "Password": "guest",
    "VirtualHost": "/"
  },
  "Email": {
    "SmtpServer": "smtp.seu-provider.com",
    "Port": 587,
    "Username": "seu-email@example.com",
    "Password": "sua-senha",
    "EnableSSL": true,
    "FromAddress": "noreply@cadastroclientes.com",
    "FromName": "Cadastro de Clientes"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    }
  }
}
```

### Variáveis de Ambiente

Para segurança em produção, use variáveis de ambiente:

```bash
# JWT
export Jwt__Secret="sua_chave_secreta_muito_comprida"
export Jwt__Issuer="CadastroClientesApi"
export Jwt__Audience="CadastroClientesApp"
export Jwt__ExpirationMinutes=60

# Database
export ConnectionStrings__DefaultConnection="Server=seu-servidor;Database=cadastro_clientes;User Id=sa;Password=SuaSenha123;"

# RabbitMQ
export RabbitMQ__HostName="rabbitmq.seu-servidor.com"
export RabbitMQ__Port=5672
export RabbitMQ__UserName="seu-usuario"
export RabbitMQ__Password="sua-senha"

# Email
export Email__SmtpServer="smtp.seu-provider.com"
export Email__Username="seu-email@example.com"
export Email__Password="sua-senha"
```

### Ambiente de Desenvolvimento

Use `appsettings.Development.json`:

```json
{
  "Jwt": {
    "Secret": "chave-temporaria-para-desenvolvimento-apenas",
    "ExpirationMinutes": 1440
  },
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=cadastro_clientes_dev.db;"
  },
  "RabbitMQ": {
    "HostName": "localhost",
    "Enabled": false
  }
}
```

---

## 🚀 Como Usar

### 1. Registrar um Novo Usuário

```bash
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "login": "usuario123",
    "email": "usuario@example.com",
    "senha": "Senha123!",
    "nomeCompleto": "João da Silva",
    "telefone": "11987654321"
  }'
```

**Resposta (201)**:
```json
{
  "sucesso": true,
  "mensagem": "Usuário registrado com sucesso",
  "dados": {
    "id": "uuid",
    "login": "usuario123",
    "email": "usuario@example.com",
    "nomeCompleto": "João da Silva"
  }
}
```

### 2. Fazer Login

```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "login": "usuario123",
    "senha": "Senha123!"
  }'
```

**Resposta (200)**:
```json
{
  "sucesso": true,
  "mensagem": "Login realizado com sucesso",
  "dados": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "expiresIn": 3600
  }
}
```

### 3. Criar um Cliente

```bash
curl -X POST http://localhost:5000/api/clientes \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer SEU_TOKEN_JWT" \
  -d '{
    "nome": "Maria Santos",
    "email": "maria@example.com",
    "telefone": "11987654321",
    "cpf": "12345678901",
    "endereco": "Rua das Flores, 123",
    "cidade": "São Paulo",
    "estado": "SP",
    "cep": "01234567"
  }'
```

**Resposta (201)**:
```json
{
  "sucesso": true,
  "mensagem": "Cliente criado com sucesso",
  "dados": {
    "id": "uuid",
    "nome": "Maria Santos",
    "email": "maria@example.com",
    "rankingCredito": 0,
    "scoreCredito": 0,
    "aptoParaCartaoCredito": false
  }
}
```

### 4. Listar Clientes com Paginação

```bash
curl -X GET "http://localhost:5000/api/clientes?pagina=1&itensPorPagina=10" \
  -H "Authorization: Bearer SEU_TOKEN_JWT"
```

**Resposta (200)**:
```json
{
  "sucesso": true,
  "mensagem": "Clientes listados com sucesso",
  "dados": {
    "items": [
      {
        "id": "uuid",
        "nome": "Maria Santos",
        "email": "maria@example.com",
        "rankingCredito": 0,
        "scoreCredito": 0
      }
    ],
    "paginacao": {
      "paginaAtual": 1,
      "itensPorPagina": 10,
      "totalItens": 1,
      "totalPaginas": 1
    }
  }
}
```

### 5. Pesquisar Clientes por Nome

```bash
curl -X GET "http://localhost:5000/api/clientes/pesquisar?nome=Maria&pagina=1" \
  -H "Authorization: Bearer SEU_TOKEN_JWT"
```

### 6. Obter Detalhes de um Cliente

```bash
curl -X GET http://localhost:5000/api/clientes/{id} \
  -H "Authorization: Bearer SEU_TOKEN_JWT"
```

### 7. Atualizar Cliente

```bash
curl -X PUT http://localhost:5000/api/clientes \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer SEU_TOKEN_JWT" \
  -d '{
    "id": "uuid",
    "nome": "Maria Santos Silva",
    "email": "maria.silva@example.com",
    "telefone": "11987654322",
    "endereco": "Rua das Flores, 456",
    "cidade": "São Paulo",
    "estado": "SP",
    "cep": "01234568"
  }'
```

### 8. Deletar Cliente

```bash
curl -X DELETE http://localhost:5000/api/clientes/{id} \
  -H "Authorization: Bearer SEU_TOKEN_JWT"
```

---

## 🔑 Autenticação

### JWT Bearer Token

Todos os endpoints (exceto `/auth/*`) requerem autenticação com JWT.

**Formato do Header**:
```
Authorization: Bearer <token>
```

### Token JWT

O token contém:
- **iss** (Issuer): CadastroClientesApi
- **aud** (Audience): CadastroClientesApp
- **exp** (Expiration): Tempo de expiração (padrão: 60 minutos)
- **sub** (Subject): ID do usuário
- **email**: Email do usuário
- **login**: Login do usuário
- **tipo**: Tipo de usuário

**Decodificação de Token** (em https://jwt.io/):
```
Header:
{
  "alg": "HS256",
  "typ": "JWT"
}

Payload:
{
  "iss": "CadastroClientesApi",
  "aud": "CadastroClientesApp",
  "sub": "uuid",
  "email": "usuario@example.com",
  "login": "usuario123",
  "tipo": "Cliente",
  "exp": 1699999999
}
```

---

## 📊 Modelos de Dados

### Cliente

```csharp
public class Cliente : BaseEntity
{
    public string Nome { get; set; }                      // Nome completo (3-150 caracteres)
    public string Email { get; set; }                     // Email único (validado)
    public string Telefone { get; set; }                  // Telefone (10-11 dígitos)
    public string Cpf { get; set; }                       // CPF (11 dígitos, validado)
    public string Endereco { get; set; }                  // Endereço (5-200 caracteres)
    public string Cidade { get; set; }                    // Cidade (2-100 caracteres)
    public string Estado { get; set; }                    // UF (2 caracteres: SP, RJ, etc)
    public string Cep { get; set; }                       // CEP (8 dígitos: XXXXX-XXX)
    public int RankingCredito { get; set; }               // Ranking 0-5
    public int ScoreCredito { get; set; }                 // Score 0-1000
    public DateTime? DataAtualizacaoRanking { get; set; } // Último update de crédito
    public bool AptoParaCartaoCredito { get; set; }       // Ranking >= 3 && Score >= 600
    public DateTime DataCriacao { get; set; }             // Data de criação
    public DateTime DataAtualizacao { get; set; }         // Última atualização
    public bool Ativo { get; set; }                       // Status (soft delete)
}
```

**Validações de Domínio**:
- **CPF**: Valida algoritmo de dígitos verificadores
- **Email**: Formato válido
- **Telefone**: 10-11 dígitos
- **CEP**: 8 dígitos
- **Estado**: Exatamente 2 caracteres
- **Nomes**: Comprimento mínimo/máximo

### Usuário

```csharp
public class Usuario : BaseEntity
{
    public string Login { get; set; }                   // Login único (3-50 caracteres)
    public string Email { get; set; }                   // Email único
    public string SenhaHash { get; set; }               // Hash SHA256 com PBKDF2
    public string SenhaSalt { get; set; }               // Salt em Base64
    public Guid? ClienteId { get; set; }                // Referência opcional a Cliente
    public string NomeCompleto { get; set; }            // Nome (3-150 caracteres)
    public string Telefone { get; set; }                // Telefone (10-11 dígitos)
    public DateTime? DataUltimoAcesso { get; set; }     // Último login
    public DateTime? DataAlteracaoSenha { get; set; }   // Última mudança de senha
    public string TipoUsuario { get; set; }             // Admin, Cliente, Operador
    public string Permissoes { get; set; }              // Separadas por vírgula
    public bool IsAdmin { get; set; }                   // Flag de administrador
    public int TentativasLoginFalhadas { get; set; }    // Contador (reseta em sucesso)
    public DateTime? DataBloqueio { get; set; }         // Data/hora do bloqueio
    public string? MotivoBloqueio { get; set; }         // Motivo do bloqueio
    public bool EmailConfirmado { get; set; }           // Status de verificação
    public string? TokenConfirmacaoEmail { get; set; }  // Token temporário
}
```

**Segurança**:
- Senha hashida com PBKDF2 (10.000 iterações) + SHA256
- Salt aleatório de 32 bytes
- Bloqueio automático após 5 tentativas falhadas
- Tokens de confirmação aleatórios (32 caracteres)

### InformaçõesFinanceirasCliente

```csharp
public class InformacoesFinanceirasCliente : BaseEntity
{
    public Guid ClienteId { get; set; }              // Referência ao Cliente
    public decimal RendaMensal { get; set; }         // Renda mensal
    public decimal DividaTotal { get; set; }         // Dívida total
    public decimal LimiteCredito { get; set; }       // Limite de crédito
    public int DiasAtraso { get; set; }              // Dias em atraso
    public DateTime DataAvaliacaoCredito { get; set; } // Data da última avaliação
    public string HistoricoCredito { get; set; }     // Notas do histórico
}
```

---

## 📁 Estrutura do Projeto

```
Cliente/
├── Core.Domain/                          # Camada de Domínio
│   ├── Entities/
│   │   ├── Cliente.cs                   # Entidade Cliente com validações
│   │   ├── Usuario.cs                   # Entidade Usuario com segurança
│   │   └── InformacoesFinanceirasCliente.cs
│   ├── Common/
│   │   └── BaseEntity.cs                # Classe base para todas as entidades
│   ├── Interfaces/
│   │   ├── IClienteRepository.cs
│   │   ├── IUsuarioRepository.cs
│   │   └── IMessageBus.cs
│   └── Core.Domain.csproj
│
├── Core.Application/                     # Camada de Aplicação
│   ├── Services/
│   │   ├── ClienteService.cs            # Casos de uso de clientes
│   │   └── AuthenticationService.cs     # Autenticação e geração JWT
│   ├── DTOs/
│   │   ├── Request/
│   │   │   ├── ClienteCreateDto.cs
│   │   │   ├── ClienteUpdateDto.cs
│   │   │   └── LoginRequest.cs
│   │   └── Response/
│   │       ├── ClienteResponseDto.cs
│   │       ├── ClienteListDto.cs
│   │       └── ApiResponseDto.cs
│   ├── Mappers/
│   │   └── ClienteMappers.cs            # Mapeamento entidade <-> DTO
│   ├── Validators/
│   │   └── ClienteValidator.cs          # Validações de negócio
│   ├── Interfaces/
│   │   ├── IClienteService.cs
│   │   └── IAuthenticationService.cs
│   ├── Core.Application.DependencyInjection.cs
│   └── Core.Application.csproj
│
├── Core.Infra/                           # Camada de Infraestrutura
│   ├── Logging/
│   │   └── LoggerService.cs             # Serviço de logging
│   ├── Email/
│   │   └── EmailService.cs              # Serviço de envio de emails
│   ├── Caching/
│   │   └── CacheService.cs              # Serviço de cache em memória
│   ├── Repositories/
│   │   └── GenericRepository.cs         # Repositório base genérico
│   ├── Extensions/
│   │   └── LoggerExtensions.cs
│   ├── Core.Infra.DependencyInjection.cs
│   └── Core.Infra.csproj
│
├── Driven.SqlLite/                       # Camada de Dados (SQLite)
│   ├── Data/
│   │   └── ApplicationDbContext.cs      # DbContext do EF Core
│   ├── Migrations/
│   │   ├── 20240101000000_InitialCreate.cs
│   │   └── ...
│   ├── Repositories/
│   │   ├── ClienteRepository.cs         # Implementação para Cliente
│   │   └── UsuarioRepository.cs         # Implementação para Usuario
│   ├── Driven.SqlLite.DependencyInjection.cs
│   └── Driven.SqlLite.csproj
│
├── Driven.RabbitMQ/                      # Camada de Mensageria
│   ├── Services/
│   │   └── RabbitMQService.cs           # Produtor/Consumidor
│   ├── Interfaces/
│   │   └── IMessageBus.cs
│   ├── Events/
│   │   ├── ClienteCriadoEvent.cs
│   │   └── ClienteAtualizadoEvent.cs
│   ├── Settings/
│   │   └── RabbitMQSettings.cs
│   ├── Driven.RabbitMQ.DependencyInjection.cs
│   └── Driven.RabbitMQ.csproj
│
├── Driving.Api/                          # Camada de API (Apresentação)
│   ├── Controllers/
│   │   ├── AuthController.cs            # Autenticação (/api/auth)
│   │   └── ClientesController.cs        # Clientes (/api/clientes)
│   ├── Extensions/
│   │   └── ControllerExtensions.cs
│   ├── Program.cs                       # Configuração da aplicação
│   ├── appsettings.json                 # Configurações padrão
│   ├── appsettings.Development.json     # Configurações desenvolvimento
│   ├── Driving.Api.csproj
│   └── Driving.Api.http                 # Requests para testes
│
├── Test.XUnit/                           # Testes Automatizados
│   ├── Services/
│   │   └── ClienteServiceTests.cs
│   ├── Controllers/
│   │   └── ClientesControllerTests.cs
│   ├── Entities/
│   │   └── ClienteEntityTests.cs
│   └── Test.XUnit.csproj
│
├── Cadastro.Clientes.sln                 # Solution do Visual Studio
├── Dockerfile                            # Containerização
├── .dockerignore
├── README.md                             # Este arquivo
├── appsettings.json                      # Configurações globais
└── cadastro_clientes.db                  # Banco de dados SQLite

```

---

## 🔌 Endpoints da API

### Autenticação (`/api/auth`)

| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| POST | `/auth/register` | Registrar novo usuário | ❌ |
| POST | `/auth/login` | Fazer login | ❌ |
| POST | `/auth/refresh-token` | Renovar token | ✅ |
| POST | `/auth/confirm-email` | Confirmar email | ❌ |

### Clientes (`/api/clientes`)

| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| GET | `/clientes` | Listar clientes (paginado) | ✅ |
| GET | `/clientes/{id}` | Obter cliente por ID | ✅ |
| GET | `/clientes/pesquisar?nome=x` | Pesquisar por nome | ✅ |
| POST | `/clientes` | Criar novo cliente | ✅ |
| PUT | `/clientes` | Atualizar cliente | ✅ |
| DELETE | `/clientes/{id}` | Deletar cliente (soft delete) | ✅ |

### Query Parameters

**Paginação**:
- `pagina` (int, padrão: 1): Número da página
- `itensPorPagina` (int, padrão: 10, máximo: 100): Itens por página

**Pesquisa**:
- `nome` (string): Termo de busca (busca parcial)

---

## 🔐 Segurança

### Implementações de Segurança

✅ **JWT Bearer Authentication**
- Tokens com expiração configurável
- Validação de issuer e audience
- Clock skew zero para precisão temporal

✅ **Password Security**
- PBKDF2 com SHA256
- 10.000 iterações de hash
- Salt aleatório de 32 bytes

✅ **Account Protection**
- Bloqueio após 5 tentativas falhadas
- Confirmação de email obrigatória
- Histórico de tentativas de acesso

✅ **CORS Policy**
- Configuração customizável
- Padrão: Aceita todas as origens (configure em produção!)

✅ **Validação de Dados**
- Validação de CPF com algoritmo de dígitos verificadores
- Validação de Email com formato padrão
- Sanitização de entradas

### Recomendações de Segurança para Produção

1. **JWT Secret**: Use uma chave muito longa e aleatória
   ```bash
   dotnet user-secrets set "Jwt:Secret" "sua-chave-super-secreta-com-minimo-64-caracteres"
   ```

2. **CORS**: Configure apenas origens confiáveis
   ```csharp
   policy.WithOrigins("https://seu-dominio.com")
         .AllowAnyMethod()
         .AllowAnyHeader();
   ```

3. **HTTPS**: Sempre use HTTPS em produção
4. **Rate Limiting**: Implemente rate limiting em produção
5. **API Keys**: Considere adicionar API keys para acesso externo
6. **Logs de Segurança**: Monitore tentativas de login suspeitas

---

## 🧪 Desenvolvimento

### Estrutura de Testes

O projeto usa **XUnit** para testes automatizados:

```bash
cd Test.XUnit
dotnet test
```

### Tipos de Testes

**Testes de Entidade** (`ClienteEntityTests.cs`):
```csharp
[Fact]
public void CriarCliente_ComDadosValidos_DeveSerSucesso()
{
    // Arrange
    var nome = "João Silva";
    var email = "joao@example.com";
    // ...

    // Act
    var cliente = Cliente.Criar(nome, email, ...);

    // Assert
    Assert.NotNull(cliente);
    Assert.Equal(nome, cliente.Nome);
}
```

**Testes de Serviço** (`ClienteServiceTests.cs`):
```csharp
[Fact]
public async Task CriarClienteAsync_ComEmailDuplicado_DeveFalhar()
{
    // Arrange
    var mockRepository = new Mock<IClienteRepository>();
    mockRepository.Setup(r => r.EmailJaRegistradoAsync(...))
        .ReturnsAsync(true);

    var service = new ClienteService(mockRepository.Object);

    // Act
    var resultado = await service.CriarAsync(clienteDto);

    // Assert
    Assert.False(resultado.Sucesso);
}
```

### Executar Testes

```bash
# Todos os testes
dotnet test

# Com verbose output
dotnet test -v detailed

# Teste específico
dotnet test --filter "ClassName=ClienteEntityTests"

# Com cobertura de código
dotnet test /p:CollectCoverage=true
```

---

## 🐳 Docker

### Build da Imagem

```bash
docker build -t cadastro-clientes-api:latest .
```

### Executar Container

```bash
docker run -d \
  --name cadastro-api \
  -p 5000:8080 \
  -e Jwt__Secret="chave-secreta" \
  -e RabbitMQ__HostName="rabbitmq" \
  cadastro-clientes-api:latest
```

### Docker Compose (com RabbitMQ)

```yaml
version: '3.8'

services:
  api:
    build: .
    ports:
      - "5000:8080"
    environment:
      - Jwt__Secret=sua-chave-secreta
      - RabbitMQ__HostName=rabbitmq
    depends_on:
      - rabbitmq

  rabbitmq:
    image: rabbitmq:3.12-management
    ports:
      - "5672:5672"
      - "15672:15672"
    environment:
      - RABBITMQ_DEFAULT_USER=guest
      - RABBITMQ_DEFAULT_PASS=guest
```

Inicie com:
```bash
docker-compose up -d
```

---

## 📋 Migrations do Banco de Dados

### Criar uma Nova Migration

```bash
cd Driven.SqlLite
dotnet ef migrations add NomeDaMigracao -o Migrations
```

### Aplicar Migrations

```bash
dotnet ef database update
```

### Ver Histórico de Migrations

```bash
dotnet ef migrations list
```

### Remover Última Migration (não aplicada)

```bash
dotnet ef migrations remove
```

### Revertir para Migration Anterior

```bash
dotnet ef database update NomeDaMigracaoAnterior
```

---

## 🚀 Deploy

### Publicar para Produção

```bash
dotnet publish -c Release -o ./publish
```

### Azure App Service

```bash
# Criar App Service
az appservice plan create --name CadastroClientesPlan --resource-group MeuGrupo --sku B1 --is-linux

# Publicar
dotnet publish -c Release -o ./publish
cd publish && zip -r ../app.zip . && cd ..

# Upload
az webapp deployment source config-zip --resource-group MeuGrupo --name MeuApp --src app.zip
```

### GitHub Actions (CI/CD)

Crie `.github/workflows/deploy.yml`:

```yaml
name: Deploy

on:
  push:
    branches: [ main ]

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest

    steps:
    - uses: actions/checkout@v2

    - name: Setup .NET
      uses: actions/setup-dotnet@v1
      with:
        dotnet-version: '8.0'

    - name: Restore
      run: dotnet restore

    - name: Test
      run: dotnet test

    - name: Publish
      run: dotnet publish -c Release -o ./publish

    - name: Deploy
      uses: azure/appservice-deploy@v2
      with:
        app-name: 'seu-app-name'
        publish-profile: ${{ secrets.AZURE_PUBLISH_PROFILE }}
```

---

## 🔧 Troubleshooting

### Erro: "RabbitMQ não foi inicializado"

**Solução**: RabbitMQ é opcional. A aplicação continua funcionando sem mensageria. Para usar RabbitMQ:

```bash
# Iniciar RabbitMQ localmente (Docker)
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3.12-management
```

Acesse a interface web em `http://localhost:15672` (guest/guest)

### Erro: "Database is locked" (SQLite)

**Solução**: SQLite tem limitações com concorrência. Para produção, migre para SQL Server:

```csharp
// Em Driven.SqlLite.DependencyInjection.cs
services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString)
);
```

### Erro: "Invalid JWT token"

**Verificar**:
1. Token está expirado? Faça novo login
2. Secret está correto? Verifique appsettings.json
3. Token está malformado? Decodifique em jwt.io

```bash
# Listar tokens ativos
curl -H "Authorization: Bearer SEU_TOKEN" http://localhost:5000/swagger
```

### Erro: "Email já registrado"

**Solução**: Cada email e CPF são únicos no sistema. Use um email diferente:

```bash
curl -X POST http://localhost:5000/api/clientes \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer TOKEN" \
  -d '{
    "email": "novo.email@example.com",
    ...
  }'
```

### Erro 500: "Internal Server Error"

**Debug**:
1. Verifique os logs: `var/logs/cadastro-clientes.log`
2. Verifique a janela do terminal/console da aplicação
3. Ative logging detalhado em appsettings.Development.json:
   ```json
   "Logging": {
     "LogLevel": {
       "Default": "Debug",
       "Microsoft": "Information"
     }
   }
   ```

### Performance Lenta

**Otimizações**:
1. Ative caching: `services.AddMemoryCache()`
2. Use paginação: `?pagina=1&itensPorPagina=50`
3. Migre para SQL Server para produção
4. Adicione índices no banco de dados

---

## 📝 Changelog

### v1.0.0 (Atual)
- ✅ Autenticação JWT completa
- ✅ Gestão de clientes (CRUD)
- ✅ Análise de crédito
- ✅ RabbitMQ com fallback
- ✅ Swagger/OpenAPI
- ✅ SQLite com migrations automáticas
- ✅ Logging e cache
- ✅ Testes com XUnit
- ✅ Docker ready

---

## 🤝 Contribuindo

1. Faça um fork do projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

---

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo LICENSE para mais detalhes.

---

## 📞 Suporte

- **Issues**: GitHub Issues
- **Email**: desenvolvimento@example.com
- **Documentação**: Acesse `/swagger` na aplicação

---

## 🎓 Referências

- [.NET 8 Documentation](https://learn.microsoft.com/dotnet/)
- [ASP.NET Core 8](https://learn.microsoft.com/aspnet/core/)
- [Entity Framework Core](https://learn.microsoft.com/ef/core/)
- [JWT.io](https://jwt.io/)
- [RabbitMQ](https://www.rabbitmq.com/)

---

**Desenvolvido com ❤️ por Desenvolvimento Backend**

Última atualização: 2024-11-03
