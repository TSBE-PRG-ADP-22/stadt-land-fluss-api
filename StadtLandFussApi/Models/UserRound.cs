namespace StadtLandFussApi.Models
{
    public class UserRound
    {
        #region Properties

        public User User { get; set; }
        public List<Answer>? Answers { get; set; }

        #endregion
    }
}
