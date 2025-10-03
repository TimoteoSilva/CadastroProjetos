using System;
using System.ComponentModel.DataAnnotations;

namespace Falcare.Projetos.App.Models
{
    public class LayoutCadastro
    {
        public int LayoutCadastroId { get; set; }

        [Required, StringLength(10)]
        public string Codigo { get; set; } = string.Empty; // FAC-L0000

        // "Ordem" = Projeto selecionado
        [Required]
        public int ProjetoId { get; set; }
        public Projeto? Projeto { get; set; }

        [Required, StringLength(60)]
        public string Titulo { get; set; } = string.Empty;

        [Required, StringLength(3)]
        public string Tipo { get; set; } = "2D"; // 2D ou 3D

        // Autor (usuário)
        [Required]
        public string AutorUserId { get; set; } = string.Empty;
        public ApplicationUser? Autor { get; set; }

        [Required]
        public DateOnly Data { get; set; }

        public DateTime DataUltimaAtualizacao { get; set; } = DateTime.UtcNow;
    }
}

