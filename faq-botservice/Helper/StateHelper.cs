using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FAQBot.Helper
{
    public static class StateHelper
    {
        public static class DegreeCourseIntent
        {
            public static bool WaitingForInformation = false;
            public static string WaitingMethod = "";
            public static string WaitingVariable = "";

            public static string LastModuleId = "";
            public static string LastDegreeCourseTitle = "";
            public static int LastDegreeCourseId = -1;
            public static int LastDepartmentId = -1;
            public static string LastDegreeLevel = "";
            public static string LastStudyModel = "";
        }
    }
}
