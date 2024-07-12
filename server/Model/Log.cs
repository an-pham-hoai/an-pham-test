using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Model
{
    public class Log : BaseEntity
    {
        public long Date { get; set; } = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        public string Content { get; set; }
    }
}
