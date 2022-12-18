using BlogProject.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BlogProject.Controllers
{
    public class TeacherController : Controller
    {
        /// <summary>
        /// displays index
        /// </summary>
        /// <returns></returns>
        // /Teacher
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// dislays a list of teachers
        /// </summary>
        /// <param name="SearchKey"></param>
        /// <returns></returns>
        // /Teacher/List
        public ActionResult List(string SearchKey = null)
        {
            TeacherDataController controller = new TeacherDataController();
            IEnumerable<Teacher> teachers = controller.ListTeachers(SearchKey);
            return View(teachers);
        }
        /// <summary>
        /// show the information on a specific teacher
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // /Teacher/Show
        public ActionResult Show(int id)
        {
            TeacherDataController controller = new TeacherDataController();
            Teacher teacher = controller.FindTeacher(id);
            return View(teacher);
        }
        /// <summary>
        /// displays the page for new teacher
        /// </summary>
        /// <returns></returns>
        // /Teacher/New
        public ActionResult New()
        {
            return View();
        }
        /// <summary>
        /// add a teacher by sending a api request to api/TeacherData/AddTeacher 
        /// </summary>
        /// <param name="teacherfname"></param>
        /// <param name="teacherlname"></param>
        /// <param name="employeenumber"></param>
        /// <param name="date"></param>
        /// <param name="salary"></param>
        /// <returns></returns>
        //POST : /Teacher/Create
        [HttpPost]
        public ActionResult Create(string teacherfname, string teacherlname, string employeenumber, DateTime date, decimal salary)
        {
            //Identify that this method is running
            //Identify the inputs provided from the form

            Debug.WriteLine("I have accessed the Create Method!");
            Debug.WriteLine(teacherfname);
            Debug.WriteLine(teacherlname);
            Debug.WriteLine(employeenumber);

            Teacher NewTeacher = new Teacher();
            NewTeacher.teacherfname = teacherfname;
            NewTeacher.teacherlname = teacherlname;
            NewTeacher.enumber = employeenumber;
            NewTeacher.date = date;
            NewTeacher.salary = salary;

            TeacherDataController controller = new TeacherDataController();
            controller.AddTeacher(NewTeacher);

            return RedirectToAction("List");
        }
        /// <summary>
        /// delets a teacher by which id the teacher has
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //POST : /Teacher
        public ActionResult Delete(int id)
        {
            TeacherDataController controller = new TeacherDataController();
            controller.DeleteTeacher(id);
            return RedirectToAction("List");
        }
        public ActionResult Update(int id)
        {
            try
            {
                TeacherDataController teacherdatacontroller = new TeacherDataController();
                Teacher SelectedTeacher = teacherdatacontroller.FindTeacher(id);
                return View(SelectedTeacher);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home");
            }
        }
        /// <summary>
        /// updates the teacher by sending an api request to /api/TeacherData/UpdateTeacher
        /// </summary>
        /// <param name="id"></param>
        /// <param name="teacherfname"></param>
        /// <param name="teacherlname"></param>
        /// <param name="employeenumber"></param>
        /// <param name="date"></param>
        /// <param name="salary"></param>
        /// <returns></returns>
        //POST : /Teacher/Update
        [HttpPost]
        public ActionResult Update(int id, string teacherfname, string teacherlname, string employeenumber, DateTime date, decimal salary)
        {
            Teacher teacher = new Teacher();
            teacher.teacherfname = teacherfname;
            teacher.teacherlname = teacherlname;
            teacher.enumber = employeenumber;
            teacher.date = date;
            teacher.salary = salary;
            TeacherDataController controller = new TeacherDataController();
            controller.UpdateTeacher(teacher, id);
            return RedirectToAction("List");
        }
    }
}