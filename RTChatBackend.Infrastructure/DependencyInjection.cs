using RTChatBackend.Application.Interfaces;
using RTChatBackend.Infrastructure.Redis;

namespace RTChatBackend.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddRedisInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var options = configuration.GetSection("Redis").Get<RedisOptions>()
                      ?? throw new ArgumentException("Redis configuration is missing.");

        services.AddSingleton(options);
        services.AddSingleton(new RedisConnectionFactory(options.ConnectionString));

        services.AddScoped<IUserSessionService, UserSessionService>();
        services.AddScoped<IPresenceService, PresenceService>();
        services.AddScoped<IChatStorageService, ChatStorageService>();
        services.AddScoped<IMessageStorageService, MessageStorageService>();

        return services;
    }
}