using LabWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LabWebApp.Data
{
    public class ApplicationDbContext: DbContext
    {
        public DbSet<Product> Product {  get; set; }
    }
}
