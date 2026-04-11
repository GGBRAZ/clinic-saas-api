using ClinicSaaS.Api.Services;
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
    private readonly CurrentClinicService _currentClinicService;

    public PatientsController(
        ClinicSaaSDbContext context,
        CurrentClinicService currentClinicService)
    {
        _context = context;
        _currentClinicService = currentClinicService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PatientResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll()
    {
        var clinicId = _currentClinicService.GetClinicId();

        if (clinicId is null || clinicId == Guid.Empty)
            return BadRequest(new { message = "X-Clinic-Id header is required." });

        var patients = await _context.Patients
            .AsNoTracking()
            .Where(x => x.ClinicId == clinicId.Value)
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
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var clinicId = _currentClinicService.GetClinicId();

        if (clinicId is null || clinicId == Guid.Empty)
            return BadRequest(new { message = "X-Clinic-Id header is required." });

        var patient = await _context.Patients
            .AsNoTracking()
            .Where(x => x.Id == id && x.ClinicId == clinicId.Value)
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
        var clinicId = _currentClinicService.GetClinicId();

        if (clinicId is null || clinicId == Guid.Empty)
            return BadRequest(new { message = "X-Clinic-Id header is required." });

        if (request is null)
            return BadRequest(new { message = "Request body is required." });

        if (string.IsNullOrWhiteSpace(request.FullName) ||
            string.IsNullOrWhiteSpace(request.Phone))
        {
            return BadRequest(new
            {
                message = "fullName and phone are required."
            });
        }

        var clinicExists = await _context.Clinics
            .AnyAsync(x => x.Id == clinicId.Value);

        if (!clinicExists)
            return NotFound(new { message = "Clinic not found." });

        Patient patient;

        try
        {
            patient = new Patient(
                clinicId.Value,
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