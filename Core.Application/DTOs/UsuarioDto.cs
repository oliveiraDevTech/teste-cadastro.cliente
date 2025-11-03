namespace Core.Application.DTOs;

/// <summary>
/// DTO para criação de novo usuário
/// </summary>
public class CriarUsuarioDto
{
    /// <summary>
    /// Login único do usuário
    /// </summary>
    public string Login { get; set; } = string.Empty;

    /// <summary>
    /// Email do usuário
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Senha em texto plano
    /// </summary>
    public string Senha { get; set; } = string.Empty;

    /// <summary>
    /// Confirmação de senha
    /// </summary>
    public string ConfirmacaoSenha { get; set; } = string.Empty;

    /// <summary>
    /// Nome completo
    /// </summary>
    public string NomeCompleto { get; set; } = string.Empty;

    /// <summary>
    /// Telefone
    /// </summary>
    public string Telefone { get; set; } = string.Empty;

    /// <summary>
    /// ID do cliente (opcional)
    /// </summary>
    public Guid? ClienteId { get; set; }

    /// <summary>
    /// Tipo de usuário
    /// </summary>
    public string TipoUsuario { get; set; } = "Cliente";
}

/// <summary>
/// DTO para login do usuário
/// </summary>
public class LoginUsuarioDto
{
    /// <summary>
    /// Login ou email do usuário
    /// </summary>
    public string LoginOuEmail { get; set; } = string.Empty;

    /// <summary>
    /// Senha
    /// </summary>
    public string Senha { get; set; } = string.Empty;
}

/// <summary>
/// DTO para resposta de login com informações do usuário
/// Estende LoginResponseDto com dados adicionais
/// </summary>
public class LoginWithUserInfoResponseDto
{
    /// <summary>
    /// Resposta de login básica (token, tipo, expiração)
    /// </summary>
    public LoginResponseDto LoginResponse { get; set; } = new();

    /// <summary>
    /// ID do usuário
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Email do usuário
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Nome do usuário
    /// </summary>
    public string NomeCompleto { get; set; } = string.Empty;

    /// <summary>
    /// ID do cliente associado (se houver)
    /// </summary>
    public Guid? ClienteId { get; set; }

    /// <summary>
    /// Se o usuário é administrador
    /// </summary>
    public bool IsAdmin { get; set; }

    /// <summary>
    /// Tipo de usuário
    /// </summary>
    public string TipoUsuario { get; set; } = string.Empty;
}

/// <summary>
/// DTO para alteração de senha
/// </summary>
public class AlterarSenhaDto
{
    /// <summary>
    /// Senha atual
    /// </summary>
    public string SenhaAtual { get; set; } = string.Empty;

    /// <summary>
    /// Nova senha
    /// </summary>
    public string NovaSenha { get; set; } = string.Empty;

    /// <summary>
    /// Confirmação da nova senha
    /// </summary>
    public string ConfirmacaoNovaSenha { get; set; } = string.Empty;
}

/// <summary>
/// DTO para atualização de dados do usuário
/// </summary>
public class AtualizarUsuarioDto
{
    /// <summary>
    /// Email
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Nome completo
    /// </summary>
    public string NomeCompleto { get; set; } = string.Empty;

    /// <summary>
    /// Telefone
    /// </summary>
    public string Telefone { get; set; } = string.Empty;
}

/// <summary>
/// DTO para confirmação de email
/// </summary>
public class ConfirmarEmailDto
{
    /// <summary>
    /// Token de confirmação
    /// </summary>
    public string Token { get; set; } = string.Empty;
}

/// <summary>
/// DTO para ressetear senha (solicitar)
/// </summary>
public class SolicitarResetSenhaDto
{
    /// <summary>
    /// Email do usuário
    /// </summary>
    public string Email { get; set; } = string.Empty;
}

/// <summary>
/// DTO para reset de senha (confirmar)
/// </summary>
public class ConfirmarResetSenhaDto
{
    /// <summary>
    /// Token de reset
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Nova senha
    /// </summary>
    public string NovaSenha { get; set; } = string.Empty;

    /// <summary>
    /// Confirmação de nova senha
    /// </summary>
    public string ConfirmacaoNovaSenha { get; set; } = string.Empty;
}

/// <summary>
/// DTO para retorno de informações de usuário
/// </summary>
public class UsuarioResponseDto
{
    /// <summary>
    /// ID do usuário
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Login
    /// </summary>
    public string Login { get; set; } = string.Empty;

    /// <summary>
    /// Email
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Nome completo
    /// </summary>
    public string NomeCompleto { get; set; } = string.Empty;

    /// <summary>
    /// Telefone
    /// </summary>
    public string Telefone { get; set; } = string.Empty;

    /// <summary>
    /// ID do cliente associado
    /// </summary>
    public Guid? ClienteId { get; set; }

    /// <summary>
    /// Tipo de usuário
    /// </summary>
    public string TipoUsuario { get; set; } = string.Empty;

    /// <summary>
    /// Se é administrador
    /// </summary>
    public bool IsAdmin { get; set; }

    /// <summary>
    /// Se o email foi confirmado
    /// </summary>
    public bool EmailConfirmado { get; set; }

    /// <summary>
    /// Se está ativo
    /// </summary>
    public bool Ativo { get; set; }

    /// <summary>
    /// Data do último acesso
    /// </summary>
    public DateTime? DataUltimoAcesso { get; set; }

    /// <summary>
    /// Data de criação
    /// </summary>
    public DateTime DataCriacao { get; set; }

    /// <summary>
    /// Data de atualização
    /// </summary>
    public DateTime? DataAtualizacao { get; set; }

    /// <summary>
    /// Permissões do usuário
    /// </summary>
    public List<string> Permissoes { get; set; } = new();
}

/// <summary>
/// DTO para atualização de permissões
/// </summary>
public class AtualizarPermissoesDto
{
    /// <summary>
    /// Lista de permissões
    /// </summary>
    public List<string> Permissoes { get; set; } = new();
}

/// <summary>
/// DTO para bloqueio/desbloqueio de usuário
/// </summary>
public class BloquearDesbloquearUsuarioDto
{
    /// <summary>
    /// Se deve bloquear (true) ou desbloquear (false)
    /// </summary>
    public bool Bloquear { get; set; }

    /// <summary>
    /// Motivo do bloqueio
    /// </summary>
    public string? Motivo { get; set; }
}

/// <summary>
/// DTO para listar usuários com paginação
/// </summary>
public class ListarUsuariosDto
{
    /// <summary>
    /// Página atual
    /// </summary>
    public int Pagina { get; set; } = 1;

    /// <summary>
    /// Quantidade de itens por página
    /// </summary>
    public int ItensPorPagina { get; set; } = 10;

    /// <summary>
    /// Filtro de busca (login ou email)
    /// </summary>
    public string? Filtro { get; set; }

    /// <summary>
    /// Tipo de usuário a filtrar
    /// </summary>
    public string? TipoUsuario { get; set; }

    /// <summary>
    /// Apenas usuários ativos
    /// </summary>
    public bool ApenasAtivos { get; set; } = true;
}

/// <summary>
/// DTO para resposta paginada de usuários
/// </summary>
public class PaginatedUsuariosResponseDto
{
    /// <summary>
    /// Total de registros
    /// </summary>
    public int Total { get; set; }

    /// <summary>
    /// Página atual
    /// </summary>
    public int Pagina { get; set; }

    /// <summary>
    /// Total de páginas
    /// </summary>
    public int TotalPaginas { get; set; }

    /// <summary>
    /// Usuários da página atual
    /// </summary>
    public List<UsuarioResponseDto> Usuarios { get; set; } = new();
}
