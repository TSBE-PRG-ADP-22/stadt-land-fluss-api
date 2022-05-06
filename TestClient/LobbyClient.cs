using Microsoft.AspNetCore.SignalR.Client;

namespace TestClient
{
    public class LobbyClient
    {
        public static async Task ExecuteAsync()
        {
            //Replace port "7054" with the port running the MainSignalServer project
            var uri = "http://localhost:8080/user-added";

            await using var connection = new HubConnectionBuilder().WithUrl(uri).Build();

            await connection.StartAsync();

            await foreach (var date in connection.StreamAsync<DateTime>("Streaming"))
            {
                Console.WriteLine(date);
            }
        }


        public static async Task JoinLobby()
        {
            //Replace port "7054" with the port running the MainSignalServer project
            var uri = "http://localhost:8080/user-added";

            await using var connection = new HubConnectionBuilder().WithUrl(uri).Build();
            await connection.StartAsync();

            await connection.InvokeAsync("JoinLobby", "12345");
        }
    }
}
