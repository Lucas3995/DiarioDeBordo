using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiarioDeBordo.Persistencia.Migrations
{
    /// <inheritdoc />
    public partial class AddConteudoColetanea : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "conteudo_coletanea",
                columns: table => new
                {
                    coletanea_id = table.Column<Guid>(type: "uuid", nullable: false),
                    conteudo_id = table.Column<Guid>(type: "uuid", nullable: false),
                    posicao = table.Column<int>(type: "integer", nullable: false),
                    anotacao_contextual = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true),
                    adicionado_em = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_conteudo_coletanea", x => new { x.coletanea_id, x.conteudo_id });
                    table.ForeignKey(
                        name: "FK_conteudo_coletanea_conteudos_coletanea_id",
                        column: x => x.coletanea_id,
                        principalTable: "conteudos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_conteudo_coletanea_conteudos_conteudo_id",
                        column: x => x.conteudo_id,
                        principalTable: "conteudos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "idx_conteudo_coletanea_posicao",
                table: "conteudo_coletanea",
                columns: new[] { "coletanea_id", "posicao" });

            migrationBuilder.CreateIndex(
                name: "IX_conteudo_coletanea_conteudo_id",
                table: "conteudo_coletanea",
                column: "conteudo_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "conteudo_coletanea");
        }
    }
}
