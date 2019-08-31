using FAQBot.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FAQBot.Repository
{
    public class PlaceOfStudyRepository : IPlaceOfStudyRepository
    {
        private const string jsonDbFile = "Db/PlacesOfStudy.json";

        public IEnumerable<PlaceOfStudy> GetPlacesOfStudy()
        {
            using (StreamReader r = new StreamReader(jsonDbFile))
            {
                string json = r.ReadToEnd();
                return JsonConvert.DeserializeObject<List<PlaceOfStudy>>(json);
            }
        }
    }
}
