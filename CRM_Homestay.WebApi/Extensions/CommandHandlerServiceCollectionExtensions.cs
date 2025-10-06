
using CRM_Homestay.Contract.OtpCodes.Handlers;
using CRM_Homestay.Service.OtpCodes.Handlers;

namespace CRM_Homestay.App.Extensions;

/// <summary>
/// Extension methods for adding command handlers to the service collection.
/// </summary>
public static class CommandHandlerServiceCollectionExtensions
{
    /// <summary>
    /// Adds command handlers to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The modified service collection.</returns>
    public static IServiceCollection AddCommandHandlers(this IServiceCollection services)
    {
        services.AddScoped<IOtpTokenHandler, OtpTokenHandler>();
        return services;
    }
}
