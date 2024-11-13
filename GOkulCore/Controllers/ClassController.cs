using Data.Context;
using Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace GOkulCore.Controllers
{
    public class ClassController : Controller
    {
        public IActionResult Index(int? id)
        {
            using (Db db = new Db())
            {
                var user = GetUser();
                if (user != null)
                {
                    if (id.HasValue)
                    {
                        ViewBag.ClassId = id;
                    }
                    else
                    {
                        ViewBag.ClassId = 0;
                    }

                    var temp = new
                    {
                        User = user,
                        Classes = db.Class.ToList()
                    };

                    return View(temp);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
        }
         
        public async Task<IActionResult> ClassDetay(int id)
        {
            try
            {
                using (Db db = new Db())
                {
                    var _class = db.Class.Find(id);
                    var classLessons = db.ClassLesson.Where(x => x.ClassId == id).ToList();
                    var lessons = new List<dynamic>();
                    var teachers = new List<dynamic>();
                    var students = new List<dynamic>();

                    var list = new ClassDetails();

                    foreach (var classLesson in classLessons)
                    {
                        var lesson = db.Lesson.Find(classLesson.LessonId);
                        dynamic lessonTemp = new
                        {
                            LessonId = lesson.LessonId,
                            Name = lesson.Name,
                            IsExtra = lesson.IsExtra
                        };

                        if (!lessons.Contains(lessonTemp))
                        {
                            lessons.Add(lessonTemp);
                        }

                        var teacher = db.Teacher.Find(classLesson.LessonId);
                        dynamic teacherTemp = new
                        {
                            TeacherId = teacher.TeacherId,
                            Name = teacher.Name + " " + teacher.Surname,
                            LessonName = teacher.Lesson.Name
                        };

                        if (!teachers.Contains(teacherTemp))
                        {
                            teachers.Add(teacherTemp);
                        }

                        var student = db.Student.Find(classLesson.ClassId);
                        dynamic studentTemp = new
                        {
                            StudentId = student.StudentId,
                            Name = student.Name + " " + student.Surname,
                            ClassName = _class.Name
                        };

                        if (!students.Contains(studentTemp))
                        {
                            students.Add(studentTemp);
                        }
                    }

                    list = new ClassDetails
                    {
                        ClassId = _class.ClassId,
                        Name = _class.Name,
                        Lessons = lessons,
                        Teachers = teachers,
                        Students = students
                    };

                    return PartialView(list);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateClass(Class objClass)
        {
            try
            {
                if (objClass.ClassId != 0)
                {
                    using (Db db = new Db())
                    {
                        var updateClass = db.Class.Find(objClass.ClassId);
                        if (updateClass != null)
                        {
                            updateClass.Name = objClass.Name;
                            db.SaveChanges();
                        }
                    }
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddClass(Class addClass)
        {
            try
            {
                using (Db db = new Db())
                {
                    if (addClass != null && addClass.ClassId == 0)
                    {
                        bool classExists = false;
                        List<Class> classes = db.Class.ToList();
                        foreach (var _class in classes)
                        {
                            if (_class.Name == addClass.Name)
                            {
                                classExists = true;
                                break;
                            }
                        }

                        if (!classExists)
                        {
                            db.Class.Add(addClass);
                            db.SaveChanges();
                            return Ok();
                        }
                        else
                        {
                            return BadRequest("Bu sınıf zaten var!");
                        }
                    }
                    else
                    {
                        return BadRequest("No Data Received! (Cannot add Class)");
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteClass(int id)
        {
            try
            {
                using (Db db = new Db())
                {
                    if (id > 0)
                    {
                        Class removeClass = db.Class.Find(id);
                        var students = db.Student.ToList();
                        foreach (var student in students)
                        {
                            if (student.ClassId == id)
                            {
                                db.Student.Remove(student);
                            }
                        }

                        var teacherClasses = db.TeacherClass.ToList();
                        foreach (var teacher in teacherClasses)
                        {
                            if (teacher.ClassId == id)
                            {
                                db.TeacherClass.Remove(teacher);
                            }
                        }

                        var classLessons = db.ClassLesson.ToList();
                        foreach (var classLesson in classLessons)
                        {
                            if (classLesson.ClassId == id)
                            {
                                db.ClassLesson.Remove(classLesson);
                            }
                        }


                        db.Class.Remove(removeClass);
                        db.SaveChanges();
                        return RedirectToAction("Classes", "Home");
                    }
                    else
                    {
                        return BadRequest("Böyle bir sınıf yok!");
                    }
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
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
    }

    public class ClassDetails
    {
        public int ClassId { get; set; }
        public string Name { get; set; }
        public List<dynamic> Lessons { get; set; }
        public List<dynamic> Teachers { get; set; }
        public List<dynamic> Students { get; set; }
    }
}
