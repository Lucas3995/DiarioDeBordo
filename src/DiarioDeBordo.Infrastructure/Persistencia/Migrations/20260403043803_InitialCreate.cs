using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable
#pragma warning disable CA1062 // EF Core generated migration — parameters guaranteed non-null by framework
#pragma warning disable CA1861 // EF Core generated migration — array literals are framework-required pattern

namespace DiarioDeBordo.Persistencia.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "categorias",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    nome_normalizado = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categorias", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "conteudos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    titulo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    descricao = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    anotacoes = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true),
                    nota = table.Column<decimal>(type: "numeric(4,1)", precision: 4, scale: 1, nullable: true),
                    formato = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    subtipo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    papel = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    tipo_coletanea = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    criado_em = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    progresso_estado = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    progresso_posicao_atual = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    progresso_nota_manual = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_conteudos", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "fontes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    conteudo_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tipo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    valor = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    plataforma = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    prioridade = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fontes", x => x.id);
                    table.ForeignKey(
                        name: "FK_fontes_conteudos_conteudo_id",
                        column: x => x.conteudo_id,
                        principalTable: "conteudos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "imagens_conteudo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    conteudo_id = table.Column<Guid>(type: "uuid", nullable: false),
                    origem_tipo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    caminho = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    principal = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_imagens_conteudo", x => x.id);
                    table.ForeignKey(
                        name: "FK_imagens_conteudo_conteudos_conteudo_id",
                        column: x => x.conteudo_id,
                        principalTable: "conteudos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "idx_categorias_usuario_nome_unique",
                table: "categorias",
                columns: new[] { "usuario_id", "nome_normalizado" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_conteudos_usuario_criado_em",
                table: "conteudos",
                columns: new[] { "usuario_id", "criado_em" });

            migrationBuilder.CreateIndex(
                name: "idx_conteudos_usuario_id",
                table: "conteudos",
                column: "usuario_id");

            migrationBuilder.CreateIndex(
                name: "idx_fontes_conteudo_prioridade_unique",
                table: "fontes",
                columns: new[] { "conteudo_id", "prioridade" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_imagens_conteudo_conteudo_id",
                table: "imagens_conteudo",
                column: "conteudo_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "categorias");

            migrationBuilder.DropTable(
                name: "fontes");

            migrationBuilder.DropTable(
                name: "imagens_conteudo");

            migrationBuilder.DropTable(
                name: "conteudos");
        }
    }
}
