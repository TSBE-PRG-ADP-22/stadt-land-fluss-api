using Microsoft.AspNetCore.SignalR;
using StadtLandFussApi.Models;
using StadtLandFussApi.Persistence;

namespace StadtLandFussApi.Hubs
{
    public class LobbyHub : Hub
    {

        #region Fields & Constructor

        private readonly AppDbContext _context;

        public LobbyHub(AppDbContext context)
        {
            _context = context;
        }

        #endregion

        #region Lobby

        [HubMethodName("join-lobby")]
        public async Task JoinLobby(string userId, string lobbyId)
        {
            var user = _context.Users.FirstOrDefault(u => u.Guid == userId);
            if (user == null)
            {
                throw new Exception("User not found.");
            }
            user.ConnectionId = Context.ConnectionId;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            await Groups.AddToGroupAsync(Context.ConnectionId, lobbyId);
            await Clients.Group(lobbyId).SendAsync("user-added", new User { Admin = user.Admin, Guid = user.Guid, Name = user.Name });
            await Clients.All.SendAsync("user-added", "BANANA").ConfigureAwait(false);
        }

        [HubMethodName("game-start")]
        public async Task GameStart()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Round

        [HubMethodName("round-finished")]
        public async Task RoundFinished()
        {
            throw new NotImplementedException();
        }

        [HubMethodName("user-round-data")]
        public async Task UserRoundData(UserRound userRound)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Review 

        [HubMethodName("answer-disliked")]
        public async Task AnswerDisliked(Answer answer)
        {
            throw new NotImplementedException();
        }

        [HubMethodName("answer-liked")]
        public async Task AnswerLiked(Answer answer)
        {
            throw new NotImplementedException();
        }

        [HubMethodName("user-ready")]
        public async Task UserReady()
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
