using System.ComponentModel.DataAnnotations;

namespace Falcare.Projetos.App.Models;

public class StatusProjeto
{
    public int StatusProjetoId { get; set; }

    [Required, StringLength(60)]
    public string Nome { get; set; } = string.Empty; // Em elaboração, Enviado, Negociação, Ganhou, Perdeu, Suspenso

    public int Ordem { get; set; }
}

