using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DiarioDeBordo.Persistencia.Migrations
{
    /// <inheritdoc />
    public partial class AddRelacoesCategoriasSessoes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "classificacao",
                table: "conteudos",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_filho",
                table: "conteudos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "total_esperado_sessoes",
                table: "conteudos",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "conteudo_categorias",
                columns: table => new
                {
                    conteudo_id = table.Column<Guid>(type: "uuid", nullable: false),
                    categoria_id = table.Column<Guid>(type: "uuid", nullable: false),
                    associada_em = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_conteudo_categorias", x => new { x.conteudo_id, x.categoria_id });
                    table.ForeignKey(
                        name: "FK_conteudo_categorias_categorias_categoria_id",
                        column: x => x.categoria_id,
                        principalTable: "categorias",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_conteudo_categorias_conteudos_conteudo_id",
                        column: x => x.conteudo_id,
                        principalTable: "conteudos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tipo_relacoes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    nome_inverso = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    nome_normalizado = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    is_sistema = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tipo_relacoes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "relacoes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    conteudo_origem_id = table.Column<Guid>(type: "uuid", nullable: false),
                    conteudo_destino_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tipo_relacao_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_inversa = table.Column<bool>(type: "boolean", nullable: false),
                    par_id = table.Column<Guid>(type: "uuid", nullable: true),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    criado_em = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_relacoes", x => x.id);
                    table.ForeignKey(
                        name: "FK_relacoes_conteudos_conteudo_destino_id",
                        column: x => x.conteudo_destino_id,
                        principalTable: "conteudos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_relacoes_conteudos_conteudo_origem_id",
                        column: x => x.conteudo_origem_id,
                        principalTable: "conteudos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_relacoes_tipo_relacoes_tipo_relacao_id",
                        column: x => x.tipo_relacao_id,
                        principalTable: "tipo_relacoes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "tipo_relacoes",
                columns: new[] { "id", "is_sistema", "nome", "nome_inverso", "nome_normalizado", "usuario_id" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), true, "Sequência", "Continuação de", "sequência", new Guid("00000000-0000-0000-0000-000000000000") },
                    { new Guid("10000000-0000-0000-0000-000000000002"), true, "Derivado de", "Derivou", "derivado de", new Guid("00000000-0000-0000-0000-000000000000") },
                    { new Guid("10000000-0000-0000-0000-000000000003"), true, "Referenciado em", "Referencia", "referenciado em", new Guid("00000000-0000-0000-0000-000000000000") },
                    { new Guid("10000000-0000-0000-0000-000000000004"), true, "Adaptação de", "Adaptado em", "adaptação de", new Guid("00000000-0000-0000-0000-000000000000") },
                    { new Guid("10000000-0000-0000-0000-000000000005"), true, "Alternativa a", "Alternativa a", "alternativa a", new Guid("00000000-0000-0000-0000-000000000000") },
                    { new Guid("10000000-0000-0000-0000-000000000006"), true, "Do mesmo tipo que", "Do mesmo tipo que", "do mesmo tipo que", new Guid("00000000-0000-0000-0000-000000000000") },
                    { new Guid("10000000-0000-0000-0000-000000000007"), true, "Complemento de", "Complementado por", "complemento de", new Guid("00000000-0000-0000-0000-000000000000") },
                    { new Guid("10000000-0000-0000-0000-000000000008"), true, "Pré-requisito para", "Requer", "pré-requisito para", new Guid("00000000-0000-0000-0000-000000000000") },
                    { new Guid("10000000-0000-0000-0000-000000000009"), true, "Contém", "Parte de", "contém", new Guid("00000000-0000-0000-0000-000000000000") }
                });

            migrationBuilder.CreateIndex(
                name: "idx_conteudos_usuario_is_filho",
                table: "conteudos",
                columns: new[] { "usuario_id", "is_filho" });

            migrationBuilder.CreateIndex(
                name: "IX_conteudo_categorias_categoria_id",
                table: "conteudo_categorias",
                column: "categoria_id");

            migrationBuilder.CreateIndex(
                name: "idx_relacoes_origem_destino_tipo_unique",
                table: "relacoes",
                columns: new[] { "conteudo_origem_id", "conteudo_destino_id", "tipo_relacao_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_relacoes_origem_usuario",
                table: "relacoes",
                columns: new[] { "conteudo_origem_id", "usuario_id" });

            migrationBuilder.CreateIndex(
                name: "idx_relacoes_par_id",
                table: "relacoes",
                column: "par_id");

            migrationBuilder.CreateIndex(
                name: "IX_relacoes_conteudo_destino_id",
                table: "relacoes",
                column: "conteudo_destino_id");

            migrationBuilder.CreateIndex(
                name: "IX_relacoes_tipo_relacao_id",
                table: "relacoes",
                column: "tipo_relacao_id");

            migrationBuilder.CreateIndex(
                name: "idx_tipo_relacoes_usuario_nome_unique",
                table: "tipo_relacoes",
                columns: new[] { "usuario_id", "nome_normalizado" },
                unique: true);

            // CHECK constraint: prevent self-referencing relations at DB level
            migrationBuilder.Sql(
                "ALTER TABLE relacoes ADD CONSTRAINT chk_relacao_sem_auto_referencia " +
                "CHECK (conteudo_origem_id != conteudo_destino_id);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "conteudo_categorias");

            migrationBuilder.DropTable(
                name: "relacoes");

            migrationBuilder.DropTable(
                name: "tipo_relacoes");

            migrationBuilder.DropIndex(
                name: "idx_conteudos_usuario_is_filho",
                table: "conteudos");

            migrationBuilder.DropColumn(
                name: "classificacao",
                table: "conteudos");

            migrationBuilder.DropColumn(
                name: "is_filho",
                table: "conteudos");

            migrationBuilder.DropColumn(
                name: "total_esperado_sessoes",
                table: "conteudos");

            // Drop CHECK constraint added in Up()
            migrationBuilder.Sql(
                "ALTER TABLE relacoes DROP CONSTRAINT IF EXISTS chk_relacao_sem_auto_referencia;");
        }
    }
}
