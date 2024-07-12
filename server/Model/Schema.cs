using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Model
{
    public class SchemaDB
    {
        public List<SchemaTable> Tables { get; set; } = new List<SchemaTable>();
    }

    public class SchemaTable
    {
        public string Name { get; set; }

        public List<string> Fields { get; set; } = new List<string>();
    }
}
