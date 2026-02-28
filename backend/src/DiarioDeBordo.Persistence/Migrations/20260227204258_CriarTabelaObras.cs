using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiarioDeBordo.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CriarTabelaObras : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Obras",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Tipo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PosicaoAtual = table.Column<int>(type: "integer", nullable: false),
                    DataUltimaAtualizacaoPosicao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProximaInfoTipo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DiasAteProximaParte = table.Column<int>(type: "integer", nullable: true),
                    PartesJaPublicadas = table.Column<int>(type: "integer", nullable: true),
                    OrdemPreferencia = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Obras", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Obras");
        }
    }
}
