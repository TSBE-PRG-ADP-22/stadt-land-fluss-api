using System.Text.Json.Serialization;

namespace StadtLandFussApi.Models
{
    /// <summary>
    /// Represents the entire game lobby with users and answers.
    /// </summary>
    public class Lobby
    {
        #region Properties

        [JsonIgnore]
        public int Id { get; set; }
        [JsonPropertyName("id")]
        public string? Code { get; set; }
        public List<Category> Categories { get; set; }
        public int Rounds { get; set; }
        public int Timelimit { get; set; }
        public List<User>? Users { get; set; }
        public Status Status { get; set; }

        #endregion
    }
}
