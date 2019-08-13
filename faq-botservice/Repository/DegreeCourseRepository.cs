using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EchoBot.Model;
using Newtonsoft.Json;

namespace EchoBot.Repository
{
    public class DegreeCourseRepository : IDegreeCourseRepository
    {
        private const string jsonDbFile = "Db/DegreeCourses.json";

        public IEnumerable<DegreeCourse> GetDegreeCourses()
        {
            using(StreamReader r = new StreamReader(jsonDbFile))
            {
                string json = r.ReadToEnd();
                return JsonConvert.DeserializeObject<List<DegreeCourse>>(json);
            }
        }
    }
}
