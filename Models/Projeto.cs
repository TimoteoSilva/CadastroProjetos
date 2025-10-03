using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Falcare.Projetos.App.Models; // (se Projeto.cs está em outro namespace)
namespace Falcare.Projetos.App.Models;
using System.ComponentModel.DataAnnotations;

public class Projeto
{
    public int ProjetoId { get; set; }

    private string _ordem = string.Empty;
    public List<TipoEquipamento> Equipamentos { get; set; } = new();
    public List<ComponenteProjeto> Componentes { get; set; } = new();

    [StringLength(20)]
    public string? RefOrcamento { get; set; }

    // .NET 8/EF Core 9: DateOnly é suportado (vamos mapear como 'date' no SQL Server)
    public DateOnly? DataInicio { get; set; }
    public DateOnly? DataEntrega { get; set; }

    [Required, StringLength(6)]
    [RegularExpression(@"^FA\d{4}$", ErrorMessage = "O campo Ordem deve estar no formato FA0000 (FA + 4 dígitos).")]
    public string Ordem
    {
        get => _ordem;
        set => _ordem = (value ?? string.Empty).ToUpperInvariant();
    }

    [Required]
    public int ClienteId { get; set; }

    [StringLength(50)]
    public string? CodigoInterno { get; set; }

    [Required, StringLength(160)]
    public string Titulo { get; set; } = string.Empty;

    public decimal? ValorEstimado { get; set; }

    [Required]
    public int StatusProjetoId { get; set; }

    // Relacionar com Identity (quem é o responsável atual)
    public string? ResponsavelUserId { get; set; }

    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public DateTime DataUltimaAtualizacao { get; set; } = DateTime.UtcNow;

    // Navegação
    public Cliente? Cliente { get; set; }
    public StatusProjeto? Status { get; set; }
    public ICollection<HistoricoProjeto> Historicos { get; set; } = new List<HistoricoProjeto>();
}

