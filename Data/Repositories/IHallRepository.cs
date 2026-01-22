using Project_X.Models;

namespace Project_X.Data.Repositories
{
    public interface IHallRepository:IRepository<Hall>
    {
        Task<Hall?> GetHallByNameAsync(string hallName, int organizationId);
        Task<List<Hall>> GetHallsByOrganizationIdAsync(int organizationId);
    }
}
