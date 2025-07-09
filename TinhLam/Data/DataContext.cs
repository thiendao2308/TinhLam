using Microsoft.EntityFrameworkCore;
using TinhLam.Models;

namespace TinhLam.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        
        public DbSet<ProductModels> Products { get; set; }

        public DbSet<CategoryModels> Categories { get; set; }
    }
}