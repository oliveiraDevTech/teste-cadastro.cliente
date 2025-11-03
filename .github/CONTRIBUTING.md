# Contribuindo para o Projeto

Obrigado por seu interesse em contribuir para o **Cadastro de Clientes API**! Este documento fornece diretrizes e instruções para contribuir ao projeto.

## 📋 Código de Conduta

Este projeto adota um [Código de Conduta](./CODE_OF_CONDUCT.md) que esperamos que todos os colaboradores sigam.

## 🚀 Como Começar

### 1. Fork e Clone

```bash
# Fork o repositório no GitHub
# Clone o seu fork
git clone https://github.com/seu-usuario/cadastro-clientes-api.git
cd cadastro-clientes-api

# Adicione o repositório original como upstream
git remote add upstream https://github.com/original/cadastro-clientes-api.git
```

### 2. Configure o Ambiente

```bash
# Restaure as dependências
dotnet restore

# Configure o banco de dados de desenvolvimento
cd Driven.SqlLite
dotnet ef database update
cd ..

# Execute os testes
dotnet test
```

### 3. Crie uma Branch

```bash
# Atualize a main/master
git fetch upstream
git checkout main
git merge upstream/main

# Crie uma nova branch para sua feature
git checkout -b feature/minha-feature
# ou para bugfix
git checkout -b bugfix/meu-bugfix
```

## 📝 Padrões de Código

### Nomenclatura

Seguimos as convenções C# padrão:

```csharp
// Classes, métodos, propriedades: PascalCase
public class ClienteService { }
public void CriarCliente() { }
public string NomeCliente { get; set; }

// Constantes: UPPER_SNAKE_CASE
private const int TENTATIVAS_MAXIMAS = 5;
private const string CHAVE_CACHE = "cliente_";

// Variáveis locais, parâmetros: camelCase
var nomeCliente = "João";
var idUsuario = Guid.NewGuid();

// Campos privados: _camelCase
private readonly IClienteRepository _repository;
private string _senhaHash;
```

### Estrutura de Pastas

Mantenha a estrutura padrão de camadas:

```
[Projeto]/
├── [Entidades/Models]
├── [DTOs] ou [ResponseModels]
├── [Services] ou [Repositories]
├── [Validators] ou [Specifications]
├── [Mappers]
└── [Extensions]
```

### Comentários e Documentação

Use **XML Documentation Comments** para métodos públicos:

```csharp
/// <summary>
/// Cria um novo cliente com os dados fornecidos
/// </summary>
/// <param name="nome">Nome completo do cliente</param>
/// <param name="email">Email válido do cliente</param>
/// <returns>Cliente criado com sucesso</returns>
/// <exception cref="ArgumentException">Lançado quando dados inválidos</exception>
public async Task<Cliente> CriarClienteAsync(string nome, string email)
{
    // Implementation
}
```

### Validação e Tratamento de Erros

```csharp
// Validação rápida (fast-fail)
public void ValidarDados(string email)
{
    if (string.IsNullOrWhiteSpace(email))
        throw new ArgumentException("Email é obrigatório");

    if (!ValidarEmail(email))
        throw new ArgumentException("Email inválido");
}

// Tratamento de exceções
try
{
    await _repository.CriarAsync(cliente);
}
catch (DbUpdateException ex)
{
    _logger.LogError(ex, "Erro ao salvar cliente no banco de dados");
    throw;
}
```

### Async/Await

Sempre use `async/await` para operações I/O:

```csharp
// ✅ Correto
public async Task<ClienteDto> ObterPorIdAsync(Guid id)
{
    var cliente = await _repository.ObterPorIdAsync(id);
    return _mapper.Map<ClienteDto>(cliente);
}

// ❌ Evitar
public ClienteDto ObterPorId(Guid id)
{
    var cliente = _repository.ObterPorIdAsync(id).Result;
    return _mapper.Map<ClienteDto>(cliente);
}
```

## 🧪 Testes

### Cobertura de Testes

- **Mínimo aceitável**: 80% de cobertura
- **Objetivo**: 90%+
- **Testes críticos**: 100% (segurança, lógica de negócio)

### Estrutura de Testes

Use **Arrange-Act-Assert**:

```csharp
[Fact]
public async Task CriarClienteAsync_ComDadosValidos_DeveRetornarClienteCriado()
{
    // Arrange
    var clienteDto = new ClienteCreateDto
    {
        Nome = "João Silva",
        Email = "joao@example.com",
        // ...
    };

    // Act
    var resultado = await _service.CriarAsync(clienteDto);

    // Assert
    Assert.True(resultado.Sucesso);
    Assert.NotNull(resultado.Dados);
    Assert.Equal("João Silva", resultado.Dados.Nome);
}
```

### Naming Convention para Testes

```
[MethodName]_[Scenario]_[ExpectedResult]

Exemplo:
- CriarClienteAsync_ComDadosValidos_DeveRetornarSucesso
- ObterPorIdAsync_ComIdInvalido_DeveLancarArgumentException
- AtualizarClienteAsync_ComEmailDuplicado_DeveFalhar
```

### Executar Testes

```bash
# Todos os testes
dotnet test

# Teste específico
dotnet test --filter "ClassName=ClienteServiceTests"

# Com cobertura
dotnet test /p:CollectCoverage=true /p:CoverageFormat=cobertura

# Modo watch
dotnet test --watch
```

## 📌 Commits

### Formato de Mensagem

Seguimos **Conventional Commits**:

```
<tipo>(<escopo>): <descrição>

<corpo>

<rodapé>
```

### Tipos

- **feat**: Nova funcionalidade
- **fix**: Correção de bug
- **docs**: Mudanças em documentação
- **style**: Formatação, sem mudança lógica
- **refactor**: Refatoração sem mudança de comportamento
- **perf**: Melhoria de performance
- **test**: Adição ou atualização de testes
- **chore**: Mudanças em build, dependências, etc.

### Exemplos

```bash
git commit -m "feat(clientes): adicionar filtro de busca por CPF"

git commit -m "fix(auth): corrigir expiração de token JWT

Ajusta o tempo de expiração do token para 60 minutos
conforme especificação de segurança.

Fixes #123"

git commit -m "docs(readme): atualizar seção de instalação"

git commit -m "refactor(cliente-service): simplificar lógica de validação"
```

## 🔄 Pull Request

### Antes de Enviar

1. **Atualize da upstream**:
   ```bash
   git fetch upstream
   git rebase upstream/main
   ```

2. **Execute os testes**:
   ```bash
   dotnet test
   ```

3. **Verifique a cobertura**:
   ```bash
   dotnet test /p:CollectCoverage=true
   ```

4. **Build local**:
   ```bash
   dotnet build
   ```

5. **Verifique o código**:
   ```bash
   # Se tiver Roslyn analyzers configurados
   dotnet build /p:EnforceCodeStyleInBuild=true
   ```

### Criando um Pull Request

1. **Push para seu fork**:
   ```bash
   git push origin feature/minha-feature
   ```

2. **Abra um PR no GitHub** com:
   - **Título descritivo**: `feat(clientes): adicionar dashboard de crédito`
   - **Descrição**: Explique o quê, por quê e como
   - **Linked Issues**: `Fixes #123`
   - **Checklist**:
     - [ ] Testes adicionados/atualizados
     - [ ] Documentação atualizada
     - [ ] Não há breaking changes
     - [ ] Cobertura de testes >= 80%

### Template de PR

```markdown
## Descrição

Descrição clara do que foi alterado.

## Tipo de Mudança

- [ ] Bug fix (mudança não-breaking que corrige um issue)
- [ ] Nova funcionalidade (mudança não-breaking que adiciona funcionalidade)
- [ ] Breaking change (mudança que quebra compatibilidade)
- [ ] Documentação

## Mudanças Realizadas

- Item 1
- Item 2
- Item 3

## Como Foi Testado?

Descreva os testes realizados.

## Checklist

- [ ] Meu código segue o style guide do projeto
- [ ] Realizei self-review do meu próprio código
- [ ] Comentei meu código, especialmente em lógica complexa
- [ ] Atualizei a documentação correspondente
- [ ] Minhas mudanças não geram novos warnings
- [ ] Adicionei testes que provam meu fix/feature funciona
- [ ] Testes novos e existentes passam localmente com minhas mudanças
```

## 🔍 Revisão de Código

### O que Esperamos

- **Código limpo**: Fácil de entender e manter
- **Documentação**: Comentários onde necessário
- **Testes**: Cobertura adequada
- **Performance**: Sem degradação de performance
- **Segurança**: Sem vulnerabilidades conhecidas

### Feedback

Todos os comentários em reviews de código têm objetivo **construtivo**. Estamos aqui para aprender juntos!

## 🐛 Reportando Bugs

### Template de Issue

```markdown
## Descrição do Bug

Descrição clara e concisa do bug.

## Passos para Reproduzir

1. Faça isso
2. Então isso
3. E isso

## Comportamento Esperado

O que deveria acontecer.

## Comportamento Atual

O que realmente acontece.

## Ambiente

- **OS**: Windows 10 / macOS / Linux
- **.NET Version**: 8.0
- **Navegador** (se aplicável): Chrome 120

## Logs/Stack Trace

```
Cole aqui a stack trace ou logs de erro
```

## Screenshots

Se aplicável, adicione screenshots.
```

## 📚 Documentação

Ao adicionar novos endpoints, services ou funcionalidades:

1. **Atualize o README.md**
2. **Adicione comentários XML**
3. **Atualize o Swagger/OpenAPI** (se aplicável)
4. **Crie exemplos de uso**

## 🔐 Segurança

Ao relatar vulnerabilidades:

1. **NÃO** crie uma issue pública
2. Envie um email para: `security@example.com`
3. Inclua detalhes da vulnerabilidade
4. Dê tempo para correção antes de divulgar

## 📋 Checklist Final

Antes de enviar seu PR, verifique:

- [ ] Código segue as convenções do projeto
- [ ] Testes foram adicionados/atualizados
- [ ] Documentação foi atualizada
- [ ] Commits têm mensagens descritivas
- [ ] Não há arquivos desnecessários inclusos
- [ ] Sem secrets ou dados sensíveis
- [ ] Build passa localmente
- [ ] Testes passam
- [ ] Sem warnings de compilação
- [ ] Código foi revisado por você mesmo primeiro

## 🎓 Recursos

- [C# Coding Conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- [.NET API Design Guidelines](https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/)
- [Conventional Commits](https://www.conventionalcommits.org/)
- [Keep a Changelog](https://keepachangelog.com/)

## ❓ Dúvidas?

- Abra uma **Discussion** no GitHub
- Crie uma **Issue** com tag `question`
- Envie um email

---

**Obrigado por contribuir! Sua ajuda é essencial para melhorar este projeto!** 🎉
