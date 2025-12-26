using LoginDotnet.Data;

namespace LoginDotnet.Extensions
{
    public static class ServicesExtension
    {
        public static void AddApplicationService(ref IServiceCollection service, IConfiguration config)
        {

            service.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            service.AddDbContext<ApplicationContext>(
                options => options.UseSqlServer(config.GetConnectionString("DefaultConnection"))
                );
            service.AddOpenApi();

            service.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            return;
        }
        
    }
}
