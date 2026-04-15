using Microsoft.Extensions.DependencyInjection;
using WebhookForge.Application.Common.Interfaces;
using WebhookForge.Application.Services;

namespace WebhookForge.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService,      AuthService>();
        services.AddScoped<IWorkspaceService, WorkspaceService>();
        services.AddScoped<IEndpointService,  EndpointService>();
        services.AddScoped<IRequestService,   RequestService>();
        services.AddScoped<IMockRuleService,  MockRuleService>();

        return services;
    }
}
