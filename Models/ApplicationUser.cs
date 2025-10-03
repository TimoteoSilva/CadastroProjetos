using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Falcare.Projetos.App.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Nome para exibição (não confundir com UserName)
        [StringLength(160)]
        public string? Nome { get; set; }
    }
}
