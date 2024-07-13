namespace Server.Model
{
    public class Score : BaseEntity
    {
        public long QuizSessionId { get; set; }
        public long UserId { get; set; }
        public int QuizScore { get; set; }
    }
}
