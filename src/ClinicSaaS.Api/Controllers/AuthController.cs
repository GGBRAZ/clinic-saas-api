using ClinicSaaS.Api.Services;
using ClinicSaaS.Application.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicSaaS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly JwtTokenService _jwtTokenService;
    private readonly IConfiguration _configuration;

    public AuthController(JwtTokenService jwtTokenService, IConfiguration configuration)
    {
        _jwtTokenService = jwtTokenService;
        _configuration = configuration;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if (request is null ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new { message = "Email and password are required." });
        }

        if (request.Email != "admin@clinic.com" || request.Password != "123456")
        {
            return BadRequest(new { message = "Invalid credentials." });
        }

        var clinicId = Guid.Parse("db519f0d-02d5-4d3b-8cf7-45ad434351b1");

        var token = _jwtTokenService.GenerateToken(
            userId: "1",
            email: request.Email,
            role: "Admin",
            clinicId: clinicId);

        var expiresInMinutes = _configuration.GetValue<int>("Jwt:ExpiresInMinutes");

        return Ok(new LoginResponse
        {
            AccessToken = token,
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(expiresInMinutes)
        });
    }
}