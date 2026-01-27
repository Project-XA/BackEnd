using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_X.Services;
using System.Threading.Tasks;

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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetSessionAttendanceCsv(int sessionId)
        {
            var csvBytes = await _reportService.GenerateSessionAttendanceCsvAsync(sessionId);
            return File(csvBytes, "text/csv", $"Attendance_Session_{sessionId}.csv");
        }
    }
}
