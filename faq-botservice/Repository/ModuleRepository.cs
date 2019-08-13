using EchoBot.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EchoBot.Repository
{
    public class ModuleRepository : IModuleRepository
    {
        private const string jsonDbFile = "Db/Modules.json";

        public IEnumerable<Module> GetModules()
        {
            using (StreamReader r = new StreamReader(jsonDbFile))
            {
                string json = r.ReadToEnd();
                return JsonConvert.DeserializeObject<List<Module>>(json);
            }
        }
    }
}
