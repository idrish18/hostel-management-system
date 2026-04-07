using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SmartHostelManagementSystem.Data;

/// <summary>
/// Design-time factory for ApplicationDbContext used for EF Core migrations
/// </summary>
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        var connectionString = "Server=localhost;Port=5432;Database=SmartHostelDB;User Id=postgres;Password=SecurePassword123!;";
        optionsBuilder.UseNpgsql(connectionString, x => x.MigrationsAssembly("SmartHostelManagementSystem"));
        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
