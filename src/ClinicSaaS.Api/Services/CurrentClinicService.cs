using System.Security.Claims;

namespace ClinicSaaS.Api.Services;

public class CurrentClinicService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentClinicService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? GetClinicId()
    {
        var user = _httpContextAccessor.HttpContext?.User;

        if (user?.Identity?.IsAuthenticated != true)
            return null;

        var clinicIdValue = user.FindFirst("clinic_id")?.Value;

        if (string.IsNullOrWhiteSpace(clinicIdValue))
            return null;

        return Guid.TryParse(clinicIdValue, out var clinicId)
            ? clinicId
            : null;
    }

    public string? GetUserId()
    {
        var user = _httpContextAccessor.HttpContext?.User;

        if (user?.Identity?.IsAuthenticated != true)
            return null;

        return user.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? user.FindFirst("sub")?.Value;
    }

    public string? GetEmail()
    {
        var user = _httpContextAccessor.HttpContext?.User;

        if (user?.Identity?.IsAuthenticated != true)
            return null;

        return user.FindFirst(ClaimTypes.Email)?.Value
            ?? user.FindFirst("email")?.Value;
    }

    public string? GetRole()
    {
        var user = _httpContextAccessor.HttpContext?.User;

        if (user?.Identity?.IsAuthenticated != true)
            return null;

        return user.FindFirst(ClaimTypes.Role)?.Value;
    }
}