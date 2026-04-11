using Microsoft.AspNetCore.Http;


namespace ClinicSaaS.Api.Services;

public class CurrentClinicService
{
    private const string HeaderName = "X-Clinic-Id";
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentClinicService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? GetClinicId()
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext is null)
            return null;

        if (!httpContext.Request.Headers.TryGetValue(HeaderName, out var values))
            return null;

        var rawValue = values.FirstOrDefault();

        if (string.IsNullOrWhiteSpace(rawValue))
            return null;

        if (!Guid.TryParse(rawValue, out var clinicId))
            return null;

        return clinicId;
    }
}