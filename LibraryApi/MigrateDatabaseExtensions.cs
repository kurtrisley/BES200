using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryApi
{
    public static class MigrateDatabaseExtensions
    {
        public static IHost MigrateDatabase<T>(this IHost webHost) where T : DbContext
        {
            using (var scope = webHost.Services.CreateScope())
            {
                T db = null;
                var services = scope.ServiceProvider;
                try
                {
                    db = services.GetRequiredService<T>();
                    db.Database.Migrate();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();

                    logger.LogWarning("Fixin' to try it again");

                    Thread.Sleep(8000);
                    logger.LogWarning("Just trying it again");
                    db.Database.Migrate();
                    logger.LogError(ex, "An error occurred trying to migrate the database");
                }
            }

            return webHost;
        }
    }
}
