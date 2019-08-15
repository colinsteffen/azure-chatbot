using EchoBot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EchoBot.Dialogs
{
    public static class DegreeCourseIntentDialogs
    {
        public static string DialogGetDegreeCourses(List<DegreeCourse> dcs)
        {
            string output;

            if (dcs.Count == 0) return "Leider konnte kein Studiengang zu Ihrer Eingabe gefunden werden.";
            else if (dcs.Count == 1) output = "Der folgende Studiengang wurde zu Ihrer Eingabe gefunden: \n";
            else output = "Die folgenden Studiengänge wurden zu Ihrer Eingabe gefunden: \n";
            
            foreach (DegreeCourse dc in dcs)
                output += $"- {dc.Title} ({dc.StudyModel}) \n";

            return output;
        }
    }
}
