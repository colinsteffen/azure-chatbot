using EchoBot.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EchoBot.Repository
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private const string jsonDbFile = "Db/Departments.json";

        public IEnumerable<Department> GetDepartments()
        {
            using (StreamReader r = new StreamReader(jsonDbFile))
            {
                string json = r.ReadToEnd();
                return JsonConvert.DeserializeObject<List<Department>>(json);
            }
        }
    }
}