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
            var uri = "http://localhost:8080/lobby-hub";

            await using var connection = new HubConnectionBuilder().WithUrl(uri).Build();

            connection.Closed += Connection_Closed;

            connection.On<string>("user-added", (user) =>
            {
                Console.WriteLine(user);
            });

            var t = connection.StartAsync();

            t.Wait();

            await connection.SendAsync("join-lobby", "681abc6c-2847-4f36-aea6-45dfe8c1b10d", "a5a5179c");
            Console.WriteLine("done");
        }

        private static Task Connection_Closed(Exception? arg)
        {
            Console.WriteLine("oops");
            return null!;
        }
    }
}
