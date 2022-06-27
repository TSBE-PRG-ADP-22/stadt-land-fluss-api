using System.Text.Json.Serialization;

namespace StadtLandFussApi.Models
{
    /// <summary>
    /// A category that the user has to answer.
    /// </summary>
    public class Category
    {
        #region Properties

        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonIgnore]
        public int LobbyId { get; set; }

        #endregion
    }
}
