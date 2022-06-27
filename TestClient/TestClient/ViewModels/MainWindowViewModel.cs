using System;
using System.Reactive;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using TestClient.Models;
using TestClient.Services;
using System.Linq;
using Refit;
using System.Collections.Generic;

namespace TestClient.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, IMainWindowViewModel
    {
        private readonly ILobbyService _lobbyService;
        private readonly ILobbyHubService _lobbyHubService;

        [Reactive] public string Greeting { get; private set; } = "Welcome to Avalonia!";
        [Reactive] public string ConnectionStatus { get; private set; } = "SignalR status:";

        [Reactive] public Lobby? Lobby { get; set; }


        public ReactiveCommand<Unit, Unit> CreateLobbyCommand { get; }
        public ReactiveCommand<Unit, Unit> ConnectCommand { get; }
        public ReactiveCommand<Unit, Unit> DisconnectCommand { get; }
        public ReactiveCommand<Unit, Unit> JoinLobbyCommand { get; }

        public MainWindowViewModel(ILobbyService lobbyService, ILobbyHubService lobbyHubService)
        {
            _lobbyService = lobbyService;
            _lobbyHubService = lobbyHubService;

            CreateLobbyCommand = ReactiveCommand.CreateFromTask(CreateLobby);
            ConnectCommand = ReactiveCommand.CreateFromTask(Connect);
            DisconnectCommand = ReactiveCommand.CreateFromTask(Disconnect);
            JoinLobbyCommand = ReactiveCommand.CreateFromTask(JoinLobby);
        }

        private async Task CreateLobby()
        {
            try
            {
                Lobby = await _lobbyService.CreateLobby(new Lobby
                {
                    Rounds = 3,
                    Timelimit = 10,
                    Categories = new List<Category> { new Category { Name = "Dummy" } }
                });
            }
            catch (ApiException ex)
            {
                throw;
            }
        }
        private async Task Connect()
        {
            _lobbyHubService.Connect();
            if (_lobbyHubService.HubConnection != null)
            {
                _lobbyHubService.HubConnection.Closed += (error) => { Console.WriteLine(error?.Message); return Task.CompletedTask; };
                _lobbyHubService.HubConnection.On<User>("user-added", (user) =>
                {
                    Greeting = user?.Name ?? string.Empty;
                });

                await _lobbyHubService.HubConnection.StartAsync();
            }
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
            if (_lobbyHubService.HubConnection != null && Lobby != null)
            {
                await _lobbyHubService.HubConnection.SendAsync("join-lobby", Lobby.Users.First().Id, Lobby.Id);
            }
        }
    }
}
