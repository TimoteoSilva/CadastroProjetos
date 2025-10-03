using System.ComponentModel.DataAnnotations;

namespace Falcare.Projetos.App.Models;

public class HistoricoProjeto
{
    public int HistoricoProjetoId { get; set; }

    [Required]
    public int ProjetoId { get; set; }

    // Usuário (Identity) que gerou o evento
    public string? UsuarioId { get; set; }

    public DateTime Data { get; set; } = DateTime.UtcNow;

    [Required, StringLength(120)]
    public string Evento { get; set; } = string.Empty; // Ex.: "Status alterado para Negociação"

    [StringLength(1000)]
    public string? Observacao { get; set; }

    // Navegação
    public Projeto? Projeto { get; set; }
}

