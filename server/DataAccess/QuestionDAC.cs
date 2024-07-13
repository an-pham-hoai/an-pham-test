using Server.Model;

namespace Server.DataAccess
{
    public class QuestionDAC : BaseDAC<Question>
    {
        #region Singleton

        public static QuestionDAC Instance { get; } = new QuestionDAC();

        private QuestionDAC() { }

        #endregion
    }
}
