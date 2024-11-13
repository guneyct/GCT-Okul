using Data.Context;
using Data.Entities;
using GOkulCore.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GOkulCore.Controllers
{
    public class HomeController : Controller
    {
        new dynamic? User = null;

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult ProfileSettings()
        {
            var user = GetUser();
            if (user != null)
            {
                if (user.Type == "Öğrenci")
                {
                    using (Db db = new Db())
                    {
                        var studentClass = db.Class.Find(user.ClassId);
                        ViewBag.ClassName = studentClass.Name;
                    }
                }
                return View(user);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        public IActionResult Index()
        {
            return View(GetAllTableCounts());
        }

        public IActionResult AddRecord(string? selectedType)
        {
            using (Db db = new Db())
            {
                var user = GetUser();
                if (user != null)
                {
                    ViewBag.SelectedType = selectedType;
                    dynamic temp = null;
                    switch (selectedType)
                    {
                        case "Class":
                            temp = new {
                                User = user,
                                Classes = db.Class.ToList()
                            };
                            break;
                        case "Lesson":
                            temp = new
                            {
                                User = user,
                                Classes = db.Class.ToList()
                            };
                            break;
                        case "Teacher":
                            temp = new
                            {
                                User = user,
                                Classes = db.Class.ToList(),
                                Lessons = db.Lesson.ToList()
                            };
                            break;
                        case "Student":
                            temp = new
                            {
                                User = user,
                                Classes = db.Class.ToList(),
                                ExtraLessons = db.Lesson.Where(x=> x.IsExtra == 1).ToList()
                            };
                            break;
                    }
                    return View(temp);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
        }

        [HttpPost]
        public IActionResult UpdateCredentials(string Email, string Password, string Image)
        {
            var user = GetUser();
            if (user != null)
            {
                using (Db db = new Db())
                {
                    string? userType = HttpContext.Session.GetString("Type");
                    switch (userType)
                    {
                        case "Admin":
                            Admin admin = db.Admin.Find(user.AdminId);
                            admin.Email = Email;
                            admin.Password = Password;
                            admin.Image = Image;
                            break;
                        case "Teacher":
                            Teacher teacher = db.Teacher.Find(user.TeacherId);
                            teacher.Email = Email;
                            teacher.Password = Password;
                            teacher.Image = Image;
                            break;
                        case "Student":
                            Student student = db.Student.Find(user.StudentId);
                            student.Email = Email;
                            student.Password = Password;
                            student.Image = Image;
                            break;
                    }
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult UpdateProfile(string Name, string Surname)
        {
            var user = GetUser();
            if (user != null)
            {
                using (Db db = new Db())
                {
                    string? userType = HttpContext.Session.GetString("Type");
                    switch (userType)
                    {
                        case "Admin":
                            Admin admin = db.Admin.Find(user.AdminId);
                            admin.Name = Name;
                            admin.Surname = Surname;
                            break;
                        case "Teacher":
                            Teacher teacher = db.Teacher.Find(user.TeacherId);
                            teacher.Name = Name;
                            teacher.Surname = Surname;
                            break;
                        case "Student":
                            Student student = db.Student.Find(user.StudentId);
                            student.Name = Name;
                            student.Surname = Surname;
                            break;
                    }
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Index", "Home");
        }

        public dynamic GetAllTableCounts()
        {
            using (Db db = new Db())
            {
                var classes = db.Class.ToList();
                var lessons = db.Lesson.ToList();
                var teachers = db.Teacher.ToList();
                var students = db.Student.ToList();

                List<dynamic> classData = new List<dynamic>();
                foreach (var item in classes)
                {
                    var classLesson = db.ClassLesson.Where(x => x.ClassId == item.ClassId).ToList();
                    var teacherClasses = db.TeacherClass.Where(x => x.ClassId == item.ClassId).ToList();
                    var classStudents = db.Student.Where(x => x.ClassId == item.ClassId).ToList();

                    dynamic tempData = new
                    {
                        ClassName = item.Name,
                        LessonCount = classLesson.Count,
                        TeacherCount = teacherClasses.Count,
                        StudentCount = classStudents.Count
                    };

                    if (!classData.Contains(tempData))
                    {
                        classData.Add(tempData);
                    }
                }

                List<dynamic> extraLessonsData = new List<dynamic>();
                foreach (var lesson in lessons)
                {
                    dynamic tempData = null;
                    if (lesson.IsExtra == 1)
                    {
                        dynamic lessonTeacher = db.Teacher.Where(x => x.LessonId == lesson.LessonId).FirstOrDefault();
                        var teacherName = "Atanmadı";
                        if (lessonTeacher != null)
                        {
                            teacherName = lessonTeacher.Name + " " + lessonTeacher.Surname;
                        }

                        var lessonStudentCount = db.Student.Where(x => x.ExtraLessonId == lesson.LessonId).ToList().Count;
                        tempData = new
                        {
                            LessonName = lesson.Name,
                            LessonTeacher = teacherName,
                            StudentCount = lessonStudentCount
                        };

                        if (!extraLessonsData.Contains(tempData))
                        {
                            extraLessonsData.Add(tempData);
                        }
                    }
                }

                List<dynamic> lessonsData = new List<dynamic>();
                var classLessons = db.ClassLesson.ToList();
                foreach (var classLesson in classLessons)
                {
                    dynamic tempData = null;
                    var _class = db.Class.Find(classLesson.ClassId);
                    var lesson = db.Lesson.Find(classLesson.LessonId);

                    if (lesson?.IsExtra == 0)
                    {
                        var teacherClasses = db.TeacherClass.Where(x => x.ClassId == _class.ClassId).ToList();
                        Teacher lessonTeacher = null;
                        foreach (var item in teacherClasses)
                        {
                            var tempTeacher = db.Teacher.Find(item.TeacherId);
                            if (tempTeacher?.LessonId == lesson.LessonId)
                            {
                                lessonTeacher = tempTeacher;
                                break;
                            }
                        }

                        var teacherName = "Atanmadı";
                        if (lessonTeacher != null)
                        {
                            teacherName = lessonTeacher.Name + " " + lessonTeacher.Surname;
                        }

                        var studentCount = db.Student.Where(x=> x.ClassId == _class.ClassId).ToList().Count();

                        tempData = new
                        {
                            LessonName = lesson.Name,
                            ClassName = _class.Name,
                            TeacherName = teacherName,
                            StudentCount = studentCount
                        };

                        if (!lessonsData.Contains(tempData))
                        {
                            lessonsData.Add(tempData);
                        }
                    }

                }

                dynamic tableCounts = new
                {
                    ClassesCount = classes.Count,
                    LessonsCount = lessons.Count,
                    TeachersCount = teachers.Count,
                    StudentsCount = students.Count,
                    ClassesData = classData,
                    ExtraLessonsData = extraLessonsData,
                    LessonsData = lessonsData
                };

                return tableCounts;
            }
        }

        public dynamic GetUser()
        {
            int? userId = HttpContext.Session.GetInt32("Id");
            string? userType = HttpContext.Session.GetString("UserNavBar");

            if (userId != null && userType != null)
            {
                using (Db db = new Db())
                {
                    switch (userType)
                    {
                        case "Admin":
                            return db.Admin.Find(userId);
                        case "Teacher":
                            var teacher = db.Teacher.Find(userId);
                            var teacherLesson = db.Lesson.Find(teacher.LessonId);
                            dynamic tempUser = new
                            {
                                TeacherId = teacher.TeacherId,
                                Name = teacher.Name,
                                Surname = teacher.Surname,
                                Email = teacher.Email,
                                Password = teacher.Password,
                                Image = teacher.Image,
                                LessonId = teacher.LessonId,
                                LessonName = teacherLesson.Name,
                                Type = teacher.Type + " - " + teacherLesson.Name
                            };

                            return tempUser;

                        case "Student":
                            return db.Student.Find(userId);
                        default:
                            return null;
                    }
                }
            }
            return null;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}