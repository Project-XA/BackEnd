using Microsoft.EntityFrameworkCore;
using Project_X.Data.Context;
using Project_X.Data.Repository;
using Project_X.Models;

namespace Project_X.Data.Repositories
{
    public class HallRepository:Repository<Hall>, IHallRepository
    {
        public HallRepository(AppDbConext context) 
            : base(context)
        {
        }
        public async Task<Hall?> GetHallByNameAsync(string hallName, int organizationId)
        {
            return await _context.Halls
                .FirstOrDefaultAsync(h => h.HallName == hallName && h.OrganizationId == organizationId);
        }

        public async Task<List<Hall>> GetHallsByOrganizationIdAsync(int organizationId)
        {
            return await _context.Halls
                .Where(h => h.OrganizationId == organizationId)
                .ToListAsync();
        }
    }
}
