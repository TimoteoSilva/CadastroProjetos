using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Falcare.Projetos.App.Models
{
    [Index(nameof(Nome), IsUnique = true)]
    public class TipoEquipamento
    {
        public int TipoEquipamentoId { get; set; }

        [Required, StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        // navegação inversa (N:N)
        public List<Projeto> Projetos { get; set; } = new();
    }
}
