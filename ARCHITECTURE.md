# Arquitetura do Sistema - Cadastro de Clientes API

## 📐 Visão Geral da Arquitetura

O sistema segue a **Arquitetura em Camadas (Layered Architecture)** com forte influência de **Domain-Driven Design (DDD)**, garantindo separação clara de responsabilidades, testabilidade e manutenibilidade.

```
┌─────────────────────────────────────────────────────────────┐
│                    Camada de Apresentação                   │
│           (Driving.Api - ASP.NET Core Controllers)          │
│  HTTP Requests → Swagger/OpenAPI → Response DTOs            │
└──────────────────────┬──────────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────────┐
│                Camada de Aplicação                           │
│        (Core.Application - Services & Use Cases)            │
│  Orquestração → Validação → DTOs → Mappers → Respostas     │
└──────────────────────┬──────────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────────┐
│                 Camada de Domínio                            │
│          (Core.Domain - Entidades & Lógica)                 │
│  Entidades Ricas → Validações → Comportamento Negócio      │
└──────────────────────┬──────────────────────────────────────┘
                       │
        ┌──────────────┼──────────────┐
        │              │              │
┌───────▼────┐  ┌──────▼──────┐  ┌───▼──────────┐
│ Core.Infra │  │   Driven.*  │  │  Interfaces  │
│(CrossCutting)│ │(Adapters)  │  │  (Contracts) │
└────────────┘  └─────────────┘  └──────────────┘
```

---

## 🏛️ Arquitetura Detalhada por Camada

### 1. Camada de Apresentação (Driving.Api)

**Responsabilidades**:
- Receber requisições HTTP
- Validar e desserializar dados
- Chamar serviços de aplicação
- Serializar e retornar respostas
- Documentar endpoints (Swagger)

**Estrutura**:
```
Driving.Api/
├── Controllers/
│   ├── AuthController.cs        # Endpoints de autenticação
│   └── ClientesController.cs    # Endpoints de clientes
├── Program.cs                   # Configuração da aplicação
├── appsettings.json
└── appsettings.Development.json
```

**Exemplo de Endpoint**:
```csharp
[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly IClienteService _service;

    [HttpPost]
    [Authorize]  // Requer JWT
    public async Task<ActionResult<ApiResponseDto<ClienteResponseDto>>>
        Criar([FromBody] ClienteCreateDto dto)
    {
        var resultado = await _service.CriarAsync(dto);

        if (!resultado.Sucesso)
            return BadRequest(resultado);

        return CreatedAtAction(nameof(ObterPorId),
            new { id = resultado.Dados.Id }, resultado);
    }
}
```

**Padrões**:
- RESTful API com HTTP verbs corretos
- Versionamento se necessário
- Consistent error handling
- Response wrappers (ApiResponseDto)

---

### 2. Camada de Aplicação (Core.Application)

**Responsabilidades**:
- Implementar casos de uso (use cases)
- Orquestrar fluxos de negócio
- Validar dados de entrada
- Mapear entre entidades e DTOs
- Chamar repositórios e serviços de infraestrutura

**Estrutura**:
```
Core.Application/
├── Services/
│   ├── ClienteService.cs          # Casos de uso de clientes
│   └── AuthenticationService.cs   # Autenticação
├── DTOs/
│   ├── Request/
│   │   ├── ClienteCreateDto.cs
│   │   └── ClienteUpdateDto.cs
│   └── Response/
│       ├── ClienteResponseDto.cs
│       └── ApiResponseDto.cs
├── Mappers/
│   └── ClienteMappers.cs          # Mapeamento DTO <-> Entidade
├── Validators/
│   └── ClienteValidator.cs        # Validações específicas
├── Interfaces/
│   ├── IClienteService.cs
│   └── IAuthenticationService.cs
└── Core.Application.DependencyInjection.cs
```

**Exemplo de Serviço**:
```csharp
public class ClienteService : IClienteService
{
    private readonly IClienteRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<ClienteService> _logger;

    public async Task<ApiResponseDto<ClienteResponseDto>> CriarAsync(
        ClienteCreateDto dto)
    {
        try
        {
            // 1. Validar DTO
            var erros = ValidarClienteCreateDto(dto);
            if (erros.Count > 0)
            {
                return new ApiResponseDto<ClienteResponseDto>
                {
                    Sucesso = false,
                    Erros = erros
                };
            }

            // 2. Verificar duplicatas
            if (await _repository.EmailJaRegistradoAsync(dto.Email))
                return new ApiResponseDto<ClienteResponseDto>
                {
                    Sucesso = false,
                    Mensagem = "Email já registrado"
                };

            // 3. Chamar repositório
            var cliente = await _repository.CriarAsync(dto);

            // 4. Mapear para DTO de resposta
            var response = _mapper.Map<ClienteResponseDto>(cliente);

            // 5. Retornar sucesso
            return new ApiResponseDto<ClienteResponseDto>
            {
                Sucesso = true,
                Mensagem = "Cliente criado com sucesso",
                Dados = response
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar cliente");
            return new ApiResponseDto<ClienteResponseDto>
            {
                Sucesso = false,
                Mensagem = "Erro ao criar cliente",
                Erros = new List<string> { ex.Message }
            };
        }
    }
}
```

**DTOs (Data Transfer Objects)**:
```csharp
// Request
public class ClienteCreateDto
{
    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(150, MinimumLength = 3)]
    public string Nome { get; set; }

    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress]
    public string Email { get; set; }
    // ... outros campos
}

// Response
public class ClienteResponseDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public string Email { get; set; }
    public int RankingCredito { get; set; }
    public int ScoreCredito { get; set; }
    public bool AptoParaCartaoCredito { get; set; }
}

// Padrão de resposta
public class ApiResponseDto<T>
{
    public bool Sucesso { get; set; }
    public string Mensagem { get; set; }
    public T Dados { get; set; }
    public List<string> Erros { get; set; } = new();
}
```

---

### 3. Camada de Domínio (Core.Domain)

**Responsabilidades**:
- Definir entidades ricas com comportamento
- Implementar validações de negócio
- Encapsular lógica de domínio
- Manter invariantes do domínio

**Estrutura**:
```
Core.Domain/
├── Entities/
│   ├── Cliente.cs                          # Entidade de cliente
│   ├── Usuario.cs                          # Entidade de usuário
│   └── InformacoesFinanceirasCliente.cs   # Entidade de dados financeiros
├── Common/
│   └── BaseEntity.cs                       # Classe base para entidades
└── Interfaces/
    ├── IClienteRepository.cs               # Contrato de repositório
    └── IMessageBus.cs                      # Contrato de mensageria
```

**Entidades Ricas (Domain-Driven Design)**:
```csharp
public class Cliente : BaseEntity
{
    // Propriedades (imutáveis, somente getters)
    public string Nome { get; private set; }
    public string Email { get; private set; }
    public int RankingCredito { get; private set; }
    public int ScoreCredito { get; private set; }
    public bool AptoParaCartaoCredito { get; private set; }

    // Construtor privado
    private Cliente() { }

    // Factory method (padrão)
    public static Cliente Criar(string nome, string email, ...)
    {
        // Validações de domínio
        ValidarDados(nome, email, ...);

        return new Cliente
        {
            Id = Guid.NewGuid(),
            Nome = nome,
            Email = email,
            DataCriacao = DateTime.UtcNow,
            Ativo = true
        };
    }

    // Comportamento rico
    public void AtualizarRankingCredito(int ranking, int score)
    {
        if (ranking < 0 || ranking > 5)
            throw new ArgumentException("Ranking inválido");

        RankingCredito = ranking;
        ScoreCredito = score;

        // Lógica de negócio: determinar aptidão automática
        AptoParaCartaoCredito = ranking >= 3 && score >= 600;

        MarcarComoAtualizada();
    }

    public bool PodeEmitirCartaoCredito()
    {
        return AptoParaCartaoCredito && Ativo &&
               RankingCredito >= 3 && ScoreCredito >= 600;
    }

    // Validações (privadas)
    private static void ValidarDados(string nome, string email, ...)
    {
        var erros = new List<string>();

        if (string.IsNullOrWhiteSpace(nome))
            erros.Add("Nome é obrigatório");
        else if (nome.Length < 3)
            erros.Add("Nome deve ter no mínimo 3 caracteres");

        // ... mais validações

        if (erros.Any())
            throw new ArgumentException(string.Join("; ", erros));
    }
}
```

**BaseEntity**:
```csharp
public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public DateTime DataAtualizacao { get; set; } = DateTime.UtcNow;
    public bool Ativo { get; set; } = true;
    public string AtualizadoPor { get; set; }

    protected void MarcarComoAtualizada(string atualizadoPor = null)
    {
        DataAtualizacao = DateTime.UtcNow;
        AtualizadoPor = atualizadoPor;
    }
}
```

**Interfaces de Domínio**:
```csharp
// Contrato para repositório (implementado na camada de dados)
public interface IClienteRepository
{
    Task<Cliente> ObterPorIdAsync(Guid id);
    Task<List<Cliente>> ListarAsync(int pagina, int itensPorPagina);
    Task<Cliente> CriarAsync(ClienteCreateDto dto);
    Task<Cliente> AtualizarAsync(ClienteUpdateDto dto);
    Task<bool> DeletarAsync(Guid id);
    Task<bool> EmailJaRegistradoAsync(string email, Guid? exceptId = null);
    Task<bool> ExisteAsync(Guid id);
}

// Contrato para mensageria
public interface IMessageBus
{
    Task PublicarEventoAsync<T>(T evento) where T : IEvent;
    bool TryConnect();
}
```

---

### 4. Camada de Infraestrutura (Core.Infra)

**Responsabilidades**:
- Implementar cross-cutting concerns
- Logging, caching, email
- Notificações
- Extensões e utilitários

**Estrutura**:
```
Core.Infra/
├── Logging/
│   └── LoggerService.cs              # Serviço de logging
├── Email/
│   └── EmailService.cs               # Envio de emails SMTP
├── Caching/
│   └── CacheService.cs               # Cache em memória
├── Repositories/
│   └── GenericRepository.cs          # Repositório base genérico
├── Extensions/
│   └── LoggerExtensions.cs           # Métodos de logging
└── Core.Infra.DependencyInjection.cs
```

**Exemplo - Serviço de Email**:
```csharp
public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly EmailSettings _settings;

    public async Task EnviarEmailAsync(string destinatario,
        string assunto, string corpo)
    {
        try
        {
            using var client = new SmtpClient(_settings.SmtpServer,
                _settings.Port);

            client.EnableSsl = _settings.EnableSSL;
            client.Credentials = new NetworkCredential(
                _settings.Username, _settings.Password);

            var mailMessage = new MailMessage(
                _settings.FromAddress,
                destinatario)
            {
                Subject = assunto,
                Body = corpo,
                IsBodyHtml = true
            };

            await client.SendMailAsync(mailMessage);

            _logger.LogInformation(
                "Email enviado para {Destinatario}", destinatario);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Erro ao enviar email para {Destinatario}", destinatario);
            throw;
        }
    }
}
```

---

### 5. Camada de Driven (Implementação de Interfaces)

#### 5.1 Driven.SqlLite (Persistência)

**Responsabilidades**:
- Implementar repositórios
- Configurar Entity Framework Core
- Gerenciar migrations
- Executar queries

**Estrutura**:
```
Driven.SqlLite/
├── Data/
│   └── ApplicationDbContext.cs       # DbContext do EF Core
├── Repositories/
│   ├── ClienteRepository.cs          # Implementação para Cliente
│   └── UsuarioRepository.cs          # Implementação para Usuario
├── Migrations/
│   ├── 20240101000000_InitialCreate.cs
│   └── ...
└── Driven.SqlLite.DependencyInjection.cs
```

**DbContext**:
```csharp
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuração de Cliente
        modelBuilder.Entity<Cliente>(builder =>
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Nome).HasMaxLength(150).IsRequired();
            builder.Property(c => c.Email).HasMaxLength(150).IsRequired();
            builder.HasIndex(c => c.Email).IsUnique();
            builder.HasIndex(c => c.Cpf).IsUnique();
        });

        // Configuração de Usuario
        modelBuilder.Entity<Usuario>(builder =>
        {
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Login).HasMaxLength(50).IsRequired();
            builder.HasIndex(u => u.Login).IsUnique();
            builder.HasIndex(u => u.Email).IsUnique();
        });
    }
}
```

**Repository Pattern**:
```csharp
public class ClienteRepository : IClienteRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public ClienteRepository(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Cliente> ObterPorIdAsync(Guid id)
    {
        return await _context.Clientes.FindAsync(id);
    }

    public async Task<bool> EmailJaRegistradoAsync(string email, Guid? exceptId = null)
    {
        return await _context.Clientes
            .Where(c => c.Email == email && (exceptId == null || c.Id != exceptId))
            .AnyAsync();
    }

    public async Task<Cliente> CriarAsync(ClienteCreateDto dto)
    {
        var cliente = Cliente.Criar(
            dto.Nome, dto.Email, dto.Telefone,
            dto.Cpf, dto.Endereco, dto.Cidade,
            dto.Estado, dto.Cep);

        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();

        return cliente;
    }
}
```

#### 5.2 Driven.RabbitMQ (Mensageria)

**Responsabilidades**:
- Implementar produtor/consumidor
- Publicar eventos
- Consumir mensagens
- Gerenciar conexões

**Estrutura**:
```
Driven.RabbitMQ/
├── Services/
│   └── RabbitMQService.cs           # Implementação de IMessageBus
├── Interfaces/
│   └── IMessageBus.cs               # Contrato
├── Events/
│   ├── ClienteCriadoEvent.cs
│   └── ClienteAtualizadoEvent.cs
├── Settings/
│   └── RabbitMQSettings.cs
└── Driven.RabbitMQ.DependencyInjection.cs
```

**Eventos de Domínio**:
```csharp
public interface IEvent
{
    Guid AggregateId { get; }
    DateTime OcorridoEm { get; }
}

public class ClienteCriadoEvent : IEvent
{
    public Guid ClienteId { get; set; }
    public Guid AggregateId => ClienteId;
    public string Nome { get; set; }
    public string Email { get; set; }
    public DateTime OcorridoEm { get; set; } = DateTime.UtcNow;
}
```

**RabbitMQ Service**:
```csharp
public class RabbitMQService : IMessageBus
{
    private IConnection _connection;
    private IModel _channel;

    public async Task PublicarEventoAsync<T>(T evento) where T : IEvent
    {
        try
        {
            var json = JsonSerializer.Serialize(evento);
            var body = Encoding.UTF8.GetBytes(json);

            _channel.BasicPublish(
                exchange: "eventos",
                routingKey: typeof(T).Name,
                basicProperties: null,
                body: body);

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao publicar evento");
        }
    }

    public bool TryConnect()
    {
        try
        {
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
```

---

## 🔄 Fluxos de Requisição Principais

### 1. Criar Cliente

```
1. POST /api/clientes (ClientesController.Criar)
   ↓
2. ValidarAutenticacao (JWT)
   ↓
3. ClienteService.CriarAsync()
   ├─ Validar ClienteCreateDto
   ├─ Verificar email duplicado
   ├─ Verificar CPF duplicado
   └─ Chamar IClienteRepository.CriarAsync()
      ↓
4. ClienteRepository.CriarAsync()
   ├─ Cliente.Criar() [validação de domínio]
   ├─ DbContext.Clientes.Add()
   └─ SaveChangesAsync()
      ↓
5. Publicar ClienteCriadoEvent via IMessageBus
   ↓
6. MapClienteParaResponseDto()
   ↓
7. Retornar ApiResponseDto<ClienteResponseDto>
```

### 2. Login de Usuário

```
1. POST /api/auth/login (AuthController.Login)
   ↓
2. AuthenticationService.LoginAsync()
   ├─ Validar credenciais
   ├─ IUsuarioRepository.ObterPorLoginAsync()
   ├─ Usuario.VerificarSenha()
   ├─ Usuario.RegistrarAcessoSucesso()
   ├─ Gerar JWT Token
   └─ Retornar token
      ↓
3. MapClienteParaResponseDto()
   ↓
4. Retornar ApiResponseDto com token
```

---

## 💉 Injeção de Dependências

**Program.cs**:
```csharp
// Serviços de Aplicação
builder.Services.AddApplicationServices(jwtSecret, issuer, audience, expiration);

// Banco de Dados
builder.Services.AddSqlLiteDatabase(connectionString);

// Infraestrutura
builder.Services.AddInfrastructureServices(configuration);

// Mensageria
builder.Services.AddRabbitMQServices(configuration);
```

**Core.Application.DependencyInjection.cs**:
```csharp
public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        string jwtSecret,
        string issuer,
        string audience,
        int expirationMinutes)
    {
        services.AddScoped<IClienteService, ClienteService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddAutoMapper(typeof(ClienteMappers));

        // JWT Configuration
        services.Configure<JwtSettings>(options =>
        {
            options.Secret = jwtSecret;
            options.Issuer = issuer;
            options.Audience = audience;
            options.ExpirationMinutes = expirationMinutes;
        });

        return services;
    }
}
```

---

## 🔒 Padrões de Segurança

### 1. Autenticação JWT

```csharp
[Authorize]  // Requer token JWT válido
[HttpGet]
public async Task<IActionResult> ListarClientes()
{
    var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    // ... resto do código
}
```

### 2. Hashing de Senha

```csharp
// PBKDF2 com SHA256
private static (string salt, string hash) GerarHashSenha(string senhaPlana)
{
    byte[] saltBytes = new byte[32];
    using (var rng = RandomNumberGenerator.Create())
    {
        rng.GetBytes(saltBytes);
    }

    string saltString = Convert.ToBase64String(saltBytes);
    string hashString = GerarHashComSalt(senhaPlana, saltString);

    return (saltString, hashString);
}
```

### 3. Validação de Entrada

```csharp
[Required(ErrorMessage = "Nome é obrigatório")]
[StringLength(150, MinimumLength = 3,
    ErrorMessage = "Nome deve ter entre 3 e 150 caracteres")]
public string Nome { get; set; }
```

---

## 📊 Diagrama de Fluxo de Dados

```
[Cliente HTTP]
      ↓
[Driving.Api - Controllers]
      ↓
[Core.Application - Services]
      ↓
[Core.Domain - Entidades]
      ↓
[Repositories + DbContext]
      ↓
[SQLite / SQL Server]
```

---

## 🎯 Decisões Arquiteturais

### 1. Por que Layered Architecture?

✅ Separação clara de responsabilidades
✅ Fácil de entender e manter
✅ Cada layer pode ser testado independentemente
✅ Permite que diferentes desenvolvedores trabalhem em paralelo

### 2. Por que Domain-Driven Design?

✅ Lógica de negócio fica no domínio
✅ Entidades ricas com comportamento
✅ Reduz bugs relacionados a validação
✅ Facilita testes de lógica crítica

### 3. Por que Repository Pattern?

✅ Abstração da camada de dados
✅ Fácil trocar de banco de dados
✅ Melhor testabilidade com mocks
✅ Queries encapsuladas

### 4. Por que DTO Pattern?

✅ Desacoplamento entre camadas
✅ Segurança (exposição seletiva de propriedades)
✅ Validação específica de contexto
✅ Versionamento de API

---

## 📈 Escalabilidade

### Melhorias Futuras

1. **CQRS (Command Query Responsibility Segregation)**
   - Separar escrita e leitura
   - Otimizar queries pesadas

2. **Event Sourcing**
   - Armazenar todos os eventos
   - Reconstruir estado histórico

3. **Microserviços**
   - Separar domínios (Clientes, Autenticação, Crédito)
   - Comunicação via RabbitMQ

4. **Cache Distribuído**
   - Redis para cache compartilhado
   - Sincronização entre instâncias

5. **Sharding**
   - Particionar dados por região
   - Escalabilidade horizontal

---

## 🧪 Estratégia de Testes

### Pirâmide de Testes

```
        △
       /|\
      / | \
     /  |  \  E2E Tests (1-5%)
    /   |   \
   /____|____\
  /     |     \
 /      |      \ Integration Tests (10-15%)
/       |       \
/________|_______\
\               /
 \    Unit      / Unit Tests (80-90%)
  \   Tests    /
   \         /
    \_______/
```

### Tipos de Testes

**Unit Tests** (80-90%):
- Testar entidades isoladas
- Testar serviços com mocks
- Testar validações

**Integration Tests** (10-15%):
- Testar fluxos com banco de dados real
- Testar repositórios
- Testar API endpoints

**E2E Tests** (1-5%):
- Testar fluxos completos
- Testar com cliente real

---

## 📚 Recursos & Referências

- [.NET Architecture Guides](https://learn.microsoft.com/dotnet/architecture/)
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Domain-Driven Design](https://martinfowler.com/bliki/DomainDrivenDesign.html)
- [Microservices Patterns](https://microservices.io/)

---

**Última atualização**: 2024-11-03
