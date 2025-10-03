using System.ComponentModel.DataAnnotations;

namespace Falcare.Projetos.App.Models
{
    public class ComponenteProjeto
    {
        public int ComponenteProjetoId { get; set; }

        [Required, StringLength(60)]
        public string Titulo { get; set; } = string.Empty;

        // Pode usar decimal com 2 casas para horas
        public decimal? HorasEstimadas { get; set; }

        // Moeda
        public decimal? CustoPrevisto { get; set; }

        // Data de entrega
        public DateOnly? DataEntrega { get; set; }

        // FK
        public int ProjetoId { get; set; }
        public Projeto? Projeto { get; set; }
    }
}
