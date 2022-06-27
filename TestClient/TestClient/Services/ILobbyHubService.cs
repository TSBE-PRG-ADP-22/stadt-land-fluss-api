using Microsoft.AspNetCore.SignalR.Client;

namespace TestClient.Services
{
    public interface ILobbyHubService
    {
        public HubConnection? HubConnection { get; }
        public void Connect();
    }
}
