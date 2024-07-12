using System.Collections.Generic;

namespace Server.Model
{
    public class BulkInput
    {
        public string Entity { get; set; }
        public List<string> Jsons { get; set; } = new List<string>();
    }
}
