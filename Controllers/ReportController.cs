using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_X.Services;
using System.Threading.Tasks;
using Project_X.Models.Response;
using Project_X.Models.DTOs;
using Models;

namespace Project_X.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("session/{sessionId}/csv")]
        public async Task<IActionResult> GetSessionAttendanceCsv(int sessionId, [FromServices] IOrganizationService organizationService, [FromServices] ISessionService sessionService)
        {
            if (!Request.Headers.TryGetValue("X-Api-Key", out var apiKey))
            {
                return Unauthorized(ApiResponse.FailureResponse("API Key is required."));
            }

            var orgResult = await organizationService.GetOrganizationByApiKeyAsync(apiKey!);
            if (!orgResult.Success)
            {
                return Unauthorized(ApiResponse.FailureResponse("Invalid API Key."));
            }

            var organization = (Organization)orgResult.Data;
            var sessionResult = await sessionService.GetSessionByIdAsync(sessionId);

            if (!sessionResult.Success)
            {
                 return NotFound(ApiResponse.FailureResponse("Session not found."));
            }

            var session = (SessionResponseDTO)sessionResult.Data;
            
            if (session.OrganizationId != organization.OrganizationId)
            {
                 return Unauthorized(ApiResponse.FailureResponse("Access Denied: Session does not belong to this organization."));
            }

            var csvBytes = await _reportService.GenerateSessionAttendanceCsvAsync(sessionId);
            return File(csvBytes, "text/csv", $"Attendance_Session_{sessionId}.csv");
        }
    }
}
