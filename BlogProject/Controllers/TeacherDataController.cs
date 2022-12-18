using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.Http;
using BlogProject.Models;
using MySql.Data.MySqlClient;

namespace BlogProject.Controllers
{

    public class TeacherDataController : ApiController
    {
        private SchoolDbContext Blog = new SchoolDbContext();
        /// <summary>
        /// api/TeacherData/List/{SearchKey?}
        /// returns a list of teacher from the sql database with a filter option
        /// </summary>
        /// <param name="SearchKey">the search key will be used filter teachers by searching for any instances of the searchkey in teachers first and last name</param>
        /// <returns>returns a list of teachers that matches with the searchkey</returns>
        [HttpGet]
        [Route("api/TeacherData/List/{SearchKey?}")]
        public IEnumerable<Teacher> ListTeachers(string SearchKey)
        {
            //Create an instance of a connection
            MySqlConnection Conn = Blog.AccessDatabase();

            //Open the connection between the web server and database
            Conn.Open();

            //Establish a new command (query) for our database
            MySqlCommand cmd = Conn.CreateCommand();

            //SQL QUERY
            if (SearchKey == null)
            {
                cmd.CommandText = "Select * from teachers";
            }
            else
            {
                cmd.CommandText = "Select * from teachers Where CONCAT(teacherfname, teacherlname) LIKE '%" + SearchKey + "%'";
            }


            //Gather Result Set of Query into a variable
            MySqlDataReader ResultSet = cmd.ExecuteReader();

            //Create an empty list of Author Names
            List<Teacher> TeacherNames = new List<Teacher> { };

            //Loop Through Each Row the Result Set
            while (ResultSet.Read())
            {
                //Access Column information by the DB column name as an index
                int id = (int)ResultSet["teacherid"];
                string fName = (string)ResultSet["teacherfname"];
                string lName = (string)ResultSet["teacherlname"];
                string eNumber = (string)ResultSet["employeenumber"];
                DateTime hireDate = (DateTime)ResultSet["hiredate"];
                //System.Diagnostics.Debug.WriteLine(ResultSet["salary"]);
                decimal salary = (decimal)ResultSet["salary"];
                Teacher teacher = new Teacher();
                teacher.teacherfname = fName;
                teacher.teacherlname = lName;
                teacher.teacherid = id;
                teacher.enumber = eNumber;
                teacher.date = hireDate;
                teacher.salary = salary;



                //Add the Author Name to the List
                TeacherNames.Add(teacher);
            }

            //Close the connection between the MySQL Database and the WebServer
            Conn.Close();

            //Return the final list of author names
            return TeacherNames;
        }
        /// <summary>
        /// api/TeacherData/FindTeacher/{id}
        /// will get that teacher's data for the given id
        /// </summary>
        /// <param name="id">teachers id</param>
        /// <returns>a teachers information for the given id</returns>
        [HttpGet]
        public Teacher FindTeacher(int id)
        {

            //Create an instance of a connection
            MySqlConnection Conn = Blog.AccessDatabase();

            //Open the connection between the web server and database
            Conn.Open();

            //Establish a new command (query) for our database
            MySqlCommand cmd = Conn.CreateCommand();

            //SQL QUERY
            cmd.CommandText = "Select * from teachers Where teacherid=" + id;

            //Gather Result Set of Query into a variable
            MySqlDataReader ResultSet = cmd.ExecuteReader();
            ResultSet.Read();
            //Create an empty list of Author Names
            Teacher teacher = new Teacher();
            string fName = (string)ResultSet["teacherfname"];
            string lName = (string)ResultSet["teacherlname"];
            string eNumber = (string)ResultSet["employeenumber"];
            DateTime hireDate = (DateTime)ResultSet["hiredate"];
            //System.Diagnostics.Debug.WriteLine(ResultSet["salary"]);
            decimal salary = (decimal)ResultSet["salary"];
            teacher.teacherfname = fName;
            teacher.teacherlname = lName;
            teacher.teacherid = id;
            teacher.enumber = eNumber;
            teacher.date = hireDate;
            teacher.salary = salary;
            Conn.Close();
            // had to close and open again to make another query not sure why
            Conn.Open();
            cmd.CommandText = "Select * from classes Where teacherid=" + id;
            ResultSet = cmd.ExecuteReader();
            System.Diagnostics.Debug.WriteLine("test");
            List<Course> courses = new List<Course> { };
            while (ResultSet.Read())
            {
                System.Diagnostics.Debug.WriteLine("test");
                System.Diagnostics.Debug.WriteLine(ResultSet["classname"]);
                Course course = new Course();
                string className = (string)ResultSet["classname"];
                string classCode = (string)ResultSet["classcode"];
                course.className = className;
                course.classCode = classCode;
                courses.Add(course);
            }
            teacher.courses = courses;

            return teacher;

        }
        /// <summary>
        /// creates a new teacher based on the id input and validate that the input fields contain valid data
        /// </summary>
        /// <param name="newTeacher"></param>
        /// <returns>returns 200 if the teacher has been successfully added and 400 if there was invalid inputs</returns>
        [HttpPost]
        public int AddTeacher([FromBody]Teacher newTeacher)
        {

            //validating data
            if(!Regex.IsMatch(newTeacher.teacherfname, @"^[a-zA-Z]+$") || 
                !Regex.IsMatch(newTeacher.teacherlname, @"^[a-zA-Z]+$") || 
                !Regex.IsMatch(newTeacher.enumber, @"^T[0-9]{3}$"))
            {

                return (400);
            }
            //Create an instance of a connection
            //string teacherfname, string teacherlname, string enumber, DateTime date, float salary
            MySqlConnection Conn = Blog.AccessDatabase();

            //Open the connection between the web server and database
            Conn.Open();

            //Establish a new command (query) for our database
            MySqlCommand cmd = Conn.CreateCommand();
            //SQL QUERY
            cmd.CommandText = "INSERT INTO teachers (teacherfname,teacherlname, employeenumber, hiredate, salary)" +
                "VALUES (@teacherfname, @teacherlname, @enumber, @date, @salary)";
            cmd.Parameters.AddWithValue("@teacherfname", newTeacher.teacherfname);
            cmd.Parameters.AddWithValue("@teacherlname", newTeacher.teacherlname);
            cmd.Parameters.AddWithValue("@enumber", newTeacher.enumber);
            cmd.Parameters.AddWithValue("@date", newTeacher.date);
            cmd.Parameters.AddWithValue("@salary", newTeacher.salary);
            MySqlDataReader ResultSet = cmd.ExecuteReader();
            //System.Diagnostics.Debug.WriteLine(ResultSet);
            Conn.Close();
            return (200);
        }
        /// <summary>
        /// delets the teacher based on which id is given
        /// </summary>
        /// <param name="id"></param>
        [HttpPost]
        public void DeleteTeacher(int id)
        {
            //Create an instance of a connection
            MySqlConnection Conn = Blog.AccessDatabase();

            //Open the connection between the web server and database
            Conn.Open();

            //Establish a new command (query) for our database
            MySqlCommand cmd = Conn.CreateCommand();
            cmd.CommandText = "DELETE FROM teachers WHERE teacherid=@id";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Prepare();

            cmd.ExecuteNonQuery();

            Conn.Close();
        }
        /// <summary>
        /// this endpoint will update a teacher based on the id input and validate that the input fields contain valid data
        /// </summary>
        /// <param name="newTeacher"></param>
        /// <param name="id"></param>
        /// <returns>200 if the teacher has been successfully updated and 400 if there was invalid inputs</returns>
        [HttpPost]
        public int UpdateTeacher([FromBody]Teacher newTeacher, int id)
        {
            //validating data this will also prevent sql injection attacks beacuse it will only allow very specific characters to be passed into an sql query
            if (!Regex.IsMatch(newTeacher.teacherfname, @"^[a-zA-Z]+$") ||
                !Regex.IsMatch(newTeacher.teacherlname, @"^[a-zA-Z]+$") ||
                !Regex.IsMatch(newTeacher.enumber, @"^T[0-9]{3}$"))
            {

                return (400);
            }
            if (newTeacher.salary == 0)
            {
                return (400);
            }
            //Create an instance of a connection
            MySqlConnection Conn = Blog.AccessDatabase();


            //Open the connection between the web server and database
            Conn.Open();
            MySqlCommand cmd = Conn.CreateCommand();
            cmd.CommandText = "UPDATE teachers SET teacherfname=@teacherfname, teacherlname=@teacherlname, employeenumber=@enumber, hiredate=@date, salary=@salary WHERE teacherid=@id";
            cmd.Parameters.AddWithValue("@teacherfname", newTeacher.teacherfname);
            cmd.Parameters.AddWithValue("@teacherlname", newTeacher.teacherlname);
            cmd.Parameters.AddWithValue("@enumber", newTeacher.enumber);
            cmd.Parameters.AddWithValue("@date", newTeacher.date);
            cmd.Parameters.AddWithValue("@salary", newTeacher.salary);
            cmd.Parameters.AddWithValue("@id", id);
            MySqlDataReader ResultSet = cmd.ExecuteReader();

            return (200);
        }
    }
}
