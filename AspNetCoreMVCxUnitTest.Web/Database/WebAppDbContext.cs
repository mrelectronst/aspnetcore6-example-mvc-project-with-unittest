using AspNetCoreMVCxUnitTest.Web.Database.DTOs;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreMVCxUnitTest.Web.Database
{
    public class WebAppDbContext : DbContext
    {
        public WebAppDbContext(DbContextOptions<WebAppDbContext> options) : base(options)
        {

        }

        public DbSet<ProductDTO> Products { get; set; }
    }
}
