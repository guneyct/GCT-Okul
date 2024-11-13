using Microsoft.AspNetCore.Mvc;
using Data.Context;
using Data.Entities;
using Microsoft.AspNetCore.Http;

namespace GOkulCore.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            string? userEmail = HttpContext.Session.GetString("UserEmail");
            string? userPassword = HttpContext.Session.GetString("UserPassword");

            if (userEmail != null && userPassword != null)
            {
                using (Db db = new Db())
                {
                    var admin = db.Admin.Where(x => x.Email == userEmail && x.Password == userPassword).ToList();
                    var teacher = db.Teacher.Where(x => x.Email == userEmail && x.Password == userPassword).ToList();
                    var student = db.Student.Where(x => x.Email == userEmail && x.Password == userPassword).ToList();

                    if (admin.Count > 0)
                    {
                        HttpContext.Session.SetInt32("Id", admin.First().AdminId);
                        HttpContext.Session.SetString("Type", "Admin");
                        return RedirectToAction("Index", "Home");
                    }
                    else if (teacher.Count > 0)
                    {
                        HttpContext.Session.SetInt32("Id", teacher.First().TeacherId);
                        HttpContext.Session.SetString("Type", "Teacher");
                        return RedirectToAction("Index", "Home");
                    }
                    else if (student.Count > 0)
                    {
                        HttpContext.Session.SetInt32("Id", student.First().StudentId);
                        HttpContext.Session.SetString("Type", "Student");
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return View();
                    }
                }
            }
            else
            {
                return View();
            }
        }

        public IActionResult Entering(string Email, string Password)
        {
            using (Db db = new Db())
            {
                var admin = db.Admin.Where(x => x.Email == Email && x.Password == Password).ToList();
                var teacher = db.Teacher.Where(x => x.Email == Email && x.Password == Password).ToList();
                var student = db.Student.Where(x => x.Email == Email && x.Password == Password).ToList();

                if (admin.Count > 0)
                {
                    string name = admin.First().Name + " " + admin.First().Surname;
                    HttpContext.Session.SetInt32("Id", admin.First().AdminId);
                    HttpContext.Session.SetString("Name", name);
                    HttpContext.Session.SetString("UserNavBar", "Admin");
                    HttpContext.Session.SetString("Type", admin.First().Type);


                    return RedirectToAction("Index", "Home");
                }
                else if (teacher.Count > 0)
                {
                    string name = teacher.First().Name + " " + teacher.First().Surname;

                    HttpContext.Session.SetInt32("Id", teacher.First().TeacherId);
                    HttpContext.Session.SetString("UserNavBar", "Teacher");
                    HttpContext.Session.SetString("Name", name);


                    var teachLesson = db.Lesson.Find(teacher.First().LessonId);
                    string _type = teacher.First().Type + " - " + teachLesson.Name;
                    HttpContext.Session.SetString("Type", _type);

                    return RedirectToAction("Index", "Home");
                }
                else if (student.Count > 0)
                {
                    string name = student.First().Name + " " + student.First().Surname;

                    HttpContext.Session.SetInt32("Id", student.First().StudentId);
                    HttpContext.Session.SetString("Name", name);
                    HttpContext.Session.SetString("UserNavBar", "Student");


                    var studentClass = db.Class.Find(student.First().ClassId);
                    string _type = teacher.First().Type + " - " + studentClass.Name;
                    HttpContext.Session.SetString("Type", _type);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
        }

        public IActionResult Exit()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
