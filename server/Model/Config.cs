using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Server.Model
{
    public class Config
    {
        #region Singleton

        private static Config instance = new Config();

        public static Config Instance
        {
            get { return instance; }
        }

        private Config() { }

        #endregion

        #region Properties

        public string ClientBaseUrl { get; set; }

        public string ConnectionString { get; set; }

        #endregion
    }
}
