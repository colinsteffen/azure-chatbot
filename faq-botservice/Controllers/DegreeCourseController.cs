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
        private IDegreeCourseRepository degreeCourseRepository;

        private List<DegreeCourse> degreeCourses;
        private List<Department> departments;
        private List<Module> modules;
        private List<PlaceOfStudy> placesOfStudy;

        public DegreeCourseController()
        {
            degreeCourseRepository = new DegreeCourseRepository();
            degreeCourses = degreeCourseRepository.GetDegreeCourses().ToList();
        }
    }
}
