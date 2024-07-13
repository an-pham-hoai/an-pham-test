namespace Server.DataAccess
{
    public class ScoreDAC
    {
        #region Singleton

        public static ScoreDAC Instance { get; } = new ScoreDAC();

        private ScoreDAC() { }

        #endregion
    }
}
