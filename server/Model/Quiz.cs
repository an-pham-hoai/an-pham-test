using System.Collections.Generic;

namespace Server.Model
{
    public class Quiz : BaseEntity
    {
        public string Code { get; set; }
        public List<string> Questions { get; set; } = new List<string>();
    }
}
