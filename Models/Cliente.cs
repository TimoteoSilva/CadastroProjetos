using System.ComponentModel.DataAnnotations;

namespace Falcare.Projetos.App.Models;

public class Cliente
{
    public int ClienteId { get; set; }

    [Required, StringLength(160)]
    public string RazaoSocial { get; set; } = string.Empty;

    [Required, StringLength(20)]
    public string? Cnpj { get; set; } = string.Empty;

    // JSON opcional para contatos (telefone, e-mail, etc.)
    public string? ContatosJson { get; set; }

    public bool Ativo { get; set; } = true;

    // Navegação
    public ICollection<Projeto> Projetos { get; set; } = new List<Projeto>();
}

