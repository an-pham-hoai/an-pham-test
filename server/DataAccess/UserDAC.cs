using Server.Model;

namespace Server.DataAccess
{
    public class UserDAC : BaseDAC<User>
    {
        #region Singleton

        public static UserDAC Instance { get; } = new UserDAC();

        private UserDAC() { }

        #endregion
    }
}
