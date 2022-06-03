using System.Text.Json.Serialization;

namespace StadtLandFussApi.Models
{
    /// <summary>
    /// An answer specific to a category and a user.
    /// </summary>
    public class Answer
    {
        #region Properties

        [JsonIgnore]
        public int Id { get; set; }
        [JsonPropertyName("key")]
        public string Key { get; set; }
        [JsonPropertyName("answer")]
        public string Value { get; set; }
        [JsonPropertyName("downvotes")]
        public int Downvotes { get; set; }
        [JsonIgnore]
        public int UserId { get; set; }
        [JsonIgnore]
        public int CategoryId { get; set; }
        [JsonPropertyName("category")]
        public Category Category { get; set; }

        #endregion
    }
}
