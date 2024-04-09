
namespace Magellan.Api.DependencyInjection;

public static class RegisterAppInsightsServices
{
    
    public static IServiceCollection AddAppInsightsServices(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddHttpContextAccessor()
            .AddApplicationInsightsTelemetry(options =>
            {
                options.ConnectionString = configuration["ApplicationInsights:ConnectionString"];
            })
            .AddLogging(logBuilder => logBuilder.AddApplicationInsights());
    }
}