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

    }
}
