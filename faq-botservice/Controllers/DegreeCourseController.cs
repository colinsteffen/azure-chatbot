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
        private IDepartmentRepository departmentRepository;
        private IModuleRepository moduleRepository;
        private IPlaceOfStudyRepository placeOfStudyRepository;

        private List<DegreeCourse> degreeCourses;
        private List<Department> departments;
        private List<Module> modules;
        private List<PlaceOfStudy> placesOfStudy;

        public DegreeCourseController()
        {
            degreeCourseRepository = new DegreeCourseRepository();
            departmentRepository = new DepartmentRepository();
            moduleRepository = new ModuleRepository();
            placeOfStudyRepository = new PlaceOfStudyRepository();

            degreeCourses = degreeCourseRepository.GetDegreeCourses().ToList();
            departments = departmentRepository.GetDepartments().ToList();
            modules = moduleRepository.GetModules().ToList();
            placesOfStudy = placeOfStudyRepository.GetPlacesOfStudy().ToList();
        }
    }
}
