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
        public IEnumerable<DegreeCourse> GetDegreeCourses()
        {
            using(StreamReader r = new StreamReader("Db/DegreeCourses.json"))
            {
                string json = r.ReadToEnd();
                return JsonConvert.DeserializeObject<List<DegreeCourse>>(json);
            }
        }

        public void InsertDegreeCourse(DegreeCourse degreeCourse)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }
    }
}
