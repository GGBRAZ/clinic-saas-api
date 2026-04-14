using ClinicSaaS.Api.Services;
using ClinicSaaS.Application.Appointments.Dtos;
using ClinicSaaS.Domain.Entities;
using ClinicSaaS.Domain.Enums;
using ClinicSaaS.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace ClinicSaaS.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly ClinicSaaSDbContext _context;
    private readonly CurrentClinicService _currentClinicService;

    public AppointmentsController(
        ClinicSaaSDbContext context,
        CurrentClinicService currentClinicService)
    {
        _context = context;
        _currentClinicService = currentClinicService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AppointmentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll([FromQuery] Guid? patientId = null)
    {
        var clinicId = _currentClinicService.GetClinicId();

        if (clinicId is null || clinicId == Guid.Empty)
            return Unauthorized(new { message = "Clinic context was not found in the authenticated token." });

        var query = _context.Appointments
            .AsNoTracking()
            .Where(x => x.ClinicId == clinicId.Value)
            .AsQueryable();

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
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var clinicId = _currentClinicService.GetClinicId();

        if (clinicId is null || clinicId == Guid.Empty)
            return BadRequest(new { message = "X-Clinic-Id header is required." });

        var appointment = await _context.Appointments
            .AsNoTracking()
            .Where(x => x.Id == id && x.ClinicId == clinicId.Value)
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

    [HttpGet("{id:guid}/history")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetHistory(Guid id)
    {
        var clinicId = _currentClinicService.GetClinicId();

        if (clinicId is null || clinicId == Guid.Empty)
            return BadRequest(new { message = "X-Clinic-Id header is required." });

        var appointmentExists = await _context.Appointments
            .AnyAsync(x => x.Id == id && x.ClinicId == clinicId.Value);

        if (!appointmentExists)
            return NotFound(new { message = "Appointment not found." });

        var history = await _context.AppointmentHistories
            .AsNoTracking()
            .Where(x => x.AppointmentId == id)
            .OrderBy(x => x.CreatedAt)
            .Select(x => new
            {
                x.Id,
                x.AppointmentId,
                x.Action,
                x.OldStatus,
                x.NewStatus,
                x.OldDate,
                x.NewDate,
                x.OldStartTime,
                x.NewStartTime,
                x.OldEndTime,
                x.NewEndTime,
                x.Notes,
                x.CreatedAt
            })
            .ToListAsync();

        return Ok(history);
    }

    [HttpPost]
    [ProducesResponseType(typeof(AppointmentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CreateAppointmentRequest request)
    {
        var clinicId = _currentClinicService.GetClinicId();

        if (clinicId is null || clinicId == Guid.Empty)
            return BadRequest(new { message = "X-Clinic-Id header is required." });

        if (request is null)
            return BadRequest(new { message = "Request body is required." });

        if (request.PatientId == Guid.Empty || request.ExpectedAmount < 0)
        {
            return BadRequest(new
            {
                message = "PatientId and a valid ExpectedAmount are required."
            });
        }

        var clinicExists = await _context.Clinics
            .AnyAsync(x => x.Id == clinicId.Value);

        if (!clinicExists)
            return NotFound(new { message = "Clinic not found." });

        var patientExists = await _context.Patients
            .AnyAsync(x => x.Id == request.PatientId && x.ClinicId == clinicId.Value);

        if (!patientExists)
            return NotFound(new { message = "Patient not found for this clinic." });

        Appointment appointment;

        try
        {
            appointment = new Appointment(
                clinicId.Value,
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

        var history = new AppointmentHistory(
            appointment.Id,
            AppointmentHistoryAction.Created,
            oldStatus: null,
            newStatus: appointment.Status,
            oldDate: null,
            newDate: appointment.Date,
            oldStartTime: null,
            newStartTime: appointment.StartTime,
            oldEndTime: null,
            newEndTime: appointment.EndTime,
            notes: "Appointment created."
        );

        _context.AppointmentHistories.Add(history);

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

    [HttpPatch("{id:guid}/reschedule")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Reschedule(Guid id, [FromBody] RescheduleAppointmentRequest request)
    {
        var clinicId = _currentClinicService.GetClinicId();

        if (clinicId is null || clinicId == Guid.Empty)
            return BadRequest(new { message = "X-Clinic-Id header is required." });

        if (request is null)
            return BadRequest(new { message = "Request body is required." });

        var appointment = await _context.Appointments
            .FirstOrDefaultAsync(x => x.Id == id && x.ClinicId == clinicId.Value);

        if (appointment is null)
            return NotFound(new { message = "Appointment not found." });

        var oldDate = appointment.Date;
        var oldStartTime = appointment.StartTime;
        var oldEndTime = appointment.EndTime;
        var oldStatus = appointment.Status;

        try
        {
            appointment.Reschedule(request.Date, request.StartTime, request.EndTime);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }

        var history = new AppointmentHistory(
            appointment.Id,
            AppointmentHistoryAction.Rescheduled,
            oldStatus: oldStatus,
            newStatus: appointment.Status,
            oldDate: oldDate,
            newDate: appointment.Date,
            oldStartTime: oldStartTime,
            newStartTime: appointment.StartTime,
            oldEndTime: oldEndTime,
            newEndTime: appointment.EndTime,
            notes: request.Notes
        );

        _context.AppointmentHistories.Add(history);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Appointment rescheduled successfully." });
    }

    [HttpPatch("{id:guid}/complete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkAsCompleted(Guid id)
    {
        var clinicId = _currentClinicService.GetClinicId();

        if (clinicId is null || clinicId == Guid.Empty)
            return BadRequest(new { message = "X-Clinic-Id header is required." });

        var appointment = await _context.Appointments
            .FirstOrDefaultAsync(x => x.Id == id && x.ClinicId == clinicId.Value);

        if (appointment is null)
            return NotFound(new { message = "Appointment not found." });

        var oldStatus = appointment.Status;

        appointment.MarkAsCompleted();

        var history = new AppointmentHistory(
            appointment.Id,
            AppointmentHistoryAction.Completed,
            oldStatus: oldStatus,
            newStatus: appointment.Status,
            oldDate: appointment.Date,
            newDate: appointment.Date,
            oldStartTime: appointment.StartTime,
            newStartTime: appointment.StartTime,
            oldEndTime: appointment.EndTime,
            newEndTime: appointment.EndTime,
            notes: "Appointment marked as completed."
        );

        _context.AppointmentHistories.Add(history);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Appointment marked as completed." });
    }

    [HttpPatch("{id:guid}/noshow")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkAsNoShow(Guid id)
    {
        var clinicId = _currentClinicService.GetClinicId();

        if (clinicId is null || clinicId == Guid.Empty)
            return BadRequest(new { message = "X-Clinic-Id header is required." });

        var appointment = await _context.Appointments
            .FirstOrDefaultAsync(x => x.Id == id && x.ClinicId == clinicId.Value);

        if (appointment is null)
            return NotFound(new { message = "Appointment not found." });

        var oldStatus = appointment.Status;

        appointment.MarkAsNoShow();

        var history = new AppointmentHistory(
            appointment.Id,
            AppointmentHistoryAction.NoShow,
            oldStatus: oldStatus,
            newStatus: appointment.Status,
            oldDate: appointment.Date,
            newDate: appointment.Date,
            oldStartTime: appointment.StartTime,
            newStartTime: appointment.StartTime,
            oldEndTime: appointment.EndTime,
            newEndTime: appointment.EndTime,
            notes: "Appointment marked as no-show."
        );

        _context.AppointmentHistories.Add(history);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Appointment marked as no-show." });
    }

    [HttpPatch("{id:guid}/cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkAsCanceled(Guid id)
    {
        var clinicId = _currentClinicService.GetClinicId();

        if (clinicId is null || clinicId == Guid.Empty)
            return BadRequest(new { message = "X-Clinic-Id header is required." });

        var appointment = await _context.Appointments
            .FirstOrDefaultAsync(x => x.Id == id && x.ClinicId == clinicId.Value);

        if (appointment is null)
            return NotFound(new { message = "Appointment not found." });

        var oldStatus = appointment.Status;

        appointment.MarkAsCanceled();

        var history = new AppointmentHistory(
            appointment.Id,
            AppointmentHistoryAction.Canceled,
            oldStatus: oldStatus,
            newStatus: appointment.Status,
            oldDate: appointment.Date,
            newDate: appointment.Date,
            oldStartTime: appointment.StartTime,
            newStartTime: appointment.StartTime,
            oldEndTime: appointment.EndTime,
            newEndTime: appointment.EndTime,
            notes: "Appointment marked as canceled."
        );

        _context.AppointmentHistories.Add(history);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Appointment marked as canceled." });
    }
}