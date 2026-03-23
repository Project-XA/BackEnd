using Microsoft.EntityFrameworkCore;
using Project_X.Data.Context;
using Project_X.Data.Repository;
using Project_X.Models;

namespace Project_X.Data.Repositories
{
    public class SectionRepository : Repository<Section>, ISectionRepository
    {
        public SectionRepository(AppDbConext context) : base(context)
        {
        }

        public async Task<List<Section>> GetSectionsByOrganizationAsync(int organizationId)
        {
            return await _context.Sections
                .Where(s => s.OrganizationId == organizationId)
                .Include(s => s.SectionUsers)
                .ToListAsync();
        }

        public async Task<Section?> GetSectionWithUsersAsync(int sectionId)
        {
            return await _context.Sections
                .Include(s => s.SectionUsers)
                    .ThenInclude(su => su.User)
                .FirstOrDefaultAsync(s => s.SectionId == sectionId);
        }

        public async Task<Section?> GetByCodeAsync(int sectionCode)
        {
            return await _context.Sections
                .FirstOrDefaultAsync(s => s.SectionCode == sectionCode);
        }

        public async Task<bool> VerifySectionCodeAsync(int sectionCode)
        {
            return await _context.Sections
                .AnyAsync(s => s.SectionCode == sectionCode);
        }
    }
}
