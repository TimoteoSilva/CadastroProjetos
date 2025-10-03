using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Falcare.Projetos.App.Services;

public class HttpUserContext : IUserContext
{
    private readonly IHttpContextAccessor _http;

    public HttpUserContext(IHttpContextAccessor http) => _http = http;

    public string? UserId =>
        _http.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
}
