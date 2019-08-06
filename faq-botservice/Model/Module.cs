using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EchoBot.Model
{
    public class Module
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public int CreditPoints { get; set; }
        public Degree Level { get; set; }
        public int Duration { get; set; }
        public string Language { get; set; }
        public List<string> Content { get; set; }
        public string Commissioner { get; set; }
        public bool NumerusClausus { get; set; }
    }
}
