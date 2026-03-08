using RTChatBackend.Application.Interfaces;
using RTChatBackend.Infrastructure.Redis;

namespace RTChatBackend.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddRedisInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var options = configuration.GetSection("Redis").Get<RedisOptions>();

        services.AddSingleton(new RedisConnectionFactory(options!.ConnectionString));

        services.AddScoped<IUserSessionService, UserSessionService>();
        services.AddScoped<IPresenceService, PresenceService>();

        return services;
    }
}