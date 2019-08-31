using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FAQBot.Model
{
    public class PlaceOfStudy
    {
        public int Id { get; set; }
        public string Street { get; set; }
        public string HouseNumber { get; set; }
        public int Postcode { get; set; }
        public string City { get; set; }
        public string Name { get; set; }
    }
}
