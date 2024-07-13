namespace Server.Model
{
    public class Question : BaseEntity
    {
        public string Code { get; set; }
        public string Description { get; set; }

        public string AnswerA { get; set; }
        public string AnswerB { get; set; }
        public string AnswerC { get; set; }
        public string AnswerD { get; set; }
        public string Answer { get; set; }
    }
}
