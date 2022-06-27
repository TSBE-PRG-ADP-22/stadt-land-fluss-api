namespace TestClient.Models
{
    /// <summary>
    /// A category that the user has to answer.
    /// </summary>
    public class Category
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int LobbyId { get; set; }
    }
}
