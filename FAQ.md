# FAQ - Perguntas Frequentes

## Instalação & Setup

### P: Qual versão do .NET é necessária?

**R**: .NET 8 ou superior. Você pode verificar a versão instalada com:
```bash
dotnet --version
```

Para instalar: https://dotnet.microsoft.com/download

### P: Como faço para executar o projeto localmente?

**R**:
```bash
# 1. Clone o repositório
git clone https://github.com/seu-usuario/cadastro-clientes-api.git
cd cadastro-clientes-api

# 2. Restaure as dependências
dotnet restore

# 3. Crie o banco de dados
cd Driven.SqlLite
dotnet ef database update
cd ..

# 4. Execute a aplicação
cd Driving.Api
dotnet run

# 5. Acesse http://localhost:5000
```

### P: O RabbitMQ é obrigatório?

**R**: **Não**. O RabbitMQ é opcional. Se não estiver disponível, a aplicação continua funcionando sem mensageria. Você verá um aviso no console:
```
⚠️  Aviso: RabbitMQ não foi inicializado
```

Para usar RabbitMQ:
```bash
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3.12-management
```

Acesse: http://localhost:15672 (guest/guest)

### P: Como configuro as variáveis de ambiente?

**R**: Existem três formas:

**1. appsettings.json** (padrão):
```json
{
  "Jwt": {
    "Secret": "sua-chave-secreta"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=cadastro_clientes.db;"
  }
}
```

**2. Variáveis de ambiente**:
```bash
export Jwt__Secret="sua-chave-secreta"
export ConnectionStrings__DefaultConnection="Data Source=cadastro_clientes.db;"
```

**3. User Secrets** (desenvolvimento):
```bash
dotnet user-secrets set "Jwt:Secret" "sua-chave-secreta"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Data Source=cadastro_clientes_dev.db;"
```

---

## Autenticação & Segurança

### P: Como faço login?

**R**:
```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "login": "usuario123",
    "senha": "Senha123!"
  }'
```

Resposta:
```json
{
  "sucesso": true,
  "dados": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "expiresIn": 3600
  }
}
```

### P: Quanto tempo dura um token?

**R**: Por padrão, **60 minutos**. Você pode alterar em `appsettings.json`:
```json
{
  "Jwt": {
    "ExpirationMinutes": 60
  }
}
```

### P: Como uso o token nos requests?

**R**: Adicione o header `Authorization: Bearer <token>`:

```bash
curl -X GET http://localhost:5000/api/clientes \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

Ou no Swagger, clique no botão "Authorize" (cadeado) no canto superior direito.

### P: A senha do usuário é segura?

**R**: Sim! Usamos:
- **PBKDF2** com SHA256
- **10.000 iterações** de hash
- **Salt aleatório** de 32 bytes

Verificação:
```csharp
var (salt, hash) = GerarHashSenha("Senha123!");
// salt: QvR9mK2pL5xY8nJ6hG3tF1wE9oI2s7uM5vN1...
// hash: aBcDeFgHiJkLmNoPqRsTuVwXyZ1a2b3c4d5e6f7g8...
```

### P: Quantas tentativas de login são permitidas?

**R**: **5 tentativas**. Após isso, a conta é bloqueada automaticamente. Um administrador pode desbloquear.

### P: Como desbloqueio uma conta?

**R**: Via endpoint (requer Admin):
```bash
curl -X POST http://localhost:5000/api/auth/desbloquear/{usuarioId} \
  -H "Authorization: Bearer ADMIN_TOKEN"
```

Ou diretamente no banco de dados:
```sql
UPDATE Usuario SET DataBloqueio = NULL, TentativasLoginFalhadas = 0
WHERE Id = '...';
```

### P: Preciso confirmar o email?

**R**: Sim. Ao registrar, um token é gerado. Use o endpoint:
```bash
curl -X POST http://localhost:5000/api/auth/confirm-email \
  -d '{
    "email": "usuario@example.com",
    "token": "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQq"
  }'
```

---

## Clientes & Dados

### P: Posso deletar um cliente?

**R**: Sim, mas usa **soft delete** (não remove do banco):
```bash
curl -X DELETE http://localhost:5000/api/clientes/{id} \
  -H "Authorization: Bearer TOKEN"
```

O cliente é marcado como `Ativo = false`. Para restaurar:
```sql
UPDATE Cliente SET Ativo = 1 WHERE Id = '...';
```

### P: Como pesquiso clientes?

**R**: Use o endpoint de pesquisa:
```bash
# Pesquisar por nome
curl -X GET "http://localhost:5000/api/clientes/pesquisar?nome=Maria&pagina=1" \
  -H "Authorization: Bearer TOKEN"

# Listar todos (paginado)
curl -X GET "http://localhost:5000/api/clientes?pagina=1&itensPorPagina=10" \
  -H "Authorization: Bearer TOKEN"
```

### P: Qual é o máximo de itens por página?

**R**: 100. Se tentar mais:
```bash
# Isso retorna 10 itens (padrão)
curl "http://localhost:5000/api/clientes?itensPorPagina=150"
```

### P: Como calcula o score de crédito?

**R**: O score é atualizado manualmente via:
```bash
curl -X PUT http://localhost:5000/api/clientes/{id}/ranking \
  -H "Authorization: Bearer TOKEN" \
  -d '{
    "rankingCredito": 4,
    "scoreCredito": 850
  }'
```

Elegibilidade para cartão:
- `RankingCredito >= 3` E `ScoreCredito >= 600`

### P: Posso importar clientes em bulk?

**R**: **Não** na v1.0.0. Mas planejamos para v1.1.0.

Para agora, use um script:
```bash
#!/bin/bash
while IFS=',' read nome email telefone cpf endereco cidade estado cep; do
  curl -X POST http://localhost:5000/api/clientes \
    -H "Content-Type: application/json" \
    -H "Authorization: Bearer TOKEN" \
    -d "{\"nome\":\"$nome\",\"email\":\"$email\",...}"
done < clientes.csv
```

### P: Como exporto os clientes?

**R**: Use consultas diretas ao banco ou SQL:
```bash
sqlite3 cadastro_clientes.db "SELECT * FROM Clientes;" > clientes.csv
```

Planejamos API de exportação para v1.1.0.

---

## Validação & Erros

### P: Qual é o formato válido de CPF?

**R**:
- **11 dígitos** (com ou sem separadores)
- **Válido com algoritmo de verificação**

Exemplos:
```
123.456.789-09  ✅ Válido
12345678909     ✅ Válido
123456789       ❌ Muito curto
12345678901     ❌ Algoritmo falha
```

### P: O que significa "Email já registrado"?

**R**: Cada email é único no sistema. Você tem dois clientes com o mesmo email?

```json
{
  "sucesso": false,
  "mensagem": "Email já registrado",
  "erros": ["Este email já está associado a outro cliente"]
}
```

Use um email diferente ou atualize o cliente existente.

### P: Quais são todos os campos obrigatórios?

**R**:

**Cliente**:
- Nome (3-150 caracteres)
- Email (válido)
- Telefone (10-11 dígitos)
- CPF (11 dígitos, válido)
- Endereço (5-200 caracteres)
- Cidade (2-100 caracteres)
- Estado (2 caracteres: SP, RJ, MG, etc)
- CEP (8 dígitos)

**Usuário**:
- Login (3-50 caracteres)
- Email (válido)
- Senha (8+ caracteres, com maiúscula, minúscula, número)
- Nome Completo (3-150 caracteres)
- Telefone (10-11 dígitos)

### P: Como corrijo um erro de validação?

**R**: Verifique o campo específico e corrija:

```json
{
  "sucesso": false,
  "mensagem": "Dados inválidos para criar cliente",
  "erros": [
    "Email inválido",
    "Telefone deve ter entre 10 e 11 dígitos"
  ]
}
```

Envie novamente com dados corretos.

---

## Testes

### P: Como executo os testes?

**R**:
```bash
# Todos os testes
cd Test.XUnit
dotnet test

# Teste específico
dotnet test --filter "ClassName=ClienteServiceTests"

# Com output detalhado
dotnet test -v detailed

# Com cobertura de código
dotnet test /p:CollectCoverage=true /p:CoverageFormat=cobertura
```

### P: Como crio um novo teste?

**R**:
```csharp
[Fact]
public async Task CriarClienteAsync_ComDadosValidos_DeveRetornarSucesso()
{
    // Arrange
    var clienteDto = new ClienteCreateDto
    {
        Nome = "João Silva",
        Email = "joao@example.com",
        // ...
    };

    var mockRepository = new Mock<IClienteRepository>();
    var service = new ClienteService(mockRepository.Object);

    // Act
    var resultado = await service.CriarAsync(clienteDto);

    // Assert
    Assert.True(resultado.Sucesso);
}
```

### P: Qual é a cobertura de testes atual?

**R**: Use:
```bash
dotnet test /p:CollectCoverage=true

# Saída:
# Total Lines: 500
# Covered Lines: 450
# Coverage: 90%
```

---

## Performance & Otimização

### P: A aplicação está lenta, o que faço?

**R**:

1. **Verifique o banco de dados**:
   ```bash
   sqlite3 cadastro_clientes.db ".indices"
   ```

2. **Ative cache**:
   ```csharp
   services.AddMemoryCache();
   ```

3. **Use paginação**:
   ```bash
   curl "http://localhost:5000/api/clientes?pagina=1&itensPorPagina=50"
   ```

4. **Migre para SQL Server** (SQLite tem limitações)

5. **Ative logging de performance**:
   ```json
   {
     "Logging": {
       "LogLevel": {
         "Default": "Debug"
       }
     }
   }
   ```

### P: Como monitoro performance?

**R**: Verifique os logs:
```bash
tail -f logs/cadastro-clientes.log | grep -i "duration"
```

Ou implemente Prometheus:
```csharp
services.AddPrometheusMetrics();
```

---

## Banco de Dados

### P: Como faço backup do banco?

**R**: Com SQLite:
```bash
# Backup
cp cadastro_clientes.db cadastro_clientes.backup.db

# Restaurar
cp cadastro_clientes.backup.db cadastro_clientes.db
```

Com SQL Server:
```sql
BACKUP DATABASE [cadastro_clientes]
TO DISK = 'C:\Backups\cadastro_clientes.bak';
```

### P: Como vejo os dados do banco?

**R**:

```bash
# SQLite
sqlite3 cadastro_clientes.db
> SELECT * FROM Clientes;
> .tables
> .schema

# SQL Server (SSMS ou Azure Data Studio)
SELECT * FROM [dbo].[Clientes];
```

### P: Como resetO o banco de dados?

**R**: **CUIDADO - PERDA DE DADOS**

```bash
# SQLite
rm cadastro_clientes.db
cd Driven.SqlLite
dotnet ef database update

# SQL Server
DROP DATABASE cadastro_clientes;
CREATE DATABASE cadastro_clientes;
dotnet ef database update
```

### P: Como migro de SQLite para SQL Server?

**R**:

1. Atualize `Driven.SqlLite/Data/ApplicationDbContext.cs`:
```csharp
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    optionsBuilder.UseSqlServer(connectionString);
}
```

2. Atualize a connection string em `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=seu-servidor;Database=cadastro_clientes;User Id=sa;Password=SuaSenha123;"
  }
}
```

3. Crie migrations:
```bash
dotnet ef migrations add MigrateToSqlServer
dotnet ef database update
```

---

## Docker & Deployment

### P: Como rodo a aplicação com Docker?

**R**:
```bash
docker build -t cadastro-api:latest .
docker run -d \
  --name cadastro-api \
  -p 5000:8080 \
  -e Jwt__Secret="sua-chave-secreta" \
  cadastro-api:latest
```

### P: Como uso Docker Compose?

**R**:
```bash
docker-compose up -d
```

Verifique:
```bash
docker-compose ps
docker-compose logs api
```

### P: Como deploys a aplicação?

**R**: Depende da plataforma:

**Azure**:
```bash
az webapp create --name seu-app --resource-group seu-grupo
dotnet publish -c Release -o ./publish
```

**AWS**:
```bash
eb init
eb create
eb deploy
```

**Heroku**:
```bash
heroku login
heroku create seu-app
git push heroku main
```

---

## Troubleshooting

### P: "Connection refused" ao conectar ao RabbitMQ

**R**: RabbitMQ não está rodando. Inicie-o:
```bash
docker run -d --name rabbitmq -p 5672:5672 rabbitmq:3.12-management
```

### P: "Database is locked" (SQLite)

**R**: Fecha outras conexões com o banco ou migra para SQL Server.

### P: "Invalid JWT token"

**R**:
- Token expirou? Faça novo login
- Secret está diferente? Verifique appsettings.json
- Token malformado? Decodifique em jwt.io

### P: 500 Internal Server Error

**R**: Verifique os logs:
```bash
tail -f logs/cadastro-clientes.log
```

Ative debug mode:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug"
    }
  }
}
```

### P: Esqueci a senha do banco

**R**: Resete:
```bash
# SQLite: delete o arquivo
rm cadastro_clientes.db

# SQL Server: resete com admin
ALTER LOGIN sa WITH PASSWORD = 'NovaSenha123!';
```

---

## Contribuição

### P: Como contribuo com o projeto?

**R**: Veja [CONTRIBUTING.md](.github/CONTRIBUTING.md)

1. Fork o repositório
2. Crie uma branch (`git checkout -b feature/minha-feature`)
3. Commit mudanças (`git commit -m 'feat: adicionar nova feature'`)
4. Push (`git push origin feature/minha-feature`)
5. Abra um Pull Request

### P: Qual padrão de código devo seguir?

**R**: Veja [CONTRIBUTING.md](.github/CONTRIBUTING.md) - seção "Padrões de Código"

Resumo:
- **PascalCase**: Classes, métodos, propriedades públicas
- **camelCase**: Variáveis locais, parâmetros
- **_camelCase**: Campos privados
- **UPPER_SNAKE_CASE**: Constantes

### P: Como reporto um bug?

**R**: Abra um [GitHub Issue](https://github.com/seu-repo/issues) com:
- Descrição clara
- Passos para reproduzir
- Comportamento esperado vs atual
- Screenshots (se aplicável)

---

## Licença & Legal

### P: Qual é a licença deste projeto?

**R**: **MIT License**. Veja [LICENSE](./LICENSE) para detalhes.

### P: Posso usar comercialmente?

**R**: Sim! MIT permite uso comercial livre.

### P: Preciso mantê-lo open source?

**R**: Não é obrigatório, mas é apreciado!

---

## Contato & Suporte

### P: Onde posso obter ajuda?

**R**:
- 📖 [README.md](./README.md) - Documentação completa
- 🐛 [Issues](https://github.com/seu-repo/issues) - Bugs
- 💬 [Discussions](https://github.com/seu-repo/discussions) - Dúvidas gerais
- 📧 Email: desenvolvimento@example.com
- 🤝 [CONTRIBUTING.md](.github/CONTRIBUTING.md) - Como contribuir

### P: Como sugiro uma feature?

**R**: Abra uma [Discussion](https://github.com/seu-repo/discussions) ou [Issue](https://github.com/seu-repo/issues) com tag `feature-request`

---

**Última atualização**: 2024-11-03

**Não encontrou sua pergunta?** Abra uma [Discussion](https://github.com/seu-repo/discussions)! 🎉
