using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EchoBot.Model
{
    public class DegreeCourse
    {
        [Key]
        public int Id { get; set; }
        public string DegreeLevel { get; set; }
        public string StudyModel { get; set; }
        public int DurationOfStudy { get; set; }
        public int DepartmentId { get; set; }
        public int PlaceOfStudyId { get; set; }
        public List<int> ModuleIds { get; set; }
        public string Title { get; set; }
        public bool NumerusClausus { get; set; }
    }
}
