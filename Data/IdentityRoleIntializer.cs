using Microsoft.AspNetCore.Identity;

namespace Project_X.Data
{
    public class IdentityRoleIntializer
    {
        public static async Task SeedRoleAsync(RoleManager<IdentityRole> roleManager)
        {
            
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }
        }
    }
}
