using System;
using System.Reactive;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using TestClient.Models;

namespace TestClient.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private HubConnection? HubConnection { get; set; }

        [Reactive] public string LobbyId { get; set; } = "e97be7b7";
        [Reactive] public string UserId { get; set; } = "24ef415c-2ae8-464d-b958-13f105cf4aef";
        [Reactive] public string Greeting { get; private set; } = "Welcome to Avalonia!";

        public ReactiveCommand<Unit, Unit> ConnectCommand { get; }
        public ReactiveCommand<Unit, Unit> DisconnectCommand { get; }
        public ReactiveCommand<Unit, Unit> JoinLobbyCommand { get; }

        public MainWindowViewModel()
        {
            ConnectCommand = ReactiveCommand.CreateFromTask(Connect);
            DisconnectCommand = ReactiveCommand.CreateFromTask(Disconnect);
            JoinLobbyCommand = ReactiveCommand.CreateFromTask(JoinLobby);
        }

        private async Task Connect()
        {
            var uri = "http://localhost:8080/lobby-hub";
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

            HubConnection.Closed += (error) => { Console.WriteLine(error?.Message); return Task.CompletedTask; };

            HubConnection.On<User>("user-added", (user) =>
            {
                Greeting = user?.Name ?? string.Empty;
            });

            await HubConnection.StartAsync();
        }

        private async Task Disconnect()
        {
            if (HubConnection != null)
            {
                await HubConnection.StopAsync();
            }
        }

        private async Task JoinLobby()
        {
            if (HubConnection != null)
            {
                await HubConnection.SendAsync("join-lobby", UserId, LobbyId);
            }
        }
    }
}
