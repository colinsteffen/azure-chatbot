using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FAQBot.Helper
{
    public static class IntentHelper
    {
        //QnA Intents
        public const string INTENT_QNA = "q_Faq";

        //Personal Directory Intents
        public const string INTENT_STAFF = "getEmail";
        public const string INTENT_STAFF_EMAIL = "getEmail";
        public const string INTENT_STAFF_PHONENUMBER = "getPhonenumber";
        public const string INTENT_STAFF_ROOM = "getRoom";
        public const string INTENT_STAFF_COURSES = "getCourses";
        public const string INTENT_STAFF_DEPARTMENT = "getDepartment";
        public const string INTENT_STAFF_OFFICE_HOURS = "getOfficeHours";
        public const string INTENT_STAFF_PUBLICATIONS = "getPublications";
        public const string INTENT_STAFF_STAFF_FROM_DEPARTMENT = "getStaffFromDepartment";

        //Courses Intents
        public const string INTENT_COURSES = "getCourses";
        public const string INTENT_COURSES_COURSES_ALL = "getCourses";
        public const string INTENT_COURSES_COURSE = "getCourse";
        public const string INTENT_COURSES_MODULES = "getModules";
        public const string INTENT_COURSES_MODULE_COMMISSIONER = "getModuleCommissioner";
        public const string INTENT_COURSES_MODULE_CONTENT = "getModuleContent";
        public const string INTENT_COURSES_MODULE_INFORMATION = "getModuleInformation";
        public const string INTENT_COURSES_MODULE_LANGUAGE = "getModuleLanguage";
        public const string INTENT_COURSES_METHOD_OF_EXAMINATION = "getModuleMethodOfExamination";
        public const string INTENT_COURSES_APPLY_INFORMATION = "getApplyInformation";

        //Event Intent
        public const string INTENT_EVENTS = "getEvents";
        public const string INTENT_EVENTS_EVENTS_ALL = "getEvents"; //TODO

        //Functionality Intents
        public const string INTENT_DESCRIBE_FUNCTIONALITY_FAQ = "FAQ";
        public const string INTENT_DESCRIBE_FUNCTIONALITY_PERSONAL_DIRECTORY = "PERSONENVERZEICHNIS";
        public const string INTENT_DESCRIBE_FUNCTIONALITY_EVENTS = "EVENTS"; //TODO
        public const string INTENT_DESCRIBE_FUNCTIONALITY_COURSES = "COURSES"; 
    }
}
