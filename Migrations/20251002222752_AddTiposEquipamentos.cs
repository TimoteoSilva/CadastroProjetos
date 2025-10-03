using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Falcare.Projetos.App.Migrations
{
    /// <inheritdoc />
    public partial class AddTiposEquipamentos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TiposEquipamentos",
                columns: table => new
                {
                    TipoEquipamentoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposEquipamentos", x => x.TipoEquipamentoId);
                });

            migrationBuilder.CreateTable(
                name: "ProjetoEquipamentos",
                columns: table => new
                {
                    ProjetoId = table.Column<int>(type: "int", nullable: false),
                    TipoEquipamentoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjetoEquipamentos", x => new { x.ProjetoId, x.TipoEquipamentoId });
                    table.ForeignKey(
                        name: "FK_ProjetoEquipamentos_Projetos_ProjetoId",
                        column: x => x.ProjetoId,
                        principalTable: "Projetos",
                        principalColumn: "ProjetoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjetoEquipamentos_TiposEquipamentos_TipoEquipamentoId",
                        column: x => x.TipoEquipamentoId,
                        principalTable: "TiposEquipamentos",
                        principalColumn: "TipoEquipamentoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjetoEquipamentos_TipoEquipamentoId",
                table: "ProjetoEquipamentos",
                column: "TipoEquipamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_TiposEquipamentos_Nome",
                table: "TiposEquipamentos",
                column: "Nome",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjetoEquipamentos");

            migrationBuilder.DropTable(
                name: "TiposEquipamentos");
        }
    }
}
