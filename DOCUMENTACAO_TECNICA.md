# Documentação Técnica - Cadastro de Clientes API

## 📋 Índice

1. [Visão Geral](#visão-geral)
2. [Arquitetura](#arquitetura)
3. [Stack Tecnológica](#stack-tecnológica)
4. [Entidades e Modelo de Domínio](#entidades-e-modelo-de-domínio)
5. [Regras de Negócio](#regras-de-negócio)
6. [APIs e Endpoints](#apis-e-endpoints)
7. [Fluxos de Processo](#fluxos-de-processo)
8. [Integração e Mensageria](#integração-e-mensageria)
9. [Segurança](#segurança)
10. [Persistência de Dados](#persistência-de-dados)
11. [Padrões e Práticas](#padrões-e-práticas)
12. [Configurações](#configurações)

---

## 📊 Visão Geral

### Propósito do Sistema
A **Cadastro de Clientes API** é um microsserviço central responsável por:
- **Gestão completa de clientes** (CRUD operations)
- **Análise de crédito** e scoring automático
- **Ranking de crédito** com elegibilidade para cartões
- **Informações financeiras** detalhadas
- **Publicação de eventos** para downstream systems
- **Autenticação e autorização** com JWT

### Contexto de Negócio
O serviço atua como sistema **master** de dados de clientes:
1. Cadastro inicial de clientes com validações completas
2. Análise de crédito com score e ranking
3. Determinação de elegibilidade para cartões de crédito
4. Atualização de informações pessoais e financeiras
5. Publicação de eventos para sistemas downstream (Crédito, Cartão)
6. Soft delete com histórico completo

### Características Principais
- ✅ **Clean Architecture** com separação de camadas
- ✅ **Domain-Driven Design** com entidades ricas
- ✅ **CQRS Pattern** para leitura e escrita
- ✅ **Event-Driven** com RabbitMQ
- ✅ **Soft Delete** para auditoria
- ✅ **Paginação** eficiente
- ✅ **Validações** robustas (CPF, Email, CEP)
- ✅ **API-First** com OpenAPI/Swagger

---

## 🏗️ Arquitetura

### Diagrama de Camadas

```
┌───────────────────────────────────────────────────────────────┐
│                    Driving.Api Layer                          │
│  Controllers, Middleware, JWT Authentication                  │
│  - ClientesController: CRUD de clientes                       │
│  - AuthController: Login e autenticação                       │
└─────────────────────────────┬─────────────────────────────────┘
                              │
┌─────────────────────────────▼─────────────────────────────────┐
│                  Core.Application Layer                       │
│  Services, DTOs, Mappers, Validators                          │
│  - ClienteService: Gestão de clientes                         │
│  - AuthenticationService: JWT e autenticação                  │
│  - ClienteMapper: Conversão Domain ↔ DTO                      │
└─────────────────────────────┬─────────────────────────────────┘
                              │
┌─────────────────────────────▼─────────────────────────────────┐
│                     Core.Domain Layer                         │
│  Entities, Value Objects, Business Rules                      │
│  - Cliente: Dados pessoais e endereço                         │
│  - InformacoesFinanceirasCliente: Score, ranking, limites     │
│  - Usuario: Autenticação e autorização                        │
└─────────────────────────────┬─────────────────────────────────┘
                              │
        ┌─────────────────────┼─────────────────────┐
        │                     │                     │
┌───────▼───────┐  ┌──────────▼─────────┐  ┌───────▼──────────┐
│ Driven.SqlLite│  │  Core.Infra        │  │ Driven.RabbitMQ  │
│ Repositories  │  │  Cache             │  │ MessageBus       │
│ EF Core       │  │  Logging           │  │ Events           │
│ Migrations    │  │  Email             │  │ Publishers       │
└───────────────┘  └────────────────────┘  └──────────────────┘
```

### Fluxo de Dados

```
[Client/Frontend]
       ↓
[API Gateway] (futuro)
       ↓
[Driving.Api - Controllers]
       ↓
[Core.Application - Services]
       ↓
[Core.Domain - Business Rules]
       ↓
[Driven.SqlLite - Persistence]
       ↓
[Database - SQLite]

       ↓ (async events)
       
[Driven.RabbitMQ - Events]
       ↓
[RabbitMQ Broker]
       ↓
[Downstream Services: Crédito, Cartão]
```

---

## 🛠️ Stack Tecnológica

### Framework & Runtime
| Tecnologia | Versão | Propósito |
|-----------|--------|-----------|
| **.NET** | 8.0 | Runtime e Framework base |
| **ASP.NET Core** | 8.0 | Web API framework |
| **C#** | 12 | Linguagem de programação |

### Persistência
| Tecnologia | Versão | Propósito |
|-----------|--------|-----------|
| **Entity Framework Core** | 8.0 | ORM para acesso a dados |
| **SQLite** | 3.x | Banco de dados embarcado |
| **EF Core Migrations** | 8.0 | Versionamento de schema |
| **LINQ** | - | Consultas type-safe |

### Mensageria
| Tecnologia | Versão | Propósito |
|-----------|--------|-----------|
| **RabbitMQ** | 3.12+ | Message broker AMQP |
| **RabbitMQ.Client** | 6.x | Client library .NET |

### Segurança
| Tecnologia | Versão | Propósito |
|-----------|--------|-----------|
| **JWT Bearer** | - | Autenticação stateless |
| **BCrypt.Net** | - | Hashing de senhas |
| **Data Protection API** | - | Criptografia de dados sensíveis |

### Validação
| Tecnologia | Versão | Propósito |
|-----------|--------|-----------|
| **FluentValidation** | 11.x | Validação de DTOs |
| **Data Annotations** | - | Validações básicas |

### Observabilidade
| Tecnologia | Versão | Propósito |
|-----------|--------|-----------|
| **Serilog** | 3.x | Logging estruturado |
| **Serilog.Sinks.Console** | - | Output para console |
| **Serilog.Sinks.File** | - | Output para arquivos |

### Cache
| Tecnologia | Versão | Propósito |
|-----------|--------|-----------|
| **IMemoryCache** | - | Cache em memória |
| **Redis** (futuro) | - | Cache distribuído |

### Qualidade & Testes
| Tecnologia | Versão | Propósito |
|-----------|--------|-----------|
| **xUnit** | 2.5+ | Framework de testes |
| **Moq** | 4.x | Mocking library |
| **FluentAssertions** | 6.x | Assertions fluentes |
| **Bogus** | - | Geração de dados fake |

### Documentação
| Tecnologia | Versão | Propósito |
|-----------|--------|-----------|
| **Swashbuckle** | 6.5+ | Swagger/OpenAPI |
| **OpenAPI** | 3.0 | Especificação de API |

---

## 📦 Entidades e Modelo de Domínio

### 1. Cliente

**Responsabilidade**: Representa um cliente no sistema com dados pessoais

```csharp
public class Cliente : BaseEntity
{
    // Dados Pessoais
    public string Nome { get; private set; }              // Min: 3, Max: 150
    public string Email { get; private set; }             // Formato válido
    public string Telefone { get; private set; }          // 10-11 dígitos
    public string Cpf { get; private set; }               // 11 dígitos, validado
    
    // Endereço
    public string Endereco { get; private set; }          // Min: 5
    public string Cidade { get; private set; }            // Min: 2
    public string Estado { get; private set; }            // 2 caracteres (UF)
    public string Cep { get; private set; }               // 8 dígitos
    
    // Crédito (Desnormalizado para performance)
    public int RankingCredito { get; private set; }       // 0-5
    public int ScoreCredito { get; private set; }         // 0-1000
    public DateTime? DataAtualizacaoRanking { get; private set; }
    public bool AptoParaCartaoCredito { get; private set; }
}
```

**Constantes de Validação**
```csharp
NOME_MIN_LENGTH = 3
NOME_MAX_LENGTH = 150
CPF_LENGTH = 11
TELEFONE_MIN_LENGTH = 10
TELEFONE_MAX_LENGTH = 11
SCORE_MIN = 0
SCORE_MAX = 1000
RANKING_MIN = 0
RANKING_MAX = 5
RANKING_MINIMO_CARTAO = 3
SCORE_MINIMO_CARTAO = 600
```

**Factory Methods**
- `Cliente.Criar()`: Cria novo cliente com validações completas
- `Cliente.Atualizar()`: Atualiza dados pessoais
- `Cliente.AtualizarRanking()`: Atualiza score e ranking de crédito
- `Cliente.Desativar()`: Soft delete

**Validações de Domínio**
- CPF deve ter 11 dígitos numéricos
- Email deve ter formato válido
- Telefone deve ter 10-11 dígitos
- CEP deve ter 8 dígitos
- Estado deve ter 2 caracteres (UF)
- Score entre 0-1000
- Ranking entre 0-5

### 2. InformacoesFinanceirasCliente

**Responsabilidade**: Armazena dados financeiros e análise de crédito detalhada

```csharp
public class InformacoesFinanceirasCliente : BaseEntity
{
    // Identificação
    public Guid ClienteId { get; private set; }
    
    // Renda
    public decimal Renda { get; private set; }
    public decimal RendaComprovada { get; private set; }
    
    // Score e Ranking
    public int ScoreCredito { get; private set; }          // 0-1000
    public int RankingCredito { get; private set; }        // 0-5
    
    // Limites
    public decimal LimiteCreditoSugerido { get; private set; }
    public decimal LimiteCreditoAtivo { get; private set; }
    
    // Análise de Risco
    public decimal TotalDebitos { get; private set; }
    public int CreditosEmAberto { get; private set; }
    public int AtrasosDiversos12Meses { get; private set; }
    
    // Cartões
    public bool AptoParaCartaoCredito { get; private set; }
    public string CartoesEmitidos { get; private set; }    // CSV: "VISA,MC"
    
    // Datas
    public DateTime? DataUltimaAnalise { get; private set; }
    public DateTime? DataProximaAnaliseRecomendada { get; private set; }
    
    // Análise
    public string? MotivoRecusa { get; private set; }
    public string AnaliseRiscoCredito { get; private set; }
    public string RecomendacoesMelhoraScore { get; private set; }
}
```

**Cálculo de Ranking**
```csharp
Ranking 5 (Excelente): Score >= 900
Ranking 4 (Bom):       Score >= 750
Ranking 3 (Aceitável): Score >= 600
Ranking 2 (Ruim):      Score >= 400
Ranking 1 (Muito Ruim): Score < 400
Ranking 0: Sem avaliação
```

**Elegibilidade para Cartão**
```csharp
AptoParaCartaoCredito = (RankingCredito >= 3 && ScoreCredito >= 600)
```

### 3. Usuario

**Responsabilidade**: Autenticação e autorização no sistema

```csharp
public class Usuario : BaseEntity
{
    public string Nome { get; private set; }
    public string Email { get; private set; }
    public string SenhaHash { get; private set; }          // BCrypt hash
    public string Role { get; private set; }               // Admin, Operator
    public bool EmailConfirmado { get; private set; }
    public int TentativasLogin { get; private set; }
    public DateTime? DataBloqueio { get; private set; }
    public DateTime? UltimoLogin { get; private set; }
}
```

**Roles Disponíveis**
- `Admin`: Acesso total ao sistema
- `Operator`: Operações de CRUD de clientes
- `ReadOnly`: Apenas consulta

### 4. BaseEntity (Herança)

```csharp
public abstract class BaseEntity
{
    public Guid Id { get; protected set; }
    public DateTime DataCriacao { get; protected set; }
    public DateTime? DataAtualizacao { get; protected set; }
    public bool Ativo { get; protected set; }
}
```

---

## ⚖️ Regras de Negócio

### Cadastro de Clientes

#### RN-001: Validação de CPF
- **Regra**: CPF deve ser único no sistema
- **Validação**: 
  - 11 dígitos numéricos
  - Não pode ser sequência (111.111.111-11)
  - Dígitos verificadores válidos
- **Implementação**: `Cliente.ValidarCpf()`

#### RN-002: Validação de Email
- **Regra**: Email deve ser único e válido
- **Formato**: `usuario@dominio.com`
- **Implementação**: Regex + verificação de unicidade

#### RN-003: Validação de Telefone
- **Regra**: Telefone brasileiro válido
- **Formatos aceitos**:
  - Celular: 11 dígitos (11987654321)
  - Fixo: 10 dígitos (1134567890)
- **Implementação**: `Cliente.ValidarTelefone()`

#### RN-004: Validação de CEP
- **Regra**: CEP deve ter 8 dígitos
- **Formato**: 01234567 (sem hífen)
- **Integração futura**: ViaCEP para validação real

#### RN-005: Dados Obrigatórios
- **Campos obrigatórios**:
  - Nome (mínimo 3 caracteres)
  - Email
  - Telefone
  - CPF
  - Endereço
  - Cidade
  - Estado (UF)
  - CEP

### Análise de Crédito

#### RN-006: Cálculo de Score de Crédito
- **Regra**: Score baseado em múltiplos fatores
- **Faixa**: 0 a 1000 pontos
- **Fatores**:
  ```
  Base: 500 pontos
  + Renda comprovada > R$ 3.000: +100
  + Sem débitos: +100
  + Sem atrasos 12m: +150
  + Créditos zerados: +50
  - Débitos totais: -10 por R$ 1.000
  - Atrasos: -50 por atraso
  ```
- **Implementação**: `InformacoesFinanceirasCliente.CalcularScore()`

#### RN-007: Cálculo de Ranking
- **Regra**: Ranking derivado do score
- **Mapeamento**:
  ```csharp
  if (score >= 900) ranking = 5;      // Excelente
  else if (score >= 750) ranking = 4; // Bom
  else if (score >= 600) ranking = 3; // Aceitável
  else if (score >= 400) ranking = 2; // Ruim
  else ranking = 1;                   // Muito Ruim
  ```

#### RN-008: Elegibilidade para Cartão
- **Regra**: Cliente elegível se atender critérios mínimos
- **Critérios**:
  - Ranking >= 3 (Aceitável)
  - Score >= 600
  - Sem bloqueios ativos
- **Fórmula**: `AptoParaCartaoCredito = (Ranking >= 3 AND Score >= 600)`

#### RN-009: Limite de Crédito Sugerido
- **Regra**: Limite baseado em renda e score
- **Cálculo**:
  ```csharp
  Se Score >= 800:
    Limite = Renda * 3.0
  Se Score >= 600:
    Limite = Renda * 2.0
  Se Score >= 400:
    Limite = Renda * 1.0
  Senão:
    Limite = 0
  ```
- **Máximo**: R$ 50.000 por cartão
- **Mínimo**: R$ 500 (se aprovado)

#### RN-010: Periodicidade de Análise
- **Regra**: Reanálise periódica de crédito
- **Frequência**:
  - Score < 600: A cada 3 meses
  - Score >= 600: A cada 6 meses
  - Score >= 800: A cada 12 meses
- **Campo**: `DataProximaAnaliseRecomendada`

### Atualização de Dados

#### RN-011: Soft Delete
- **Regra**: Clientes não são removidos fisicamente
- **Implementação**:
  - `Ativo = false`
  - `DataAtualizacao = DateTime.UtcNow`
  - Mantém histórico completo
- **Reativação**: Possível via suporte

#### RN-012: Campos Imutáveis
- **Regra**: Alguns campos não podem ser alterados após criação
- **Campos imutáveis**:
  - CPF (identificador único)
  - Data de Criação
  - ID
- **Justificativa**: Integridade e auditoria

#### RN-013: Histórico de Alterações
- **Regra**: Toda alteração atualiza `DataAtualizacao`
- **Implementação**: Interceptor do EF Core
- **Log**: Todas as mudanças são logadas

### Publicação de Eventos

#### RN-014: Eventos de Cliente
- **Regra**: Operações críticas geram eventos
- **Eventos**:
  - `cliente.criado`: Novo cliente cadastrado
  - `cliente.atualizado`: Dados alterados
  - `cliente.deletado`: Soft delete executado
  - `cliente.ranking.atualizado`: Score/ranking mudou
- **Destino**: RabbitMQ para consumo downstream

#### RN-015: Evento de Ranking Atualizado
- **Regra**: Mudança de elegibilidade notifica sistema de Crédito
- **Trigger**: Quando `AptoParaCartaoCredito` muda de false → true
- **Payload**: ID, Score, Ranking, Limite sugerido
- **Consumidores**: Serviço de Crédito, Serviço de Notificação

---

## 🌐 APIs e Endpoints

### Base URL
```
http://localhost:5000/api
```

### Autenticação

#### POST /auth/login
Autentica usuário e retorna token JWT

**Request**
```json
{
  "email": "admin@sistema.com",
  "password": "Admin@123"
}
```

**Response 200 OK**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "type": "Bearer",
  "expiresIn": 3600,
  "usuario": {
    "id": "guid",
    "nome": "Admin Sistema",
    "email": "admin@sistema.com",
    "role": "Admin"
  }
}
```

**Response 401 Unauthorized**
```json
{
  "sucesso": false,
  "mensagem": "Credenciais inválidas"
}
```

### Clientes - CRUD Operations

#### GET /clientes
Lista todos os clientes com paginação

**Headers**
```
Authorization: Bearer {token}
```

**Query Parameters**
- `pagina`: Número da página (default: 1)
- `itensPorPagina`: Itens por página (default: 10, max: 100)

**Response 200 OK**
```json
{
  "sucesso": true,
  "mensagem": "Clientes listados com sucesso",
  "dados": {
    "itens": [
      {
        "id": "guid",
        "nome": "João Silva",
        "email": "joao@email.com",
        "cpf": "12345678901",
        "telefone": "11987654321",
        "cidade": "São Paulo",
        "estado": "SP",
        "rankingCredito": 4,
        "scoreCredito": 780,
        "aptoParaCartaoCredito": true
      }
    ],
    "paginaAtual": 1,
    "itensPorPagina": 10,
    "totalItens": 45,
    "totalPaginas": 5
  }
}
```

#### GET /clientes/{id}
Obtém cliente por ID

**Response 200 OK**
```json
{
  "sucesso": true,
  "mensagem": "Cliente obtido com sucesso",
  "dados": {
    "id": "guid",
    "nome": "João Silva",
    "email": "joao@email.com",
    "telefone": "11987654321",
    "cpf": "12345678901",
    "endereco": "Rua Exemplo, 123",
    "cidade": "São Paulo",
    "estado": "SP",
    "cep": "01234567",
    "rankingCredito": 4,
    "scoreCredito": 780,
    "aptoParaCartaoCredito": true,
    "dataAtualizacaoRanking": "2024-11-01T10:00:00Z",
    "informacoesFinanceiras": {
      "renda": 5000.00,
      "rendaComprovada": 4500.00,
      "limiteCreditoSugerido": 9000.00,
      "totalDebitos": 1200.00,
      "cartoesEmitidos": "VISA"
    }
  }
}
```

**Response 404 Not Found**
```json
{
  "sucesso": false,
  "mensagem": "Cliente não encontrado",
  "erros": ["Nenhum cliente encontrado com o ID: {id}"]
}
```

#### GET /clientes/pesquisar?nome={nome}
Pesquisa clientes por nome

**Query Parameters**
- `nome`: Nome ou parte do nome (obrigatório)
- `pagina`: Número da página (default: 1)
- `itensPorPagina`: Itens por página (default: 10)

**Response 200 OK**
```json
{
  "sucesso": true,
  "mensagem": "Pesquisa realizada com sucesso",
  "dados": {
    "itens": [...],
    "paginaAtual": 1,
    "totalItens": 3
  }
}
```

#### POST /clientes
Cria novo cliente

**Request**
```json
{
  "nome": "Maria Santos",
  "email": "maria@email.com",
  "telefone": "11987654321",
  "cpf": "98765432100",
  "endereco": "Av. Paulista, 1000",
  "cidade": "São Paulo",
  "estado": "SP",
  "cep": "01310100",
  "informacoesFinanceiras": {
    "renda": 6000.00,
    "rendaComprovada": 6000.00
  }
}
```

**Response 201 Created**
```json
{
  "sucesso": true,
  "mensagem": "Cliente criado com sucesso",
  "dados": {
    "id": "new-guid",
    "nome": "Maria Santos",
    ...
  }
}
```

**Response 400 Bad Request**
```json
{
  "sucesso": false,
  "mensagem": "Dados inválidos",
  "erros": [
    "CPF já cadastrado no sistema",
    "Email inválido"
  ]
}
```

#### PUT /clientes/{id}
Atualiza cliente existente

**Request**
```json
{
  "nome": "Maria Santos Silva",
  "email": "maria.silva@email.com",
  "telefone": "11999887766",
  "endereco": "Av. Paulista, 1000 - Apto 101",
  "cidade": "São Paulo",
  "estado": "SP",
  "cep": "01310100"
}
```

**Response 200 OK**
```json
{
  "sucesso": true,
  "mensagem": "Cliente atualizado com sucesso",
  "dados": { ... }
}
```

#### DELETE /clientes/{id}
Remove cliente (soft delete)

**Response 200 OK**
```json
{
  "sucesso": true,
  "mensagem": "Cliente deletado com sucesso"
}
```

---

## 🔄 Fluxos de Processo

### Fluxo 1: Criação de Cliente

```
[Frontend] → POST /clientes
       ↓
[ClientesController.Criar()]
       ↓
[ModelState.IsValid?]
       ↓ Sim
[ClienteService.CriarAsync()]
       ↓
[Validações de Negócio]
       ↓
[CPF já existe?] → Sim → [Retorna 400]
       ↓ Não
[Email já existe?] → Sim → [Retorna 400]
       ↓ Não
[Cliente.Criar()]
       ↓
[InformacoesFinanceirasCliente.Criar()]
       ↓
[CalcularScore() e CalcularRanking()]
       ↓
[ClienteRepository.AdicionarAsync()]
       ↓
[Begin Transaction]
       ↓
[SaveChanges()]
       ↓
[PublicarEvento("cliente.criado")]
       ↓
[Commit Transaction]
       ↓
[Retorna 201 Created]
```

### Fluxo 2: Atualização de Ranking de Crédito

```
[Serviço de Crédito] → Pub "credito.avaliado"
       ↓
[RabbitMQ Subscriber]
       ↓
[CreditoAvaliadoHandler]
       ↓
[ClienteRepository.ObterPorIdAsync()]
       ↓
[Cliente.AtualizarRanking(score, ranking)]
       ↓
[CalcularElegibilidadeCartao()]
       ↓
[AptoParaCartaoCredito mudou?]
       ↓ Sim
[PublicarEvento("cliente.elegivel.cartao")]
       ↓
[SaveChanges()]
       ↓
[Log alteração]
```

### Fluxo 3: Pesquisa de Clientes

```
[Frontend] → GET /clientes/pesquisar?nome=João
       ↓
[ClientesController.Pesquisar()]
       ↓
[ClienteService.PesquisarPorNomeAsync()]
       ↓
[ClienteRepository.PesquisarPorNomeAsync()]
       ↓
[LINQ Query com LIKE '%João%']
       ↓
[Aplicar paginação]
       ↓
[Projetar para ClienteListDto]
       ↓
[Retornar PaginatedResponseDto]
       ↓
[200 OK com resultados]
```

---

## 📨 Integração e Mensageria

### RabbitMQ

**Configuração**
```json
{
  "RabbitMQ": {
    "Host": "localhost",
    "Port": 5672,
    "VirtualHost": "/",
    "Username": "guest",
    "Password": "guest"
  }
}
```

**Exchanges e Filas**

| Exchange | Tipo | Routing Key | Fila | Consumer |
|----------|------|-------------|------|----------|
| `cliente-events` | Topic | `cliente.criado` | `cliente-criado-queue` | Crédito, Email |
| `cliente-events` | Topic | `cliente.atualizado` | `cliente-atualizado-queue` | Auditoria |
| `cliente-events` | Topic | `cliente.elegivel.cartao` | `cliente-elegivel-queue` | Cartão |
| `credito-events` | Topic | `credito.avaliado` | `credito-avaliado-queue` | Cliente (este) |

**Evento Publicado: cliente.criado**
```json
{
  "eventId": "guid",
  "eventType": "cliente.criado",
  "timestamp": "2024-11-03T10:00:00Z",
  "data": {
    "clienteId": "guid",
    "nome": "João Silva",
    "email": "joao@email.com",
    "cpf": "12345678901",
    "telefone": "11987654321",
    "endereco": {
      "logradouro": "Rua Exemplo, 123",
      "cidade": "São Paulo",
      "estado": "SP",
      "cep": "01234567"
    }
  }
}
```

**Evento Publicado: cliente.elegivel.cartao**
```json
{
  "eventId": "guid",
  "eventType": "cliente.elegivel.cartao",
  "timestamp": "2024-11-03T10:30:00Z",
  "data": {
    "clienteId": "guid",
    "scoreCredito": 780,
    "rankingCredito": 4,
    "limiteCreditoSugerido": 9000.00,
    "aptoParaCartaoCredito": true
  }
}
```

**Evento Consumido: credito.avaliado**
```json
{
  "eventId": "guid",
  "eventType": "credito.avaliado",
  "timestamp": "2024-11-03T10:25:00Z",
  "data": {
    "clienteId": "guid",
    "scoreCredito": 780,
    "rankingCredito": 4,
    "limiteSugerido": 9000.00,
    "analise": {
      "renda": 6000.00,
      "debitos": 1200.00,
      "atrasos": 0
    }
  }
}
```

---

## 🔒 Segurança

### Autenticação JWT

**Configuração**
```json
{
  "Jwt": {
    "Secret": "chave-secreta-minimo-32-caracteres",
    "Issuer": "CadastroClientesApi",
    "Audience": "CadastroClientesApp",
    "ExpirationMinutes": 60
  }
}
```

**Claims no Token**
```json
{
  "sub": "user-guid",
  "email": "admin@sistema.com",
  "name": "Admin Sistema",
  "role": "Admin",
  "iat": 1699012800,
  "exp": 1699016400
}
```

**Proteção de Endpoints**
```csharp
[Authorize]  // Todos os métodos do controller
[Authorize(Roles = "Admin")]  // Apenas admins
```

### Hashing de Senhas

**BCrypt**
- Work factor: 12
- Salt automático
- Resistente a rainbow tables
- One-way hash (irreversível)

```csharp
// Hash
string hash = BCrypt.Net.BCrypt.HashPassword(senha, 12);

// Verificação
bool valido = BCrypt.Net.BCrypt.Verify(senha, hash);
```

### Proteção de Dados Sensíveis

**CPF Mascarado em Logs**
```csharp
// Log: 123.***.***-01
string cpfMascarado = MascararCpf(cpf);
```

**Email Mascarado**
```csharp
// Log: jo***@email.com
string emailMascarado = MascararEmail(email);
```

### Tentativas de Login

**RN-016: Bloqueio por Tentativas**
- Máximo: 5 tentativas
- Bloqueio: 30 minutos
- Reset: Após login bem-sucedido

```csharp
if (usuario.TentativasLogin >= 5)
{
    if (usuario.DataBloqueio > DateTime.UtcNow.AddMinutes(-30))
        throw new Exception("Usuário bloqueado");
}
```

---

## 💾 Persistência de Dados

### Schema do Banco de Dados

**Tabela: Clientes**
```sql
CREATE TABLE Clientes (
    Id TEXT PRIMARY KEY,
    Nome TEXT NOT NULL,
    Email TEXT NOT NULL UNIQUE,
    Telefone TEXT NOT NULL,
    Cpf TEXT NOT NULL UNIQUE,
    Endereco TEXT NOT NULL,
    Cidade TEXT NOT NULL,
    Estado TEXT NOT NULL,
    Cep TEXT NOT NULL,
    RankingCredito INTEGER DEFAULT 0,
    ScoreCredito INTEGER DEFAULT 0,
    DataAtualizacaoRanking TEXT,
    AptoParaCartaoCredito INTEGER DEFAULT 0,
    DataCriacao TEXT NOT NULL,
    DataAtualizacao TEXT,
    Ativo INTEGER NOT NULL DEFAULT 1,
    
    CONSTRAINT CK_Clientes_Ranking CHECK (RankingCredito BETWEEN 0 AND 5),
    CONSTRAINT CK_Clientes_Score CHECK (ScoreCredito BETWEEN 0 AND 1000)
);

CREATE INDEX IX_Clientes_Cpf ON Clientes(Cpf);
CREATE INDEX IX_Clientes_Email ON Clientes(Email);
CREATE INDEX IX_Clientes_Nome ON Clientes(Nome);
CREATE INDEX IX_Clientes_RankingCredito ON Clientes(RankingCredito);
```

**Tabela: InformacoesFinanceirasCliente**
```sql
CREATE TABLE InformacoesFinanceirasCliente (
    Id TEXT PRIMARY KEY,
    ClienteId TEXT NOT NULL,
    Renda REAL DEFAULT 0,
    RendaComprovada REAL DEFAULT 0,
    ScoreCredito INTEGER DEFAULT 0,
    RankingCredito INTEGER DEFAULT 0,
    LimiteCreditoSugerido REAL DEFAULT 0,
    LimiteCreditoAtivo REAL DEFAULT 0,
    TotalDebitos REAL DEFAULT 0,
    CreditosEmAberto INTEGER DEFAULT 0,
    AtrasosDiversos12Meses INTEGER DEFAULT 0,
    AptoParaCartaoCredito INTEGER DEFAULT 0,
    CartoesEmitidos TEXT,
    DataUltimaAnalise TEXT,
    DataProximaAnaliseRecomendada TEXT,
    MotivoRecusa TEXT,
    AnaliseRiscoCredito TEXT,
    RecomendacoesMelhoraScore TEXT,
    DataCriacao TEXT NOT NULL,
    DataAtualizacao TEXT,
    Ativo INTEGER NOT NULL DEFAULT 1,
    
    FOREIGN KEY (ClienteId) REFERENCES Clientes(Id)
);

CREATE INDEX IX_InfFinanceiras_ClienteId ON InformacoesFinanceirasCliente(ClienteId);
CREATE INDEX IX_InfFinanceiras_Score ON InformacoesFinanceirasCliente(ScoreCredito);
```

**Tabela: Usuarios**
```sql
CREATE TABLE Usuarios (
    Id TEXT PRIMARY KEY,
    Nome TEXT NOT NULL,
    Email TEXT NOT NULL UNIQUE,
    SenhaHash TEXT NOT NULL,
    Role TEXT NOT NULL DEFAULT 'Operator',
    EmailConfirmado INTEGER DEFAULT 0,
    TentativasLogin INTEGER DEFAULT 0,
    DataBloqueio TEXT,
    UltimoLogin TEXT,
    DataCriacao TEXT NOT NULL,
    DataAtualizacao TEXT,
    Ativo INTEGER NOT NULL DEFAULT 1
);

CREATE INDEX IX_Usuarios_Email ON Usuarios(Email);
```

### Migrations

**Lista de Migrations**
1. `20250101000000_InitialCreate.cs`: Tabela Clientes
2. `20250101000001_AddInformacoesFinanceiras.cs`: Tabela InformacoesFinanceirasCliente
3. `20250101000002_AddUsuarios.cs`: Tabela Usuarios

---

## 📐 Padrões e Práticas

### Design Patterns

#### Repository Pattern
```csharp
public interface IClienteRepository
{
    Task<ClienteResponseDto?> ObterPorIdAsync(Guid id);
    Task<PaginatedResponseDto<ClienteListDto>> ListarAsync(int pagina, int itensPorPagina);
    Task AdicionarAsync(Cliente cliente);
    Task AtualizarAsync(Cliente cliente);
}
```

#### Service Pattern
```csharp
public interface IClienteService
{
    Task<ApiResponseDto<ClienteResponseDto>> ObterPorIdAsync(Guid id);
    Task<ApiResponseDto<ClienteResponseDto>> CriarAsync(ClienteCreateDto dto);
    Task<ApiResponseDto<ClienteResponseDto>> AtualizarAsync(ClienteUpdateDto dto);
}
```

#### DTO Pattern
- Separação entre domínio e API
- DTOs específicos: Create, Update, Response, List
- AutoMapper para conversões

### Princípios SOLID

✅ **Single Responsibility**: Cada classe tem uma responsabilidade
✅ **Open/Closed**: Extensível via interfaces
✅ **Liskov Substitution**: Herança apropriada
✅ **Interface Segregation**: Interfaces específicas
✅ **Dependency Inversion**: Dependência de abstrações

---

## ⚙️ Configurações

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=cadastro_clientes.db;"
  },
  "Jwt": {
    "Secret": "sua_chave_super_secreta_com_minimo_32_caracteres_para_producao",
    "Issuer": "CadastroClientesApi",
    "Audience": "CadastroClientesApp",
    "ExpirationMinutes": 60
  },
  "RabbitMQ": {
    "Host": "localhost",
    "Port": 5672,
    "VirtualHost": "/",
    "Username": "guest",
    "Password": "guest",
    "Enabled": true
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

---

**Última Atualização**: 03 de Novembro de 2024
**Versão**: 1.0.0
**Mantenedor**: Equipe de Desenvolvimento Backend
