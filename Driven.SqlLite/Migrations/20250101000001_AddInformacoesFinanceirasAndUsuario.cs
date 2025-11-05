using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Driven.SqlLite.Migrations
{
    /// <inheritdoc />
    public partial class AddInformacoesFinanceirasAndUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Criar tabela InformacoesFinanceirasClientes
            migrationBuilder.CreateTable(
                name: "InformacoesFinanceirasClientes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ClienteId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Renda = table.Column<decimal>(type: "TEXT", precision: 15, scale: 2, nullable: false),
                    RendaComprovada = table.Column<decimal>(type: "TEXT", precision: 15, scale: 2, nullable: false),
                    ScoreCredito = table.Column<int>(type: "INTEGER", nullable: false),
                    RankingCredito = table.Column<int>(type: "INTEGER", nullable: false),
                    LimiteCreditoSugerido = table.Column<decimal>(type: "TEXT", precision: 15, scale: 2, nullable: false),
                    LimiteCreditoAtivo = table.Column<decimal>(type: "TEXT", precision: 15, scale: 2, nullable: false),
                    TotalDebitos = table.Column<decimal>(type: "TEXT", precision: 15, scale: 2, nullable: false),
                    CreditosEmAberto = table.Column<int>(type: "INTEGER", nullable: false),
                    AtrasosDiversos12Meses = table.Column<int>(type: "INTEGER", nullable: false),
                    AptoParaCartaoCredito = table.Column<bool>(type: "INTEGER", nullable: false),
                    CartoesEmitidos = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    DataUltimaAnalise = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DataProximaAnaliseRecomendada = table.Column<DateTime>(type: "TEXT", nullable: true),
                    MotivoRecusa = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    AnaliseRiscoCredito = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    Recomendacoes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CriadoPor = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    AtualizadoPor = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InformacoesFinanceirasClientes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InformacoesFinanceirasClientes_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Criar índices para InformacoesFinanceirasClientes
            migrationBuilder.CreateIndex(
                name: "IX_InformacoesFinanceiras_ClienteId",
                table: "InformacoesFinanceirasClientes",
                column: "ClienteId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InformacoesFinanceiras_RankingCredito",
                table: "InformacoesFinanceirasClientes",
                column: "RankingCredito");

            migrationBuilder.CreateIndex(
                name: "IX_InformacoesFinanceiras_ScoreCredito",
                table: "InformacoesFinanceirasClientes",
                column: "ScoreCredito");

            // Criar tabela Usuarios
            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Login = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    SenhaHash = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    SenhaSalt = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    ClienteId = table.Column<Guid>(type: "TEXT", nullable: true),
                    NomeCompleto = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Telefone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    DataUltimoAcesso = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DataAlteracaoSenha = table.Column<DateTime>(type: "TEXT", nullable: true),
                    TipoUsuario = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Permissoes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    IsAdmin = table.Column<bool>(type: "INTEGER", nullable: false),
                    TentativasLoginFalhadas = table.Column<int>(type: "INTEGER", nullable: false),
                    DataBloqueio = table.Column<DateTime>(type: "TEXT", nullable: true),
                    MotivoBloqueio = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    EmailConfirmado = table.Column<bool>(type: "INTEGER", nullable: false),
                    TokenConfirmacaoEmail = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CriadoPor = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    AtualizadoPor = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Usuarios_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            // Criar índices para Usuarios
            migrationBuilder.CreateIndex(
                name: "IX_Usuario_Ativo",
                table: "Usuarios",
                column: "Ativo");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_ClienteId",
                table: "Usuarios",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_Login",
                table: "Usuarios",
                column: "Login",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InformacoesFinanceirasClientes");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
