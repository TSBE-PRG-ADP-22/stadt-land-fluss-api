using Microsoft.AspNetCore.SignalR;

namespace StadtLandFussApi.Hubs
{
    public class LobbyHub : Hub
    {
        public async IAsyncEnumerable<DateTime> Streaming(CancellationToken cancellationToken)
        {
            while (true)
            {
                yield return DateTime.UtcNow;
                await Task.Delay(1000, cancellationToken);
            }
        }

        public async Task JoinLobby(string lobbyName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, lobbyName);
            await Clients.Group(lobbyName).SendAsync("JoinLobby", Context.User.Identity.Name + " joined.");
        }

        public Task LeaveLobby(string lobbyName)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, lobbyName);
        }
    }
}
