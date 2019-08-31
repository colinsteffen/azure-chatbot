using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FAQBot.Model
{
    public class Module
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public int CreditPoints { get; set; }
        public string DegreeLevel { get; set; }
        public int Duration { get; set; }
        public string Language { get; set; }
        public List<string> Content { get; set; }
        public string Commissioner { get; set; }
        public List<string> MethodOfExamination { get; set; }
    }
}
