using Microsoft.EntityFrameworkCore;
namespace LoginDotnet.Data
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions options) : base(options)
        {
        }

      
    }
}
