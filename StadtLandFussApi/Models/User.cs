﻿using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace StadtLandFussApi.Models
{
    /// <summary>
    /// A user and his answers inside a lobby.
    /// </summary>
    public class User
    {
        #region Properties

        [JsonIgnore]
        public int Id { get; set; }
        [JsonPropertyName("id")]
        public string Guid { get; set; }
        public bool Admin { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public bool Ready { get; set; }
        [JsonIgnore]
        public int LobbyId { get; set; }
        [JsonIgnore]
        public List<Answer>? Answers { get; set; }
        [JsonIgnore]
        public string? ConnectionId { get; set; }
        [NotMapped]
        public bool IsCurrentUser { get; set; }

        #endregion
    }
}
