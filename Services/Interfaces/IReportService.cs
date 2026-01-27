using System.Threading.Tasks;

namespace Project_X.Services
{
    public interface IReportService
    {
        Task<byte[]> GenerateSessionAttendanceCsvAsync(int sessionId);
    }
}
