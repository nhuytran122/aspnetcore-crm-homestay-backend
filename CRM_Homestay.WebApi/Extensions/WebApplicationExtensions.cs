using CRM_Homestay.Database.Contexts;
using Microsoft.EntityFrameworkCore;

namespace CRM_Homestay.App.Extensions;

/// <summary>
/// WebApplicationExtensions
/// </summary>
public static class WebApplicationExtensions
{
    /// <summary>
    /// ApplyMigrations
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static WebApplication ApplyMigrations(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<HomestayContext>();
            db.Database.Migrate();
            return app;
        }
    }

}