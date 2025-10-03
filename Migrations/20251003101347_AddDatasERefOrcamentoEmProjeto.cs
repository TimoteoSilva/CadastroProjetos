using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Falcare.Projetos.App.Migrations
{
    /// <inheritdoc />
    public partial class AddDatasERefOrcamentoEmProjeto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "DataEntrega",
                table: "Projetos",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "DataInicio",
                table: "Projetos",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefOrcamento",
                table: "Projetos",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataEntrega",
                table: "Projetos");

            migrationBuilder.DropColumn(
                name: "DataInicio",
                table: "Projetos");

            migrationBuilder.DropColumn(
                name: "RefOrcamento",
                table: "Projetos");
        }
    }
}
