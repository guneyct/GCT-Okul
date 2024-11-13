using Data.Context;
using Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace GOkulCore.Controllers
{
    public class LessonController : Controller
    {
        public IActionResult Index(int? id)
        {
            using (Db db = new Db())
            {
                if (id.HasValue)
                {
                    ViewBag.LessonId = id.Value;
                }
                else
                {
                    ViewBag.LessonId = 0;
                }

                List<Lesson> Lessons = db.Lesson.ToList();
                return View(Lessons);
            }
        }
         
        public async Task<IActionResult> LessonDetay(int id)
        {
            try
            {
                using (Db db = new Db())
                {
                    var lesson = db.Lesson.Find(id);
                    var classLessons = db.ClassLesson.Where(x => x.LessonId == lesson.LessonId).ToList();
                    var classes = new List<dynamic>();
                    var teachers = new List<dynamic>();
                    var students = new List<dynamic>();
                    var lessonTeacher = db.Teacher.Where(x => x.LessonId == lesson.LessonId).FirstOrDefault();

                    LessonDetails list;

                    dynamic tempLessonTeach = new
                    {
                        TeacherId = 0,
                        Name = "Atanmamış",
                        LessonName = lesson.Name
                    };

                    if (lessonTeacher != null)
                    {
                        tempLessonTeach = new
                        {
                            TeacherId = lessonTeacher.TeacherId,
                            Name = lessonTeacher.Name + " " + lessonTeacher.Surname,
                            LessonName = lessonTeacher.Lesson.Name
                        };

                        teachers.Add(tempLessonTeach);

                    }


                    if (classLessons.Count > 0)
                    {
                        foreach (var classLesson in classLessons)
                        {
                            var _class = db.Class.Find(classLesson.ClassId);
                            dynamic _classTemp = new
                            {
                                ClassId = _class.ClassId,
                                Name = _class.Name,
                            };

                            if (!classes.Contains(_classTemp))
                            {
                                classes.Add(_classTemp);
                            }

                            var teacher = db.Teacher.Find(classLesson.LessonId);
                            var gctLesson = db.Lesson.Find(classLesson.LessonId);
                            dynamic teacherTemp = new
                            {
                                TeacherId = 0,
                                Name = "Atanmamış",
                                LessonName = gctLesson.Name
                            };
                            if (teacher != null)
                            {
                                teacherTemp = new
                                {
                                    TeacherId = teacher.TeacherId,
                                    Name = teacher.Name + " " + teacher.Surname,
                                    LessonName = teacher.Lesson.Name
                                };

                                if (!teachers.Contains(teacherTemp))
                                {
                                    teachers.Add(teacherTemp);
                                }
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
                    }
                    else
                    {
                        List<Student> extraStudents = db.Student.Where(x => x.ExtraLessonId == lesson.LessonId).ToList();
                        foreach (var student in extraStudents)
                        {
                            var _class = db.Class.Find(student.ClassId);
                            students.Add(new
                            {
                                StudentId = student.StudentId,
                                Name = student.Name + " " + student.Surname,
                                ClassName = _class.Name
                            });
                        }

                        classes.Add(new
                        {
                            ClassId = 0,
                            Name = "-"
                        });
                    }

                    list = new LessonDetails
                    {
                        LessonId = lesson.LessonId,
                        Name = lesson.Name,
                        IsExtra = lesson.IsExtra,
                        Classes = classes,
                        Teachers = teachers,
                        Students = students,
                        LessonTeacher = tempLessonTeach
                    };


                    return PartialView(list);
                }
            }
            catch (Exception ex)
            {
                BadRequest(ex.Message);
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateLesson(Lesson objLesson)
        {
            try
            {
                if (objLesson.LessonId != 0)
                {
                    using (Db db = new Db())
                    {
                        var updateLesson = db.Lesson.Find(objLesson.LessonId);
                        if (updateLesson != null)
                        {
                            updateLesson.Name = objLesson.Name;
                            updateLesson.IsExtra = objLesson.IsExtra;

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
        public async Task<IActionResult> AddLesson(Lesson addLesson)
        {
            try
            {
                using (Db db = new Db())
                {
                    if (addLesson != null && addLesson.LessonId == 0)
                    {
                        bool lessonExists = false;
                        List<Lesson> lessons = db.Lesson.ToList();
                        foreach (var lesson in lessons)
                        {
                            if (lesson.Name == addLesson.Name)
                            {
                                lessonExists = true;
                                break;
                            }
                        }

                        if (!lessonExists)
                        {
                            int selectedClass = addLesson.IsExtra;
                            if (selectedClass == 0)
                            {
                                addLesson.IsExtra = 1;
                            }
                            else
                            {
                                addLesson.IsExtra = 0;
                                db.Lesson.Add(addLesson);
                                db.SaveChanges();


                                ClassLesson addLessonToClass = new ClassLesson();
                                addLessonToClass.LessonId = addLesson.LessonId;
                                addLessonToClass.ClassId = selectedClass;
                                db.ClassLesson.Add(addLessonToClass);

                                db.SaveChanges();
                            }

                            return Ok();
                        }
                        else
                        {
                            return BadRequest("Bu ders zaten ekli!");
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
        public async Task<IActionResult> DeleteLesson(int id)
        {
            try
            {
                using (Db db = new Db())
                {
                    if (id > 0)
                    {
                        var removeLesson = db.Lesson.Find(id);

                        var classLessons = db.ClassLesson.ToList();
                        foreach (var item in classLessons)
                        {
                            if (item.LessonId == id)
                            {
                                db.ClassLesson.Remove(item);
                            }
                        }

                        var students = db.Student.ToList();
                        foreach (var student in students)
                        {
                            if (student.ExtraLessonId == id)
                            {
                                student.ExtraLessonId = 0;
                            }
                        }

                        var teachers = db.Teacher.ToList();
                        foreach (var item in teachers)
                        {
                            if (item.LessonId == id)
                            {
                                db.Teacher.Remove(item);
                            }
                        }

                        db.Lesson.Remove(removeLesson);
                        db.SaveChanges();

                        return RedirectToAction("Lessons", "Home");
                    }
                    else
                    {
                        return BadRequest("Böyle bir ders yok!");
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

    public class LessonDetails
    {
        public int LessonId { get; set; }
        public string Name { get; set; }
        public int IsExtra { get; set; }
        public List<dynamic> Classes { get; set; }
        public List<dynamic> Teachers { get; set; }
        public List<dynamic> Students { get; set; }
        public dynamic LessonTeacher { get; set; }
    }
}
