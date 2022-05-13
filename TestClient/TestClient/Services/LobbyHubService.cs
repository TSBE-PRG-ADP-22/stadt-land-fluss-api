using Microsoft.AspNetCore.SignalR.Client;

namespace TestClient.Services
{
    public class LobbyHubService : ILobbyHubService
    {
        public HubConnection? HubConnection { get; private set; }
    }
}
