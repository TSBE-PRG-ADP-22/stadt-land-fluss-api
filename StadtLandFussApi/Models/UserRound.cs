namespace StadtLandFussApi.Models
{
    /// <summary>
    /// All answers of a user after a round.
    /// </summary>
    public class UserRound
    {
        #region Properties

        public User User { get; set; }
        public List<Answer> Answers { get; set; }

        #endregion
    }
}
