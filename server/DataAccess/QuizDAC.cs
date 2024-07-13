using Server.Model;

namespace Server.DataAccess
{
    public class QuizDAC : BaseDAC<Quiz>
    {
        #region Singleton

        public static QuizDAC Instance { get; } = new QuizDAC();

        private QuizDAC() { }

        #endregion
    }
}
