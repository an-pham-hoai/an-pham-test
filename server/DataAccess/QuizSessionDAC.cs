using Server.Model;

namespace Server.DataAccess
{
    public class QuizSessionDAC : BaseDAC<QuizSession>
    {
        #region Singleton

        public static QuizSessionDAC Instance { get; } = new QuizSessionDAC();

        private QuizSessionDAC() { }

        #endregion
    }
}
