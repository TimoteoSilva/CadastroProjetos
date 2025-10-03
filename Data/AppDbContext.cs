using Falcare.Projetos.App.Models;
using Falcare.Projetos.App.Services;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace Falcare.Projetos.App.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    private readonly IUserContext _userContext;
    private readonly IChangeNotes _changeNotes;
    public DbSet<TipoEquipamento> TiposEquipamentos { get; set; } = default!;
    public AppDbContext(DbContextOptions<AppDbContext> options,
                        IUserContext userContext,
                        IChangeNotes changeNotes)
        : base(options)
    {
        _userContext = userContext;
        _changeNotes = changeNotes;
    }

    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Projeto> Projetos => Set<Projeto>();
    public DbSet<StatusProjeto> Statuses => Set<StatusProjeto>();
    public DbSet<HistoricoProjeto> Historicos => Set<HistoricoProjeto>();
    public DbSet<ComponenteProjeto> ComponentesProjetos => Set<ComponenteProjeto>();
    public DbSet<LayoutCadastro> LayoutsCadastro => Set<LayoutCadastro>();
    protected override void OnModelCreating(ModelBuilder b)
    {
        var l = b.Entity<LayoutCadastro>();
        l.Property(x => x.Codigo).HasMaxLength(10).IsRequired();
        l.HasIndex(x => x.Codigo).IsUnique();

        l.Property(x => x.Titulo).HasMaxLength(60).IsRequired();
        l.Property(x => x.Tipo).HasMaxLength(3).IsRequired();

        l.Property(x => x.Data).HasColumnType("date");

        // (opcional) tira milissegundos
        l.Property(x => x.DataUltimaAtualizacao).HasPrecision(0);

        // FK para Projeto
        l.HasOne(x => x.Projeto)
         .WithMany() // sem navegação no Projeto (opcional)
         .HasForeignKey(x => x.ProjetoId)
         .OnDelete(DeleteBehavior.Cascade);

        // FK para Usuario (ApplicationUser)
        l.HasOne(x => x.Autor)
         .WithMany()
         .HasForeignKey(x => x.AutorUserId)
         .OnDelete(DeleteBehavior.Restrict);

        var c = b.Entity<ComponenteProjeto>();
        c.Property(x => x.Titulo).HasMaxLength(60).IsRequired();
        c.Property(x => x.HorasEstimadas).HasPrecision(9, 2);
        c.Property(x => x.CustoPrevisto).HasPrecision(18, 2);
        c.Property(x => x.DataEntrega).HasColumnType("date");

        c.HasOne(x => x.Projeto)
         .WithMany(p => p.Componentes)
         .HasForeignKey(x => x.ProjetoId)
         .OnDelete(DeleteBehavior.Cascade);

        base.OnModelCreating(b);
        b.Entity<Cliente>().HasIndex(x => x.RazaoSocial);
        b.Entity<Projeto>().HasIndex(x => new { x.ClienteId, x.StatusProjetoId });
        b.Entity<StatusProjeto>().HasIndex(x => x.Ordem).IsUnique(false);

        b.Entity<Projeto>()
            .HasOne(o => o.Cliente).WithMany(c => c.Projetos)
            .HasForeignKey(o => o.ClienteId).OnDelete(DeleteBehavior.Restrict);

        b.Entity<Projeto>()
            .HasOne(o => o.Status).WithMany()
            .HasForeignKey(o => o.StatusProjetoId).OnDelete(DeleteBehavior.Restrict);

        // Precisão do decimal para não truncar no SQL Server
        b.Entity<Projeto>()
            .Property(p => p.ValorEstimado)
            .HasPrecision(18, 2);   // ajuste se precisar mais casas/intervalo

        b.Entity<HistoricoProjeto>()
            .HasOne(h => h.Projeto).WithMany(o => o.Historicos)
            .HasForeignKey(h => h.ProjetoId).OnDelete(DeleteBehavior.Cascade);

        // Projeto - novos campos
        b.Entity<Projeto>()
         .Property(p => p.RefOrcamento)
         .HasMaxLength(20);

        b.Entity<Projeto>()
         .Property(p => p.DataInicio)
         .HasColumnType("date");

        b.Entity<Projeto>()
         .Property(p => p.DataEntrega)
         .HasColumnType("date");

        // Projeto <-> TipoEquipamento (N:N) com join "ProjetoEquipamentos"
        b.Entity<Projeto>()
         .HasMany(p => p.Equipamentos)
         .WithMany(t => t.Projetos)
         .UsingEntity<Dictionary<string, object>>(
             "ProjetoEquipamentos",
             j => j
                 .HasOne<TipoEquipamento>()
                 .WithMany()
                 .HasForeignKey("TipoEquipamentoId")
                 .OnDelete(DeleteBehavior.Cascade),
             j => j
                 .HasOne<Projeto>()
                 .WithMany()
                 .HasForeignKey("ProjetoId")
                 .OnDelete(DeleteBehavior.Cascade),
             j =>
             {
                 j.HasKey("ProjetoId", "TipoEquipamentoId");
                 j.ToTable("ProjetoEquipamentos");
             }
         );

    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var userId = _userContext.UserId;

        // Cache de nomes de status (sem rastreamento)
        var statusNames = await Statuses.AsNoTracking()
            .ToDictionaryAsync(s => s.StatusProjetoId, s => s.Nome, cancellationToken);

        // Snapshot para não modificar coleção enquanto enumera
        var entries = ChangeTracker.Entries<Projeto>().ToList();

        var pendentes = new List<HistoricoProjeto>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.DataCriacao = now;
                entry.Entity.DataUltimaAtualizacao = now;

                pendentes.Add(new HistoricoProjeto
                {
                    Projeto = entry.Entity,
                    UsuarioId = userId,
                    Data = now,
                    Evento = "Orçamento criado"
                });
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.DataUltimaAtualizacao = now;

                // Troca de status
                var statusProp = entry.Property(o => o.StatusProjetoId);
                if (statusProp.IsModified)
                {
                    var antigoId = (int)statusProp.OriginalValue!;
                    var novoId = (int)statusProp.CurrentValue!;

                    var nomeAntigo = statusNames.TryGetValue(antigoId, out var nA) ? nA : antigoId.ToString();
                    var nomeNovo = statusNames.TryGetValue(novoId, out var nN) ? nN : novoId.ToString();

                    var obs = _changeNotes.PopStatusNote(entry.Entity.ProjetoId);

                    pendentes.Add(new HistoricoProjeto
                    {
                        ProjetoId = entry.Entity.ProjetoId,
                        UsuarioId = userId,
                        Data = now,
                        Evento = $"Status alterado: {nomeAntigo} → {nomeNovo}",
                        Observacao = string.IsNullOrWhiteSpace(obs) ? null : obs
                    });
                }

                // Troca de responsável
                if (entry.Property(o => o.ResponsavelUserId).IsModified)
                {
                    pendentes.Add(new HistoricoProjeto
                    {
                        ProjetoId = entry.Entity.ProjetoId,
                        UsuarioId = userId,
                        Data = now,
                        Evento = "Responsável alterado"
                    });
                }
            }
        }

        // 1ª gravação (projetos)
        var affected = await base.SaveChangesAsync(cancellationToken);

        // 2ª gravação (histórico)
        if (pendentes.Count > 0)
        {
            Historicos.AddRange(pendentes);
            affected += await base.SaveChangesAsync(cancellationToken);
        }

        return affected;
    }

}
