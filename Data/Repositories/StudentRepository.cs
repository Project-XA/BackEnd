using Microsoft.EntityFrameworkCore;
using Models;
using Project_X.Data.Context;
using Project_X.Data.Repository;

namespace Project_X.Data.Repositories
{
    public class StudentRepository : Repository<Student>, IStudentRepository
    {
        public StudentRepository(AppDbConext context)
            : base(context)
        {
        }

        public async Task<Student?> GetByAppUserIdAsync(string appUserId)
        {
            return await _context.Students
                .Include(s => s.User)
                .Include(s => s.Organization)
                .FirstOrDefaultAsync(s => s.AppUserId == appUserId);
        }

        public async Task<Student?> GetByOrganizationAndRollNumberAsync(int organizationId, string rollNumber)
        {
            return await _context.Students
                .FirstOrDefaultAsync(s => s.OrganizationId == organizationId && s.RollNumber == rollNumber);
        }

        public async Task<Student?> GetByOrganizationAndEmailAsync(int organizationId, string email)
        {
            return await _context.Students
                .Include(s => s.User)
                .Include(s => s.Organization)
                .FirstOrDefaultAsync(s => s.OrganizationId == organizationId && s.User.Email == email);
        }

        public async Task<List<Student>> GetByOrganizationIdAsync(int organizationId)
        {
            return await _context.Students
                .Include(s => s.User)
                .Include(s => s.Organization)
                .Where(s => s.OrganizationId == organizationId)
                .ToListAsync();
        }
    }
}
