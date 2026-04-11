using ClinicSaaS.Application.Dashboard.Dtos;
using ClinicSaaS.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClinicSaaS.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
public class DashboardController : ControllerBase
{
    private readonly DashboardService _service;

    public DashboardController(DashboardService service)
    {
        _service = service;
    }

    [HttpGet("financial")]
    [ProducesResponseType(typeof(FinancialDashboardDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFinancial([FromQuery] Guid clinicId)
    {
        if (clinicId == Guid.Empty)
            return BadRequest(new { message = "clinicId is required." });

        var result = await _service.GetFinancial(clinicId);
        return Ok(result);
    }

    [HttpGet("financial-by-period")]
    [ProducesResponseType(typeof(FinancialByPeriodDashboardDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFinancialByPeriod(
        [FromQuery] Guid clinicId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        if (clinicId == Guid.Empty)
            return BadRequest(new { message = "clinicId is required." });

        if (startDate == default || endDate == default)
            return BadRequest(new { message = "startDate and endDate are required." });

        if (endDate.Date < startDate.Date)
            return BadRequest(new { message = "endDate must be greater than or equal to startDate." });

        var result = await _service.GetFinancialByPeriod(clinicId, startDate, endDate);
        return Ok(result);
    }
}