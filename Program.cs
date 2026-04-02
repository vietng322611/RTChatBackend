using Microsoft.AspNetCore.Authentication.Cookies;
using RTChatBackend.Api.Hubs;
using RTChatBackend.Application.Interfaces;
using RTChatBackend.Application.Services;
using RTChatBackend.Infrastructure;
using RTChatBackend.Infrastructure.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRedisInfrastructure(builder.Configuration);

builder.Services.AddSignalR();
builder.Services.AddSingleton<PresenceService>();

builder.Services.AddSingleton<ICodeGenerator, CodeGenerator>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IMessageService, MessageService>();

builder.Services.AddControllers();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "RTChat.Auth";
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
        options.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        };
    });

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<ChatHub>("/chatHub");

app.MapControllers();
app.Run();