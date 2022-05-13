using System.Reactive;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using TestClient.Services;

namespace TestClient.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, IMainWindowViewModel
    {
        private readonly ILobbyService _lobbyService;
        private readonly ILobbyHubService _lobbyHubService;

        [Reactive] public string LobbyId { get; set; } = "e97be7b7";
        [Reactive] public string UserId { get; set; } = "24ef415c-2ae8-464d-b958-13f105cf4aef";
        [Reactive] public string Greeting { get; private set; } = "Welcome to Avalonia!";

        public ReactiveCommand<Unit, Unit> ConnectCommand { get; }
        public ReactiveCommand<Unit, Unit> DisconnectCommand { get; }
        public ReactiveCommand<Unit, Unit> JoinLobbyCommand { get; }

        public MainWindowViewModel(ILobbyService lobbyService, ILobbyHubService lobbyHubService)
        {
            _lobbyService = lobbyService;
            _lobbyHubService = lobbyHubService;

            ConnectCommand = ReactiveCommand.CreateFromTask(Connect);
            DisconnectCommand = ReactiveCommand.CreateFromTask(Disconnect);
            JoinLobbyCommand = ReactiveCommand.CreateFromTask(JoinLobby);
        }

        private async Task Connect()
        {
            //var uri = "http://localhost:8080/lobby-hub";
            //HubConnection = new HubConnectionBuilder()
            //    .AddJsonProtocol()
            //    .WithUrl(uri)
            //    .WithAutomaticReconnect()
            //    .ConfigureLogging(logging =>
            //    {
            //        logging.SetMinimumLevel(LogLevel.Information);
            //        logging.AddConsole();
            //    })
            //    .Build();

            //HubConnection.Closed += (error) => { Console.WriteLine(error?.Message); return Task.CompletedTask; };

            //HubConnection.On<User>("user-added", (user) =>
            //{
            //    Greeting = user?.Name ?? string.Empty;
            //});

            //await HubConnection.StartAsync();
        }

        private async Task Disconnect()
        {
            if (_lobbyHubService.HubConnection != null)
            {
                await _lobbyHubService.HubConnection.StopAsync();
            }
        }

        private async Task JoinLobby()
        {
            if (_lobbyHubService.HubConnection != null)
            {
                await _lobbyHubService.HubConnection.SendAsync("join-lobby", UserId, LobbyId);
            }
        }
    }
}
