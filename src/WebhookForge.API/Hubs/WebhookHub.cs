using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace WebhookForge.API.Hubs;

/// <summary>
/// SignalR hub for real-time webhook request feed.
/// Clients join a group named after the endpoint ID they want to watch.
/// </summary>
[Authorize]
public class WebhookHub : Hub
{
    /// <summary>
    /// Subscribe to live requests for a specific endpoint.
    /// </summary>
    public async Task Subscribe(string endpointId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, endpointId);
    }

    /// <summary>
    /// Unsubscribe from an endpoint's live feed.
    /// </summary>
    public async Task Unsubscribe(string endpointId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, endpointId);
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }
}
