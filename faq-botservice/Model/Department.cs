using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EchoBot.Model
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DepartmentNumber { get; set; }
        public List<int> DegreeCourses { get; set; }
    }
}
