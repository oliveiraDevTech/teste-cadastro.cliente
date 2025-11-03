using Microsoft.EntityFrameworkCore;
using Core.Domain.Entities;

namespace Driven.SqlLite.Data;

/// <summary>
/// Contexto de banco de dados para a aplicação
/// Gerencia a persistência de dados usando Entity Framework Core e SQLite
/// </summary>
public class ApplicationDbContext : DbContext
{
    /// <summary>
    /// DbSet para a entidade Cliente
    /// </summary>
    public DbSet<Cliente> Clientes { get; set; }

    /// <summary>
    /// DbSet para a entidade Informações Financeiras do Cliente
    /// </summary>
    public DbSet<InformacoesFinanceirasCliente> InformacoesFinanceirasClientes { get; set; }

    /// <summary>
    /// DbSet para a entidade Usuário
    /// </summary>
    public DbSet<Usuario> Usuarios { get; set; }

    /// <summary>
    /// Construtor do contexto
    /// </summary>
    /// <param name="options">Opções de configuração do DbContext</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Configura o modelo do banco de dados
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configurar a entidade Cliente
        modelBuilder.Entity<Cliente>(entity =>
        {
            // Chave primária
            entity.HasKey(e => e.Id);

            // Propriedades
            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .IsRequired();

            entity.Property(e => e.Nome)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(e => e.Telefone)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(e => e.Cpf)
                .IsRequired()
                .HasMaxLength(14);

            entity.Property(e => e.Endereco)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.Cidade)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Estado)
                .IsRequired()
                .HasMaxLength(2);

            entity.Property(e => e.Cep)
                .IsRequired()
                .HasMaxLength(9);

            entity.Property(e => e.DataCriacao)
                .IsRequired()
                .HasDefaultValue(DateTime.UtcNow);

            entity.Property(e => e.DataAtualizacao)
                .IsRequired(false);

            entity.Property(e => e.Ativo)
                .IsRequired()
                .HasDefaultValue(true);

            // Índices
            entity.HasIndex(e => e.Email)
                .IsUnique()
                .HasDatabaseName("IX_Cliente_Email");

            entity.HasIndex(e => e.Cpf)
                .IsUnique()
                .HasDatabaseName("IX_Cliente_Cpf");

            entity.HasIndex(e => e.Nome)
                .HasDatabaseName("IX_Cliente_Nome");

            entity.HasIndex(e => e.Ativo)
                .HasDatabaseName("IX_Cliente_Ativo");

            // Nome da tabela
            entity.ToTable("Clientes");
        });

        // Configurar a entidade InformacoesFinanceirasCliente
        modelBuilder.Entity<InformacoesFinanceirasCliente>(entity =>
        {
            // Chave primária
            entity.HasKey(e => e.Id);

            // Propriedades
            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .IsRequired();

            entity.Property(e => e.ClienteId)
                .IsRequired();

            entity.Property(e => e.Renda)
                .HasPrecision(15, 2)
                .IsRequired();

            entity.Property(e => e.RendaComprovada)
                .HasPrecision(15, 2)
                .IsRequired();

            entity.Property(e => e.ScoreCredito)
                .IsRequired();

            entity.Property(e => e.RankingCredito)
                .IsRequired();

            entity.Property(e => e.LimiteCreditoSugerido)
                .HasPrecision(15, 2)
                .IsRequired();

            entity.Property(e => e.LimiteCreditoAtivo)
                .HasPrecision(15, 2)
                .IsRequired();

            entity.Property(e => e.TotalDebitos)
                .HasPrecision(15, 2)
                .IsRequired();

            entity.Property(e => e.CartoesEmitidos)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.AnaliseRiscoCredito)
                .IsRequired()
                .HasMaxLength(1000);

            entity.Property(e => e.Recomendacoes)
                .IsRequired()
                .HasMaxLength(1000);

            entity.Property(e => e.MotivoRecusa)
                .IsRequired(false)
                .HasMaxLength(500);

            entity.Property(e => e.DataUltimaAnalise)
                .IsRequired(false);

            entity.Property(e => e.DataProximaAnaliseRecomendada)
                .IsRequired(false);

            entity.Property(e => e.DataCriacao)
                .IsRequired()
                .HasDefaultValue(DateTime.UtcNow);

            entity.Property(e => e.DataAtualizacao)
                .IsRequired(false);

            entity.Property(e => e.Ativo)
                .IsRequired()
                .HasDefaultValue(true);

            // Índices
            entity.HasIndex(e => e.ClienteId)
                .IsUnique()
                .HasDatabaseName("IX_InformacoesFinanceiras_ClienteId");

            entity.HasIndex(e => e.RankingCredito)
                .HasDatabaseName("IX_InformacoesFinanceiras_RankingCredito");

            entity.HasIndex(e => e.ScoreCredito)
                .HasDatabaseName("IX_InformacoesFinanceiras_ScoreCredito");

            // Relacionamento com Cliente
            entity.HasOne<Cliente>()
                .WithMany()
                .HasForeignKey(e => e.ClienteId)
                .OnDelete(DeleteBehavior.Cascade);

            // Nome da tabela
            entity.ToTable("InformacoesFinanceirasClientes");
        });

        // Configurar a entidade Usuario
        modelBuilder.Entity<Usuario>(entity =>
        {
            // Chave primária
            entity.HasKey(e => e.Id);

            // Propriedades
            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .IsRequired();

            entity.Property(e => e.Login)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(e => e.SenhaHash)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(e => e.SenhaSalt)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(e => e.NomeCompleto)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(e => e.Telefone)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(e => e.TipoUsuario)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("Cliente");

            entity.Property(e => e.Permissoes)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(e => e.TokenConfirmacaoEmail)
                .IsRequired(false)
                .HasMaxLength(100);

            entity.Property(e => e.MotivoBloqueio)
                .IsRequired(false)
                .HasMaxLength(255);

            entity.Property(e => e.DataUltimoAcesso)
                .IsRequired(false);

            entity.Property(e => e.DataAlteracaoSenha)
                .IsRequired(false);

            entity.Property(e => e.DataBloqueio)
                .IsRequired(false);

            entity.Property(e => e.DataCriacao)
                .IsRequired()
                .HasDefaultValue(DateTime.UtcNow);

            entity.Property(e => e.DataAtualizacao)
                .IsRequired(false);

            entity.Property(e => e.Ativo)
                .IsRequired()
                .HasDefaultValue(true);

            entity.Property(e => e.EmailConfirmado)
                .IsRequired()
                .HasDefaultValue(false);

            entity.Property(e => e.IsAdmin)
                .IsRequired()
                .HasDefaultValue(false);

            // Índices
            entity.HasIndex(e => e.Login)
                .IsUnique()
                .HasDatabaseName("IX_Usuario_Login");

            entity.HasIndex(e => e.Email)
                .IsUnique()
                .HasDatabaseName("IX_Usuario_Email");

            entity.HasIndex(e => e.ClienteId)
                .HasDatabaseName("IX_Usuario_ClienteId");

            entity.HasIndex(e => e.Ativo)
                .HasDatabaseName("IX_Usuario_Ativo");

            // Relacionamento com Cliente (opcional)
            entity.HasOne<Cliente>()
                .WithMany()
                .HasForeignKey(e => e.ClienteId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);

            // Nome da tabela
            entity.ToTable("Usuarios");
        });
    }
}
