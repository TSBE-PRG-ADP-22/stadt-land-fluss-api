using System.Text.Json.Serialization;

namespace StadtLandFussApi.Models
{
    /// <summary>
    /// A category that the user has to answer.
    /// </summary>
    public class Category
    {

        #region Properties

        [JsonIgnore]
        public int Id { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public int LobbyId { get; set; }

        #endregion

    }
}
