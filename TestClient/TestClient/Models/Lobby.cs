using System.Collections.Generic;

namespace TestClient.Models
{
    /// <summary>
    /// Represents the entire game lobby with users and answers.
    /// </summary>
    public class Lobby
    {
        public string? Id { get; set; }
        public List<Category>? Categories { get; set; }
        public int Rounds { get; set; }
        public int Timelimit { get; set; }
        public List<User>? Users { get; set; }
        public Status Status { get; set; }
    }
}
