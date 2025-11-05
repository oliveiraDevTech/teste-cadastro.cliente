using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Core.Application;
using Driven.SqlLite;
using Driven.SqlLite.Data;
using Driven.RabbitMQ;
using Driven.RabbitMQ.Interfaces;
using Core.Infra;

var builder = WebApplication.CreateBuilder(args);

// ========== CONFIGURAÇÃO DE SERVIÇOS ==========

// Adicionar Controllers
builder.Services.AddControllers();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Cadastro de Clientes API",
        Version = "v1",
        Description = "API para gestão de clientes com autenticação JWT",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Desenvolvimento Backend"
        }
    });

    // Configurar autenticação JWT no Swagger
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

// Obter JWT Secret das variáveis de ambiente ou usar padrão para desenvolvimento
var jwtSecret = builder.Configuration["Jwt:Secret"]
    ?? builder.Configuration["JWT_SECRET"]
    ?? "sua_chave_super_secreta_com_minimo_32_caracteres_para_producao";

// Obter outras configurações JWT
var jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? builder.Configuration["JWT_ISSUER"]
    ?? "CadastroClientesApi";

var jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? builder.Configuration["JWT_AUDIENCE"]
    ?? "CadastroClientesApp";

var jwtExpirationMinutes = int.TryParse(
    builder.Configuration["Jwt:ExpirationMinutes"]
    ?? builder.Configuration["JWT_EXPIRATION"],
    out var expiration) ? expiration : 60;

// Configurar autenticação JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecret)),
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,
        ValidateAudience = true,
        ValidAudience = jwtAudience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Injeção de dependências - Camada de Aplicação
builder.Services.AddApplicationServices(jwtSecret, jwtIssuer, jwtAudience, jwtExpirationMinutes);

// Injeção de dependências - Camada de Dados
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=cadastro_clientes.db;";
builder.Services.AddSqlLiteDatabase(connectionString);

// Injeção de dependências - Infraestrutura (Logging, Cache, Email)
builder.Services.AddInfrastructureServices(builder.Configuration);

// Injeção de dependências - RabbitMQ (Mensageria)
builder.Services.AddRabbitMQServices(builder.Configuration);

// ========== CONSTRUIR APLICAÇÃO ==========

var app = builder.Build();

// ========== CONFIGURAR HTTP REQUEST PIPELINE ==========

// Sempre usar Swagger (Development e Production)
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Cadastro de Clientes API v1");
    options.RoutePrefix = "swagger";
    options.DocumentTitle = "Cadastro de Clientes API";
});

if (app.Environment.IsDevelopment())
{
    // Desabilitar redireção HTTPS em desenvolvimento para evitar erros de certificado
}
else
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowAll");

// ========== MAPEAR HEALTH CHECK ENDPOINT ==========
app.MapHealthChecks("/api/health");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ========== APLICAR MIGRATIONS E CRIAR BANCO AUTOMATICAMENTE ==========

try
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Criar banco de dados automaticamente
        Console.WriteLine("🔄 Criando banco de dados...");
        dbContext.Database.EnsureCreated();
        Console.WriteLine("✅ Banco de dados criado com sucesso!");

        // ========== SEED DE USUÁRIO PADRÃO ==========
        
        // Verificar se já existe usuário padrão
        var usuarioExiste = dbContext.Usuarios.Any(u => u.Login == "user");
        
        if (!usuarioExiste)
        {
            Console.WriteLine("🔄 Criando usuário padrão...");
            
            // Criar usuário padrão: user / Password123
            var usuario = Core.Domain.Entities.Usuario.Criar(
                login: "user",
                email: "user@sistema.com",
                senhaPlana: "Password123",  // Senha forte: contém maiúsculas, minúsculas e números
                nomeCompleto: "Usuário Padrão",
                telefone: "11999999999"
            );
            
            dbContext.Usuarios.Add(usuario);
            dbContext.SaveChanges();
            
            Console.WriteLine("✅ Usuário padrão criado com sucesso!");
            Console.WriteLine("   Login: user");
            Console.WriteLine("   Senha: Password123");
        }
        else
        {
            Console.WriteLine("ℹ️  Usuário padrão já existe no banco de dados");
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Erro ao criar banco de dados: {ex.Message}");
    throw;
}

// ========== INICIALIZAR RABBITMQ (COM FALLBACK) ==========

try
{
    // Inicializa RabbitMQ se disponível
    var messageBus = app.Services.GetRequiredService<IMessageBus>();
    if (messageBus?.TryConnect() ?? false)
    {
        Console.WriteLine("✅ RabbitMQ conectado com sucesso");

        // ========== INICIALIZAR CONSUMIDORES DE EVENTOS ==========

        var subscriber = app.Services.GetRequiredService<IMessageSubscriber>();
        var analiseCreditoHandler = app.Services.GetRequiredService<Core.Application.Handlers.AnaliseCartaoCreditoCompleteEventHandler>();
        var logger = app.Services.GetRequiredService<Microsoft.Extensions.Logging.ILogger<Program>>();
        
        // Obter configurações do RabbitMQ incluindo nomes das filas
        var rabbitMQSettings = app.Configuration.GetSection("RabbitMQ").Get<Driven.RabbitMQ.Settings.RabbitMQSettings>();
        var queueName = rabbitMQSettings?.Queues?.AnaliseCreditoComplete ?? "analise.credito.complete";

        // Subscrever ao evento de análise de crédito completa
        try
        {
            await subscriber.SubscribeAsync<Driven.RabbitMQ.Events.AnaliseCartaoCreditoCompleteEvent>(
                queueName: queueName,
                handler: async (evento) =>
                {
                    logger.LogInformation("Evento AnaliseCartaoCreditoCompleteEvent recebido para cliente {ClienteId}", evento.ClienteId);
                    await analiseCreditoHandler.HandleAsync(evento);
                });

            Console.WriteLine("✅ Consumer de AnaliseCartaoCreditoCompleteEvent inicializado");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao inicializar consumer de AnaliseCartaoCreditoCompleteEvent");
        }
    }
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"⚠️  Aviso: RabbitMQ não foi inicializado. {ex.Message}");
    Console.WriteLine("A aplicação continuará funcionando sem mensageria.");
}
catch (Exception ex)
{
    Console.WriteLine($"⚠️  Aviso: Erro ao conectar ao RabbitMQ: {ex.Message}");
    Console.WriteLine("A aplicação continuará funcionando sem mensageria.");
}

app.Run();
