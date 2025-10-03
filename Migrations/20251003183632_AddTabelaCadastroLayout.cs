using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Falcare.Projetos.App.Migrations
{
    /// <inheritdoc />
    public partial class AddTabelaCadastroLayout : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LayoutsCadastro",
                columns: table => new
                {
                    LayoutCadastroId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ProjetoId = table.Column<int>(type: "int", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    AutorUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Data = table.Column<DateOnly>(type: "date", nullable: false),
                    DataUltimaAtualizacao = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LayoutsCadastro", x => x.LayoutCadastroId);
                    table.ForeignKey(
                        name: "FK_LayoutsCadastro_AspNetUsers_AutorUserId",
                        column: x => x.AutorUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LayoutsCadastro_Projetos_ProjetoId",
                        column: x => x.ProjetoId,
                        principalTable: "Projetos",
                        principalColumn: "ProjetoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LayoutsCadastro_AutorUserId",
                table: "LayoutsCadastro",
                column: "AutorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_LayoutsCadastro_Codigo",
                table: "LayoutsCadastro",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LayoutsCadastro_ProjetoId",
                table: "LayoutsCadastro",
                column: "ProjetoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LayoutsCadastro");
        }
    }
}
