using EchoBot.Model;
using EchoBot.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EchoBot.Controllers
{
    public class DegreeCourseController
    {
        private List<DegreeCourse> degreeCourses;
        private IDegreeCourseRepository repository;

        public DegreeCourseController()
        {
            repository = new DegreeCourseRepository();
            degreeCourses = repository.GetDegreeCourses().ToList();
        }
    }
}
