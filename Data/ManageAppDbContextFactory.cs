using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace WebChatApp.Data
{
    public class ManageAppDbContextFactory : IDesignTimeDbContextFactory<ManageAppDbContext>
    {
        public ManageAppDbContext CreateDbContext(string[] args)
        {
            var enviromentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIROMENT");

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{enviromentName}.json")
                .Build();

            var builder = new DbContextOptionsBuilder<ManageAppDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            builder.UseNpgsql(connectionString);

            return new ManageAppDbContext(builder.Options);
        }
    }
}
