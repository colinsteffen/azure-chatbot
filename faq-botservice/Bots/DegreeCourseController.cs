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

        //Degree Courses
        public List<DegreeCourse> GetFilteredDegreeCourses(int departmentId, string degreeLevel)
        {
            List<DegreeCourse> degreeCoursesFiltered = new List<DegreeCourse>();
            degreeCoursesFiltered.AddRange(degreeCourses);

            if(departmentId >= 0)
            {
                degreeCoursesFiltered = (from dc in degreeCoursesFiltered
                                         where departmentId == dc.DepartmentId
                                         select dc).ToList();
            }
            if(!string.IsNullOrEmpty(degreeLevel))
            {
                degreeCoursesFiltered = (from dc in degreeCoursesFiltered
                                         where degreeLevel.ToLower().Equals(dc.DegreeLevel.ToLower())
                                         select dc).ToList();
            }

            return degreeCoursesFiltered;
        }

        public int GetDegreeCourseIdFromName(string name)
        {
            List<int> ids = (from dc in degreeCourses
                             where name.Equals(dc.Title)
                             select dc.Id).ToList();
            return ids.First();
        }

        //Department
        public int GetDepartmentIdFromName(string name)
        {
            List<int> ids = (from dc in departments
                             where dc.Name.ToLower().Contains(name.ToLower())
                             select dc.Id).ToList();
            return ids.First();
        }

        //Modules
        public string getModuleIdFromName(string name)
        {
            List<string> ids = (from m in modules
                             where m.Title.ToLower().Contains(name.ToLower())
                             select m.Id).ToList();
            return ids.First();
        }

        public Module getModule(string id)
        {
            List<Module> moduleList = (from m in modules
                                where m.Id.ToLower().Contains(id.ToLower())
                                select m).ToList();
            return moduleList.First();
        }
    }
}
