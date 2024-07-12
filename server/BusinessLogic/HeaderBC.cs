using Server.DataAccess;
using Server.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.BusinessLogic
{
    public class HeaderBC
    {
        #region Singleton

        private static HeaderBC instance = new HeaderBC();

        public static HeaderBC Instance
        {
            get
            {
                return instance;
            }
        }

        private HeaderBC() { }

        #endregion

        public string GetScreen()
        {
            return GetHeader("Screen");
        }

        /// <summary>
        /// Gets nonce token - an arbitrary number that can be used just once in a cryptographic communication.
        /// Used for authentication
        /// </summary>
        /// <returns></returns>
        public long GetNonce()
        {
            try
            {
                string h = GetHeader("Nonce");
                if (string.IsNullOrEmpty(h)) return 0;
                string d = CryptoBC.Instance.DecryptStringAES(h);
                if (long.TryParse(d, out long nonce))
                {
                    return nonce;
                }

                return 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public long GetLoginUserId()
        {
            try
            {
                string h = GetHeader("LoginUserId");
                if (string.IsNullOrEmpty(h)) return 0;
                string d = CryptoBC.Instance.DecryptStringAES(h);
                if (long.TryParse(d, out long id))
                {
                    return id;
                }

                return 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public string GetVendorToken()
        {
            return GetHeader("VendorToken");
        }

        public int GetCompanyId()
        {
            string h = GetHeader("CompanyId");
            if (string.IsNullOrEmpty(h)) return 0;
            string d = CryptoBC.Instance.DecryptStringAES(h);
            if (int.TryParse(d, out int id))
            {
                return id;
            }

            return 0;
        }

        public string GetHeader(string key)
        {
            if (AppHttpContext.Current.Request.Headers.ContainsKey(key)) return AppHttpContext.Current.Request.Headers[key];
            return string.Empty;
        }
    }
}
