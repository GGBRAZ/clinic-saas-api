using ClinicSaaS.Application.Patients.Dtos;
using ClinicSaaS.Domain.Entities;
using ClinicSaaS.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicSaaS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    private readonly ClinicSaaSDbContext _context;

    public PatientsController(ClinicSaaSDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PatientResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] Guid? clinicId = null)
    {
        var query = _context.Patients.AsNoTracking().AsQueryable();

        if (clinicId.HasValue && clinicId.Value != Guid.Empty)
            query = query.Where(x => x.ClinicId == clinicId.Value);

        var patients = await query
            .OrderBy(x => x.FullName)
            .Select(x => new PatientResponse
            {
                Id = x.Id,
                ClinicId = x.ClinicId,
                FullName = x.FullName,
                Phone = x.Phone,
                Email = x.Email,
                BirthDate = x.BirthDate,
                Notes = x.Notes,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();

        return Ok(patients);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PatientResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var patient = await _context.Patients
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new PatientResponse
            {
                Id = x.Id,
                ClinicId = x.ClinicId,
                FullName = x.FullName,
                Phone = x.Phone,
                Email = x.Email,
                BirthDate = x.BirthDate,
                Notes = x.Notes,
                CreatedAt = x.CreatedAt
            })
            .FirstOrDefaultAsync();

        if (patient is null)
            return NotFound(new { message = "Patient not found." });

        return Ok(patient);
    }

    [HttpPost]
    [ProducesResponseType(typeof(PatientResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CreatePatientRequest request)
    {
        if (request is null)
            return BadRequest(new { message = "Request body is required." });

        if (request.ClinicId == Guid.Empty ||
            string.IsNullOrWhiteSpace(request.FullName) ||
            string.IsNullOrWhiteSpace(request.Phone))
        {
            return BadRequest(new
            {
                message = "ClinicId, fullName, and phone are required."
            });
        }

        var clinicExists = await _context.Clinics.AnyAsync(x => x.Id == request.ClinicId);

        if (!clinicExists)
            return NotFound(new { message = "Clinic not found." });

        Patient patient;

        try
        {
            patient = new Patient(
                request.ClinicId,
                request.FullName,
                request.Phone,
                request.Email,
                request.BirthDate,
                request.Notes
            );
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }

        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();

        var response = new PatientResponse
        {
            Id = patient.Id,
            ClinicId = patient.ClinicId,
            FullName = patient.FullName,
            Phone = patient.Phone,
            Email = patient.Email,
            BirthDate = patient.BirthDate,
            Notes = patient.Notes,
            CreatedAt = patient.CreatedAt
        };

        return CreatedAtAction(nameof(GetById), new { id = patient.Id }, response);
    }
}