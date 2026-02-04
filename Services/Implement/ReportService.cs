using AutoMapper;
using CsvHelper;
using Project_X.Data.UnitOfWork;
using Project_X.Models.DTOs;
using Project_X.Services;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace Project_X.Services
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReportService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<byte[]> GenerateSessionAttendanceCsvAsync(int sessionId)
        {
            var logs = await _unitOfWork.AttendanceLogs.GetLogsWithDetailsAsync(sessionId);

            var attendanceRecords = _mapper.Map<List<AttendanceRecordDTO>>(logs);
            using (var memoryStream = new MemoryStream())
            using (var writer = new StreamWriter(memoryStream))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(attendanceRecords);
                await writer.FlushAsync();
                return memoryStream.ToArray();
            }
        }
    }
}
