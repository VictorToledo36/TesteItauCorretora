// TesteItauCorretora.Infrastructure/Persistence/AppDbContextFactory.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TesteItauCorretora.Infrastructure.Persistence;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseMySql(
            "Server=localhost;Database=testeItauCorretora;User=root;Password=root;",
            ServerVersion.AutoDetect("Server=localhost;Database=testeItauCorretora;User=root;Password=root;")
        );

        return new AppDbContext(optionsBuilder.Options);
    }
}