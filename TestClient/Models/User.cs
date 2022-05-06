using System;
using System.Collections.Generic;

namespace TestClient.Models
{
    /// <summary>
    /// A user and his answers inside a lobby.
    /// </summary>
    public class User
    {
        public Guid Id { get; set; }
        public bool Admin { get; set; }
        public string? Name { get; set; }
        //public List<Answer>? Answers { get; set; }
    }
}
