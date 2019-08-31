using FAQBot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FAQBot.Repository
{
    public interface IModuleRepository
    {
        IEnumerable<Module> GetModules();
    }
}
