using ClinicSaaS.Application.Appointments.Dtos;
using ClinicSaaS.Domain.Entities;
using ClinicSaaS.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicSaaS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly ClinicSaaSDbContext _context;

    public AppointmentsController(ClinicSaaSDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AppointmentResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] Guid? clinicId = null, [FromQuery] Guid? patientId = null)
    {
        var query = _context.Appointments
            .AsNoTracking()
            .AsQueryable();

        if (clinicId.HasValue && clinicId.Value != Guid.Empty)
            query = query.Where(x => x.ClinicId == clinicId.Value);

        if (patientId.HasValue && patientId.Value != Guid.Empty)
            query = query.Where(x => x.PatientId == patientId.Value);

        var appointments = await query
            .OrderBy(x => x.Date)
            .ThenBy(x => x.StartTime)
            .Select(x => new AppointmentResponse
            {
                Id = x.Id,
                ClinicId = x.ClinicId,
                PatientId = x.PatientId,
                Date = x.Date,
                StartTime = x.StartTime,
                EndTime = x.EndTime,
                ExpectedAmount = x.ExpectedAmount,
                Status = x.Status,
                Notes = x.Notes,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();

        return Ok(appointments);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AppointmentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var appointment = await _context.Appointments
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new AppointmentResponse
            {
                Id = x.Id,
                ClinicId = x.ClinicId,
                PatientId = x.PatientId,
                Date = x.Date,
                StartTime = x.StartTime,
                EndTime = x.EndTime,
                ExpectedAmount = x.ExpectedAmount,
                Status = x.Status,
                Notes = x.Notes,
                CreatedAt = x.CreatedAt
            })
            .FirstOrDefaultAsync();

        if (appointment is null)
            return NotFound(new { message = "Appointment not found." });

        return Ok(appointment);
    }

    [HttpPost]
    [ProducesResponseType(typeof(AppointmentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CreateAppointmentRequest request)
    {
        if (request is null)
            return BadRequest(new { message = "Request body is required." });

        if (request.ClinicId == Guid.Empty ||
            request.PatientId == Guid.Empty ||
            request.ExpectedAmount < 0)
        {
            return BadRequest(new
            {
                message = "ClinicId, PatientId, and a valid ExpectedAmount are required."
            });
        }

        var clinicExists = await _context.Clinics
            .AnyAsync(x => x.Id == request.ClinicId);

        if (!clinicExists)
            return NotFound(new { message = "Clinic not found." });

        var patientExists = await _context.Patients
            .AnyAsync(x => x.Id == request.PatientId && x.ClinicId == request.ClinicId);

        if (!patientExists)
            return NotFound(new { message = "Patient not found for this clinic." });

        Appointment appointment;

        try
        {
            appointment = new Appointment(
                request.ClinicId,
                request.PatientId,
                request.Date,
                request.StartTime,
                request.EndTime,
                request.ExpectedAmount,
                request.Notes
            );
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }

        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();

        var response = new AppointmentResponse
        {
            Id = appointment.Id,
            ClinicId = appointment.ClinicId,
            PatientId = appointment.PatientId,
            Date = appointment.Date,
            StartTime = appointment.StartTime,
            EndTime = appointment.EndTime,
            ExpectedAmount = appointment.ExpectedAmount,
            Status = appointment.Status,
            Notes = appointment.Notes,
            CreatedAt = appointment.CreatedAt
        };

        return CreatedAtAction(nameof(GetById), new { id = appointment.Id }, response);
    }

    [HttpPatch("{id:guid}/complete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkAsCompleted(Guid id)
    {
        var appointment = await _context.Appointments.FirstOrDefaultAsync(x => x.Id == id);

        if (appointment is null)
            return NotFound(new { message = "Appointment not found." });

        appointment.MarkAsCompleted();
        await _context.SaveChangesAsync();

        return Ok(new { message = "Appointment marked as completed." });
    }

    [HttpPatch("{id:guid}/noshow")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkAsNoShow(Guid id)
    {
        var appointment = await _context.Appointments.FirstOrDefaultAsync(x => x.Id == id);

        if (appointment is null)
            return NotFound(new { message = "Appointment not found." });

        appointment.MarkAsNoShow();
        await _context.SaveChangesAsync();

        return Ok(new { message = "Appointment marked as no-show." });
    }

    [HttpPatch("{id:guid}/cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkAsCanceled(Guid id)
    {
        var appointment = await _context.Appointments.FirstOrDefaultAsync(x => x.Id == id);

        if (appointment is null)
            return NotFound(new { message = "Appointment not found." });

        appointment.MarkAsCanceled();
        await _context.SaveChangesAsync();

        return Ok(new { message = "Appointment marked as canceled." });
    }
}