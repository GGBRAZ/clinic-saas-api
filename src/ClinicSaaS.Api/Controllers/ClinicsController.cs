using ClinicSaaS.Application.Clinics.Dtos;
using ClinicSaaS.Domain.Entities;
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
    [ProducesResponseType(typeof(IEnumerable<ClinicResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var clinics = await _context.Clinics
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new ClinicResponse
            {
                Id = x.Id,
                Name = x.Name,
                Slug = x.Slug,
                Email = x.Email,
                Phone = x.Phone,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();

        return Ok(clinics);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ClinicResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var clinic = await _context.Clinics
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new ClinicResponse
            {
                Id = x.Id,
                Name = x.Name,
                Slug = x.Slug,
                Email = x.Email,
                Phone = x.Phone,
                CreatedAt = x.CreatedAt
            })
            .FirstOrDefaultAsync();

        if (clinic is null)
            return NotFound(new { message = "Clinic not found." });

        return Ok(clinic);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ClinicResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateClinicRequest request)
    {
        if (request is null)
            return BadRequest(new { message = "Request body is required." });

        if (string.IsNullOrWhiteSpace(request.Name) ||
            string.IsNullOrWhiteSpace(request.Slug) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Phone))
        {
            return BadRequest(new
            {
                message = "Name, slug, email, and phone are required."
            });
        }

        var normalizedSlug = request.Slug.Trim().ToLowerInvariant();

        var slugAlreadyExists = await _context.Clinics
            .AnyAsync(x => x.Slug == normalizedSlug);

        if (slugAlreadyExists)
        {
            return Conflict(new
            {
                message = "A clinic with this slug already exists."
            });
        }

        Clinic clinic;

        try
        {
            clinic = new Clinic(
                request.Name,
                request.Slug,
                request.Email,
                request.Phone
            );
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }

        _context.Clinics.Add(clinic);
        await _context.SaveChangesAsync();

        var response = new ClinicResponse
        {
            Id = clinic.Id,
            Name = clinic.Name,
            Slug = clinic.Slug,
            Email = clinic.Email,
            Phone = clinic.Phone,
            CreatedAt = clinic.CreatedAt
        };

        return CreatedAtAction(nameof(GetById), new { id = clinic.Id }, response);
    }
}