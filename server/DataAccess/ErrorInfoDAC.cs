using Server.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.DataAccess
{
    public class ErrorInfoDAC : BaseDAC<ErrorInfo>
    {
        #region Singleton

        public static ErrorInfoDAC Instance { get; } = new ErrorInfoDAC();

        private ErrorInfoDAC() { }

        #endregion

    }
}
