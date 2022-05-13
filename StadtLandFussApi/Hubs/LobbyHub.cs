using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using StadtLandFussApi.Models;
using StadtLandFussApi.Persistence;

namespace StadtLandFussApi.Hubs
{
    public class LobbyHub : Hub
    {

        #region Fields & Constructor

        private readonly AppDbContext _context;
        private readonly List<string> _letters = new()
        {
            "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "X", "Y", "Z"
        };
        private readonly Random _random = new();

        public LobbyHub(AppDbContext context)
        {
            _context = context;
        }

        #endregion

        #region Lobby

        [HubMethodName("join-lobby")]
        public async Task JoinLobby(string userId, string lobbyId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Guid == userId);
            if (user == null)
            {
                throw new Exception("User not found.");
            }
            user.ConnectionId = Context.ConnectionId;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            await Groups.AddToGroupAsync(Context.ConnectionId, lobbyId);
            await Clients.Group(lobbyId).SendAsync("user-added", new User { Admin = user.Admin, Guid = user.Guid, Name = user.Name });
        }

        [HubMethodName("game-start")]
        public async Task GameStart()
        {
            var lobby = await GetLobby();
            lobby.Status = Status.InProgress;
            await _context.SaveChangesAsync();
            await StartRound(lobby);
        }

        #endregion

        #region Round

        [HubMethodName("round-finished")]
        public async Task RoundFinished(string letter)
        {
            await Clients.Group((await GetLobby()).Code!).SendAsync("round-finished", letter);
        }

        [HubMethodName("user-round-data")]
        public async Task UserRoundData(List<Answer> answers)
        {
            var user = await _context.Users.Include(u => u.Answers).FirstOrDefaultAsync(u => u.ConnectionId == Context.ConnectionId);
            if (user == null)
            {
                throw new Exception("User not found.");
            }
            if (user.Answers == null)
            {
                user.Answers = new();
            }
            user.Answers.AddRange(answers);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
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

        #region Private Functions

        private async Task<Answer> GetAnswer(string guid, string value)
        {
            var answer = await _context.Answers.Include(a => a.Category).FirstOrDefaultAsync(a => a.Key == guid && a.Value == value);
            if (answer == null)
            {
                throw new Exception("Answer not found.");
            }
            return answer;
        }

        private async Task<Lobby> GetLobby()
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.ConnectionId == Context.ConnectionId);
            if (user == null)
            {
                throw new Exception("User not found.");
            }
            var lobby = await _context.Lobbies.FirstOrDefaultAsync(l => l.Id == user.LobbyId);
            if (lobby == null)
            {
                throw new Exception("Lobby not found.");
            }
            return lobby;
        }

        private async Task StartRound(Lobby lobby)
        {
            var letters = _letters;
            var answers = await _context.Answers.Include(a => a.Category).Where(a => a.Category.LobbyId == lobby.Id).ToListAsync();
            var excluded = letters.Except(answers.Select(a => a.Key).Distinct().ToList()).ToList();
            var letter = excluded[_random.Next(0, excluded.Count - 1)];
            await Clients.Group(lobby.Code!).SendAsync("round-started", letter);
            await Task.Delay(lobby.Timelimit);
            await Clients.Group(lobby.Code!).SendAsync("round-finished", letter);
        }

        #endregion
    }
}
