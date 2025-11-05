using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Driven.SqlLite.Migrations
{
    /// <inheritdoc />
    public partial class AddCreditoFieldsToClientes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RankingCredito",
                table: "Clientes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ScoreCredito",
                table: "Clientes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataAtualizacaoRanking",
                table: "Clientes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AptoParaCartaoCredito",
                table: "Clientes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "CartaoId",
                table: "Clientes",
                type: "TEXT",
                nullable: true);

            // Criar índices para performance
            migrationBuilder.CreateIndex(
                name: "IX_Clientes_RankingCredito",
                table: "Clientes",
                column: "RankingCredito");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_ScoreCredito",
                table: "Clientes",
                column: "ScoreCredito");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_AptoParaCartaoCredito",
                table: "Clientes",
                column: "AptoParaCartaoCredito");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_CartaoId",
                table: "Clientes",
                column: "CartaoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Clientes_RankingCredito",
                table: "Clientes");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_ScoreCredito",
                table: "Clientes");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_AptoParaCartaoCredito",
                table: "Clientes");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_CartaoId",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "RankingCredito",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "ScoreCredito",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "DataAtualizacaoRanking",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "AptoParaCartaoCredito",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "CartaoId",
                table: "Clientes");
        }
    }
}
