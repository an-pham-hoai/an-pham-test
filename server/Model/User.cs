namespace Server.Model
{
    public class User : BaseEntity
    {
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Name { get; set; }

        public long Timestamp { get; set; }
        public string QuizSessionId { get; set; }
    }
}
