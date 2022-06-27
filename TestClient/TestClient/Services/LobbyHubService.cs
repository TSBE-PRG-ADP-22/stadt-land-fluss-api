using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TestClient.Services
{
    public class LobbyHubService : ILobbyHubService
    {
        private ClientConfiguration ClientConfig { get; }

        public HubConnection? HubConnection { get; private set; }
        public LobbyHubService(ClientConfiguration clientConfiguration)
        {
            ClientConfig = clientConfiguration;
        }


        public void Connect()
        {
            var uri = $"{ClientConfig.BaseUrl}/lobby-hub";
            HubConnection = new HubConnectionBuilder()
                .AddJsonProtocol()
                .WithUrl(uri)
                .WithAutomaticReconnect()
                .ConfigureLogging(logging =>
                {
                    logging.SetMinimumLevel(LogLevel.Information);
                    logging.AddConsole();
                })
                .Build();
        }
    }
}
