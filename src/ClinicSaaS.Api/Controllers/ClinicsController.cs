using ClinicSaaS.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicSaaS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClinicsController : ControllerBase
{
    private readonly ClinicSaaSDbContext _context;

    public ClinicsController(ClinicSaaSDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var clinics = await _context.Clinics.ToListAsync();
        return Ok(clinics);
    }
}