using Falcare.Projetos.App.Data;
using Falcare.Projetos.App.Models;
using Falcare.Projetos.App.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// DB SQL Express
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                      ?? "Server=.\\SQLEXPRESS;Database=FalcareProjetos;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Identity (usuário/roles padrão)
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    // Login sem exigir confirmação de e-mail (mude para true se quiser exigir)
    options.SignIn.RequireConfirmedAccount = false;

    // E-mail único
    options.User.RequireUniqueEmail = true;

    // Regras de senha (ajuste conforme sua política)
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = true;
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserContext, HttpUserContext>();
builder.Services.AddScoped<IChangeNotes, ChangeNotes>();

var app = builder.Build();

// Migrar banco e fazer seed básico
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
    var desejados = new (string Nome, int Ordem)[]
    {
        ("Engenharia", 1),
        ("Montagem",   2),
        ("Garantia",   3),
        ("Finalizado", 4),
    };
    var existentes = await db.Statuses.ToListAsync();
    foreach (var (nome, ordem) in desejados)
    {
        var s = existentes.FirstOrDefault(x => x.Nome == nome);
        if (s is null)
        {
            db.Statuses.Add(new StatusProjeto { Nome = nome, Ordem = ordem });
        }
        else
        {
            if (s.Ordem != ordem)
            {
                s.Ordem = ordem;
                db.Statuses.Update(s);
            }
        }
    }
    await db.SaveChangesAsync();


    // Seed de Status
    if (!db.Statuses.Any())
    {
        db.Statuses.AddRange(
            new StatusProjeto { Nome = "Em elaboração", Ordem = 1 },
            new StatusProjeto { Nome = "Enviado", Ordem = 2 },
            new StatusProjeto { Nome = "Negociação", Ordem = 3 },
            new StatusProjeto { Nome = "Ganhou", Ordem = 4 },
            new StatusProjeto { Nome = "Perdeu", Ordem = 5 },
            new StatusProjeto { Nome = "Suspenso", Ordem = 6 }
        );
        await db.SaveChangesAsync();
    }

    // Seed de Roles + Admin
    var roleMgr = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userMgr = services.GetRequiredService<UserManager<ApplicationUser>>();

    foreach (var r in new[] { "Admin", "Diretoria", "Comercial", "PM" })
        if (!await roleMgr.RoleExistsAsync(r))
            await roleMgr.CreateAsync(new IdentityRole(r));

    var adminEmail = "admin@falcare.local";
    var admin = await userMgr.FindByEmailAsync(adminEmail);
    if (admin == null)
    {
        admin = new ApplicationUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
        // TODO: troque a senha padrão após o primeiro login
        await userMgr.CreateAsync(admin, "Falcare@123");
        await userMgr.AddToRoleAsync(admin, "Admin");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
