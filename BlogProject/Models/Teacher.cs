using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogProject.Models
{
    public class Teacher
    {
        public int teacherid;
        public string teacherfname;
        public string teacherlname;
        public string enumber;
        public DateTime date;
        public decimal salary;
        public IEnumerable<Course> courses;
    }
}