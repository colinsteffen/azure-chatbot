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
        public Degree Degree { get; set; }
        public int DurationOfStudy { get; set; }
        public Department Department { get; set; }
        public PlaceOfStudy PlaceOfStudy { get; set; }
    }
}
