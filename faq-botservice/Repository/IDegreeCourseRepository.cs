using EchoBot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EchoBot.Repository
{
    public interface IDegreeCourseRepository
    {
        IEnumerable<DegreeCourse> GetDegreeCourses();
        void InsertDegreeCourse(DegreeCourse degreeCourse);
        void Save();
    }
}
