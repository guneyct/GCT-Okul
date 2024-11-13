using Data.Context;
using Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace GOkulCore.Controllers
{
    public class StudentController : Controller
    {
        public IActionResult Index(int? id)
        {
            if (id.HasValue)
            {
                ViewBag.StudentId = id;
            }
            else
            {
                ViewBag.StudentId = 0;
            }

            using (Db db = new Db())
            {
                var students = db.Student.ToList();
                List<dynamic> list = new List<dynamic>();
                foreach (var student in students)
                {
                    var _class = db.Class.Find(student.ClassId);
                    if (_class != null)
                    {
                        student.Class = _class;
                    }

                    var lesson = db.Lesson.Find(student.ExtraLessonId);
                    if (lesson != null)
                    {
                        list.Add(new
                        {
                            StudentId = student.StudentId,
                            Name = student.Name,
                            Surname = student.Surname,
                            Email = student.Email,
                            Password = student.Password,
                            ExtraLessonName = lesson.Name,
                            Class = new
                            {
                                ClassId = student.Class.ClassId,
                                Name = student.Class.Name
                            },
                            Image = student.Image == null ? "-" : student.Image,
                        });
                    }
                    else
                    {
                        list.Add(new
                        {
                            StudentId = student.StudentId,
                            Name = student.Name,
                            Surname = student.Surname,
                            Email = student.Email,
                            Password = student.Password,
                            ExtraLessonName = "-",
                            Class =  new
                            {
                                ClassId = student.Class.ClassId,
                                Name = student.Class.Name
                            },
                            Image = student.Image == null ? "-" : student.Image,
                        });
                    }
                }
                return View(list);
            }
        }

        public IActionResult StudentGrades(int? studentId, int? lessonId)
        {
            var user = GetUser();
            if (user != null)
            {
                if (studentId.HasValue && lessonId.HasValue)
                {
                    ViewBag.StudentId = studentId.Value;
                    ViewBag.LessonId = lessonId.Value;
                }
                else
                {
                    ViewBag.StudentId = 0;
                    ViewBag.LessonId = 0;
                }

                return View(user);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult StudentDetay(int id)
        {
            using (Db db = new Db())
            {
                var student = db.Student.Find(id);
                var classLessons = db.ClassLesson.Where(x => x.ClassId == student.ClassId).ToList();
                var lessons = new List<dynamic>();
                var teachers = new List<dynamic>();

                dynamic list = null;

                foreach (var classLesson in classLessons)
                {
                    var lesson = db.Lesson.Find(classLesson.LessonId);
                    var grade = db.Grade.Where(x => x.LessonId == lesson.LessonId).FirstOrDefault();
                    dynamic lessonTemp = new
                    {
                        LessonId = lesson.LessonId,
                        Name = lesson.Name,
                        IsExtra = lesson.IsExtra,
                        Grade = grade == null ? 0 : grade.LessonGrade
                    };

                    if (!lessons.Contains(lessonTemp))
                    {
                        lessons.Add(lessonTemp);
                    }

                    var teacherClass = db.TeacherClass.Where(x => x.ClassId == student.ClassId).FirstOrDefault();
                    var teacher = db.Teacher.Find(teacherClass.TeacherId);
                    dynamic studentTemp = new
                    {
                        TeacherId = teacher.TeacherId,
                        Name = teacher.Name + " " + student.Surname,
                        LessonName = teacher.Lesson.Name,
                    };

                    if (!teachers.Contains(studentTemp))
                    {
                        teachers.Add(studentTemp);
                    }
                }

                var tempClassesList = db.Class.ToList();
                List<dynamic> tempClasses = new List<dynamic>();
                foreach (var tempClass in tempClassesList)
                {
                    tempClasses.Add(new
                    {
                        ClassId = tempClass.ClassId,
                        Name = tempClass.Name
                    });
                }

                var tempExtraLessons = db.Lesson.Where(x => x.IsExtra == 1).ToList();
                List<dynamic> tempLessonList = new List<dynamic>();
                foreach (var tempLesson in tempExtraLessons)
                {
                    tempLessonList.Add(new
                    {
                        LessonId = tempLesson.LessonId,
                        Name = tempLesson.Name,
                    });
                }
                tempLessonList.Add(new
                {
                    LessonId = 0,
                    Name = "-",
                });

                dynamic tempExtraData = null;
                var tempExtraGrades = db.Lesson.Find(student.ExtraLessonId);
                if (tempExtraGrades != null)
                {
                    var extraGrade = db.Grade.Where(x => x.LessonId == tempExtraGrades.LessonId && x.StudentId == id).FirstOrDefault();
                    if (extraGrade != null)
                    {
                        tempExtraData = new
                        {
                            LessonId = tempExtraGrades.LessonId,
                            Name = tempExtraGrades.Name,
                            IsExtra = tempExtraGrades.IsExtra,
                            LessonGrade = extraGrade.LessonGrade
                        };
                    }
                    else
                    {
                        tempExtraData = new
                        {
                            LessonId = tempExtraGrades.LessonId,
                            Name = tempExtraGrades.Name,
                            IsExtra = tempExtraGrades.IsExtra,
                            LessonGrade = 0
                        };
                    }
                }


                list = new
                {
                    StudentId = student.StudentId,
                    Name = student.Name,
                    Surname = student.Surname,
                    Email = student.Email,
                    Password = student.Password,
                    StudentExtraLesson = student.ExtraLessonId,
                    ClassId = student.ClassId,
                    Lessons = lessons,
                    Teachers = teachers,
                    AllClasses = tempClasses,
                    ExtraLessons = tempLessonList,
                    ExtraGrades = tempExtraData
                };

                return PartialView(list);
            }
        }

        [HttpGet]
        public IActionResult GetStudentGrades(int studentId)
        {
            using (Db db = new Db())
            {
                var student = db.Student.Find(studentId);
                List<dynamic> LessonGrades = new List<dynamic>();

                if (student != null)
                {
                    var lessons = db.ClassLesson.Where(x => x.ClassId == student.ClassId).ToList();

                    foreach (var classLesson in lessons)
                    {
                        var lesson = db.Lesson.Find(classLesson.LessonId);
                        var lessonGrade = db.Grade.Where(x => x.LessonId == lesson.LessonId && x.StudentId == student.StudentId).FirstOrDefault();
                        var grade = 0;
                        if (lessonGrade != null)
                        {
                            grade = lessonGrade.LessonGrade;
                        }

                        dynamic tempLessons = new
                        {
                            LessonId = lesson.LessonId,
                            Name = lesson.Name,
                            IsExtra = lesson.IsExtra,
                            Grade = grade
                        };

                        if (!LessonGrades.Contains(tempLessons))
                        {
                            LessonGrades.Add(tempLessons);
                        }
                    }
                }

                var extraLesson = db.Lesson.Find(student.ExtraLessonId);
                if (extraLesson != null)
                {
                    var extraGrade = db.Grade.Where(x => x.LessonId == extraLesson.LessonId && x.StudentId == student.StudentId).FirstOrDefault();
                    var grade = 0;
                    if (extraGrade != null)
                    {
                        grade = extraGrade.LessonGrade;
                    }

                    dynamic tempLessons = new
                    {
                        LessonId = extraLesson.LessonId,
                        Name = extraLesson.Name,
                        IsExtra = extraLesson.IsExtra,
                        Grade = grade
                    };

                    if (!LessonGrades.Contains(tempLessons))
                    {
                        LessonGrades.Add(tempLessons);
                    }
                }

                var a = Json(LessonGrades);
                return a;
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddStudent(Student addStudent)
        {
            try
            {
                using (Db db = new Db())
                {
                    if (addStudent != null && addStudent.StudentId == 0)
                    {
                        bool teacherExists = false;
                        List<Teacher> teachers = db.Teacher.ToList();
                        foreach (var teacher in teachers)
                        {
                            if ((teacher.Name == addStudent.Name && teacher.Surname == addStudent.Surname) || (teacher.Email == addStudent.Email))
                            {
                                teacherExists = true;
                                break;
                            }
                        }

                        if (!teacherExists)
                        {
                            db.Student.Add(addStudent);
                            db.SaveChanges();
                            return Ok();
                        }
                        else
                        {
                            return BadRequest("Email veya Ad Soyad kontrol edin!");
                        }
                    }
                    else
                    {
                        return BadRequest("No Data Received! (Cannot add Student)");
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Bir hata oluştu");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            try
            {
                using (Db db = new Db())
                {
                    if (id > 0)
                    {
                        var removeStudent = db.Student.Find(id);
                        var studentGrades = db.Grade.ToList();
                        foreach (var item in studentGrades)
                        {
                            if (item.StudentId == id)
                            {
                                db.Grade.Remove(item);
                            }
                        }

                        db.Student.Remove(removeStudent);
                        db.SaveChanges();

                        return RedirectToAction("Teachers", "Home");
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

        [HttpPost]
        public async Task<IActionResult> UpdateStudent(Student objStudent)
        {
            try
            {
                if (objStudent.StudentId != 0)
                {
                    using (Db db = new Db())
                    {
                        var updateStudent = db.Student.Find(objStudent.StudentId);
                        if (updateStudent != null)
                        {
                            updateStudent.Name = objStudent.Name;
                            updateStudent.Surname = objStudent.Surname;
                            updateStudent.Email = objStudent.Email;
                            updateStudent.Password = objStudent.Password;
                            updateStudent.ExtraLessonId = objStudent.ExtraLessonId;
                            updateStudent.ClassId = objStudent.ClassId;

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
        public async Task<IActionResult> UpdateExtra(ExtraData extraData)
        {
            try
            {
                using (Db db = new Db())
                {
                    int studentId = extraData.StudentId;
                    int lessonId = extraData.LessonId;
                    int extraId = extraData.ExtraLessonId;

                    var student = db.Student.Find(studentId);
                    if (student != null)
                    {
                        student.ExtraLessonId = extraId;
                        db.SaveChanges();
                    }
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
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

    public class ExtraData
    {
        public int StudentId { get; set; }
        public int LessonId { get; set; }
        public int ExtraLessonId { get; set; }
    }
}
