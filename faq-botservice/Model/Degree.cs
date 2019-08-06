using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EchoBot.Model
{
    public class Degree
    {
        public int Id { get; set; }
        public string DegreeTitle { get; set; }
        public string Abbreviation { get; set; }
        public List<Module> Modules { get; set; }
    }
}
