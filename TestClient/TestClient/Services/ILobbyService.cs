using System.Threading.Tasks;
using Refit;
using TestClient.Models;

namespace TestClient.Services
{
    [Headers("Content-Type: application/json; charset=UTF-8")]
    public interface ILobbyService
    {
        [Post("/lobby")]
        public Task<Lobby> CreateLobby();

        [Post("/lobby/{id}/user")]
        public Task<User> CreateUser(string id);
    }
}
