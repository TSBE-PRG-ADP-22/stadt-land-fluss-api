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
            var lobby = await GetLobby();
            if (lobby == null)
            {
                throw new Exception("Lobby not found.");
            }
            if (await _context.Users.Include(u => u.Answers).Where(u => u.LobbyId == lobby.Id).AllAsync(u => u.Answers != null && u.Answers.Any(a => a.Key == answers.First().Key)))
            {
                await Clients.Group(lobby.Code!).SendAsync("round-info", await _context.Users.Include(u => u.Answers)
                    .Where(u => u.LobbyId == lobby.Id)
                    .Select(u => new UserRound() { 
                        Answers = u.Answers!.Where(a => a.Key == answers.First().Key).ToList(), 
                        User = u 
                    }).ToListAsync());
            }
        }

        #endregion

        #region Review 

        [HubMethodName("answer-disliked")]
        public async Task AnswerDisliked(Answer answer, string userId)
        {
            var result = await GetAnswer(userId, answer.Category.Name, answer.Value);
            result.Downvotes++;
            _context.Update(result);
            await _context.SaveChangesAsync();
            await Clients.Group((await GetLobby()).Code!).SendAsync("answer-disliked", result);
        }

        [HubMethodName("answer-liked")]
        public async Task AnswerLiked(Answer answer, string userId)
        {
            var result = await GetAnswer(userId, answer.Category.Name, answer.Value);
            result.Downvotes--;
            _context.Update(result);
            await _context.SaveChangesAsync();
            await Clients.Group((await GetLobby()).Code!).SendAsync("answer-liked", result);
        }

        [HubMethodName("user-ready")]
        public async Task UserReady()
        {
            var user = await _context.Users.Include(u => u.Answers).FirstOrDefaultAsync(u => u.ConnectionId == Context.ConnectionId);
            if (user == null)
            {
                throw new Exception("User not found.");
            }
            user.Ready = true;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            var lobby = await GetLobby();
            await Clients.Group(lobby.Code!).SendAsync("user-ready", user);
            var users = await _context.Users.Include(u => u.Answers).Where(u => u.LobbyId == lobby.Id).ToListAsync();
            if (users.All(u => u.Ready))
            {
                foreach (var u in users)
                {
                    u.Ready = false;
                    _context.Users.Update(user);
                }
                await _context.SaveChangesAsync();
                if (lobby.PlayedRounds == lobby.Rounds)
                {
                    lobby.Status = Status.Finished;
                    _context.Lobbies.Update(lobby);
                    await _context.SaveChangesAsync();
                    await Clients.Group(lobby.Code!).SendAsync("game-finished", new Ranking() {
                        Rankings = users.Select(u => new Rank()
                        {
                            User = u,
                            Points = CalculatePoints(u.Answers!, users)
                        }).ToList()
                    });
                }
                else
                {
                    await StartRound(lobby);
                }
            }
        }

        #endregion

        #region Private Functions

        private async Task<Answer> GetAnswer(string userGuid, string categoryName, string value)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Guid == userGuid);
            if (user == null)
            {
                throw new Exception("User not found.");
            }
            var answer = await _context.Answers.Include(a => a.Category).FirstOrDefaultAsync(a => a.UserId == user.Id && a.Value == value && a.Category.Name == categoryName);
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
            lobby.PlayedRounds++;
            _context.Lobbies.Update(lobby);
            await _context.SaveChangesAsync();
            await Clients.Group(lobby.Code!).SendAsync("round-started", letter);
            if (lobby.Timelimit > 0)
            {
                await Task.Delay(lobby.Timelimit);
                await Clients.Group(lobby.Code!).SendAsync("round-finished", letter);
            }
        }

        private static int CalculatePoints(List<Answer> answers, List<User> users)
        {
            var total = 0;
            foreach (var answer in answers)
            {
                users = users.Where(u => u.Id != answer.UserId).ToList();
                if (answer.Downvotes > ((users.Count + 1) / 2))
                {
                    break;
                } 
                else if (users.Any(u => u.Answers != null && u.Answers.Where(a => a.Key == answer.Key && a.CategoryId == answer.CategoryId && a.Downvotes <= ((users.Count + 1) / 2)).Any(a => a.Value.ToLower() == answer.Value.ToLower())))
                {
                    total += 5;
                } 
                else if (users.Any(u => u.Answers == null || !u.Answers.Any(a => a.Key == answer.Key && answer.Downvotes <= ((users.Count + 1) / 2))))
                {
                    total += 20;
                } 
                else
                {
                    total += 10;
                }
            }
            return total;
        }

        #endregion
    }
}
