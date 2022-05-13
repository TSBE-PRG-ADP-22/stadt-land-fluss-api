namespace StadtLandFussApi.Models
{
    /// <summary>
    /// The amount of points a specifc user earned during a game.
    /// </summary>
    public class Rank
    {
        #region Properties

        public User User { get; set; }
        public int Points { get; set; }

        #endregion
    }
}
