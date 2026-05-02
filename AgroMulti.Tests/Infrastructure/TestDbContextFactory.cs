using AgroMulti.Data.Data;
using Microsoft.EntityFrameworkCore;

namespace AgroMulti.Tests.Infraestructura
{
    public class TestDbContextFactory
    {
        public static string NewDataBaseName() => $"AgroMulti_{Guid.NewGuid()}";

        public static AgroMultiContext CreateContext(string databaseName)
        {
            var options = new DbContextOptionsBuilder<AgroMultiContext>()
                .UseInMemoryDatabase(databaseName)
                .Options;

            return new InMemoryAgroMultiContext(options);
        }

        private sealed class InMemoryAgroMultiContext(DbContextOptions<AgroMultiContext> options)
            : AgroMultiContext(options)
        {
            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                // Intentionally empty: tests provide InMemory provider through options.
            }
        }
    }
}