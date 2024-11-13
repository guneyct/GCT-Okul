using Data.Context;
using Data.Entities;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;

namespace GOkulCore.Controllers
{
    public class TeacherController : Controller
    {
        public IActionResult Index(int? id)
        {
            using (Db db = new Db())
            {
                if (id.HasValue)
                {
                    ViewBag.TeacherId = id;
                }
                else
                {
                    ViewBag.TeacherId = 0;
                }

                List<Teacher> teachers = db.Teacher.ToList();
                foreach (var teacher in teachers)
                {
                    Lesson teacherLesson = db.Lesson.Find(teacher.LessonId);
                    teacher.Lesson = teacherLesson;
                }
                return View(teachers);
            }
        }

        public IActionResult TeacherStudents(int? studentId, int? lessonId)
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

                return View(GetLessonStudents(user));
            }
            else
            {
                return BadRequest("Kullanıcı yok!");
            }
        }

        [HttpGet]
        public IActionResult TeacherDetay(int id)
        {
            using (Db db = new Db())
            {
                var teacher = db.Teacher.Find(id);
                var classLessons = db.ClassLesson.Where(x => x.LessonId == teacher.LessonId).ToList();
                var classes = new List<dynamic>();
                var students = new List<dynamic>();

                TeacherDetails list;

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

                var allLessons = db.Lesson.ToList();
                var tempLessons = new List<dynamic>();
                foreach (var lesson in allLessons)
                {
                    var classLesson = db.ClassLesson.Where(x=> x.LessonId == lesson.LessonId).FirstOrDefault();
                    if (classLesson != null)
                    {
                        var _class = db.Class.Find(classLesson.ClassId);
                        tempLessons.Add(new
                        {
                            LessonId = lesson.LessonId,
                            Name = lesson.Name,
                            IsExtra = lesson.IsExtra,
                            ClassName = _class.Name

                        });
                    }
                    else
                    {
                        tempLessons.Add(new
                        {
                            LessonId = lesson.LessonId,
                            Name = lesson.Name,
                            IsExtra = lesson.IsExtra,
                            ClassName = "Ekstra Ders"

                        });
                    }
                }


                list = new TeacherDetails
                {
                    TeacherId = teacher.TeacherId,
                    Name = teacher.Name,
                    Surname = teacher.Surname,
                    Email = teacher.Email,
                    Password = teacher.Password,
                    TeacherLessonId = teacher.LessonId,
                    Classes = classes,
                    Students = students,
                    Lessons = tempLessons,
                };


                return PartialView(list);
            }
        }

        [HttpGet]
        public IActionResult TeacherStudentsDetay(int studentId, int lessonId)
        {
            try
            {
                using (Db db = new Db())
                {
                    var student = db.Student.Find(studentId);
                    var gLesson = db.Lesson.Find(lessonId);
                    dynamic teacherGrade = db.Grade.Where(x => x.LessonId == lessonId && x.StudentId == studentId).FirstOrDefault();
                    if (teacherGrade == null)
                    {
                        teacherGrade = new
                        {
                            LessonGrade = 0
                        };
                    }

                    var studentClass = db.Class.Find(student.ClassId);
                    var classLessons = db.ClassLesson.Where(x => x.ClassId == student.ClassId).ToList();
                    List<dynamic> studentLessons = new List<dynamic>();
                    dynamic tempStudentExtraLesson = null;
                    foreach (var item in classLessons)
                    {
                        var lesson = db.Lesson.Find(item.LessonId);
                        dynamic lessonGrade = db.Grade.Where(x => x.StudentId == studentId && x.LessonId == item.LessonId).FirstOrDefault();
                        if (lessonGrade == null)
                        {
                            lessonGrade = new
                            {
                                LessonGrade = 0
                            };
                        }

                        var classTeachers = db.TeacherClass.Where(x => x.ClassId == item.ClassId).ToList();
                        foreach (var classTeacher in classTeachers)
                        {
                            var teacher = db.Teacher.Find(classTeacher.TeacherId);

                            if (teacher.LessonId == item.LessonId)
                            {
                                var tempLesson = new
                                {
                                    LessonId = lesson.LessonId,
                                    Name = lesson.Name,
                                    IsExtra = lesson.IsExtra,
                                    LessonGrade = lessonGrade.LessonGrade,
                                    TeacherId = teacher.TeacherId,
                                    TeacherName = teacher.Name + " " + teacher.Surname
                                };

                                if (!studentLessons.Contains(tempLesson))
                                {
                                    studentLessons.Add(tempLesson);
                                }
                                break;
                            }
                        }

                    }

                    var studentExtraLesson = db.Lesson.Find(student.ExtraLessonId);
                    if (studentExtraLesson != null)
                    {
                        dynamic extraLessonGrade = db.Grade.Where(x => x.LessonId == studentExtraLesson.LessonId && x.StudentId == studentId).FirstOrDefault();

                        if (extraLessonGrade == null)
                        {
                            extraLessonGrade = new
                            {
                                GradeId = 0,
                                LessonId = 0,
                                StudentId = 0,
                                LessonGrade = 0
                            };
                        }

                        dynamic extraTeacher = db.Teacher.Where(x => x.LessonId == studentExtraLesson.LessonId).FirstOrDefault();

                        var teachName = "-";
                        if (extraTeacher == null)
                        {
                            extraTeacher = new
                            {
                                TeacherId = 0,
                            };
                        }
                        else
                        {
                            teachName = extraTeacher.Name + " " + extraTeacher.Surname;
                        }
                        tempStudentExtraLesson = new
                        {
                            LessonId = studentExtraLesson.LessonId,
                            Name = studentExtraLesson.Name,
                            IsExtra = studentExtraLesson.IsExtra,
                            LessonGrade = extraLessonGrade.LessonGrade,
                            TeacherId = extraTeacher.TeacherId,
                            TeacherName = teachName
                        };
                    }

                    var grade = 0;
                    if (teacherGrade != null)
                    {
                        grade = teacherGrade.LessonGrade;
                    }

                    var tempExtras = db.Lesson.Where(x => x.IsExtra == 1).ToList();
                    List<dynamic> AllExtraLessons = new List<dynamic>();
                    foreach (var lesson in tempExtras)
                    {
                        dynamic tempExtraLesson = new
                        {
                            LessonId = lesson.LessonId,
                            Name = lesson.Name,
                            IsExtra = lesson.IsExtra == 1 ? "Evet" : "Hayır"
                        };

                        if (!AllExtraLessons.Contains(tempExtraLesson))
                        {
                            AllExtraLessons.Add(tempExtraLesson);
                        }
                    }

                    dynamic tempList = new
                    {
                        LessonId = lessonId,
                        GradeId = teacherGrade.GradeId,
                        LessonName = gLesson.Name,
                        StudentId = studentId,
                        Name = student.Name + " " + student.Surname,
                        ClassId = student.ClassId,
                        ClassName = studentClass.Name,
                        Grade = grade,
                        StudentLessons = studentLessons,
                        StudentExtraLesson = tempStudentExtraLesson,
                        AllExtraLessons = AllExtraLessons
                    };

                    return PartialView(tempList);
                }
            }
            catch (Exception ex)
            {

                return PartialView(ex);
            }
        }

        [HttpGet]
        public IActionResult GetTeachers()
        {
            using (Db db = new Db())
            {
                var teachers = db.Teacher.ToList();
                var teachList = new List<dynamic>();
                foreach (var teacher in teachers)
                {
                    var lesson = db.Lesson.Find(teacher.LessonId);
                    var lessonName = lesson.Name;
                    teachList.Add(new
                    {
                        TeacherId = teacher.TeacherId,
                        Name = teacher.Name,
                        Surname = teacher.Surname,
                        Mail = teacher.Email,
                        Password = teacher.Password,
                        Image = teacher.Image,
                        LessonName = lessonName,
                        IsExtra = lesson.IsExtra
                    });
                }

                var a = Json(teachList);
                return a;
            }
        }

        public dynamic GetLessonStudents(dynamic User)
        {
            using (Db db = new Db())
            {
                int lessonId = User.LessonId;
                if (lessonId == 0)
                {
                    return null;
                }

                var classLessons = db.ClassLesson.Where(x => x.LessonId == lessonId).ToList();
                List<dynamic> students = new List<dynamic>();
                foreach (var item in classLessons)
                {

                    var student = db.Student.Where(x => x.ClassId == item.ClassId).FirstOrDefault();
                    var _class = db.Class.Find(student.ClassId);
                    var studentGrade = db.Grade.Where(x => x.LessonId == lessonId && x.StudentId == student.StudentId).FirstOrDefault();
                    var grade = 0;
                    var gradeId = 0;
                    if (studentGrade != null)
                    {
                        grade = studentGrade.LessonGrade;
                        gradeId = studentGrade.GradeId;
                    }

                    dynamic tempStudents = new
                    {
                        StudentId = student.StudentId,
                        LessonId = User.LessonId,
                        Name = student.Name,
                        Surname = student.Surname,
                        Mail = student.Email,
                        Password = student.Password,
                        Image = student.Image,
                        GradeId = gradeId,
                        LessonGrade = grade,
                        ClassId = student.ClassId,
                        ClassName = _class.Name
                    };

                    if (!students.Contains(tempStudents))
                    {
                        students.Add(tempStudents);
                    }

                }

                dynamic temp = new
                {
                    User = User,
                    Students = students
                };

                return temp;
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTeacher(Teacher objTeacher)
        {
            try
            {
                if (objTeacher.TeacherId != 0)
                {
                    using (Db db = new Db())
                    {
                        var updateTeacher = db.Teacher.Find(objTeacher.TeacherId);
                        if (updateTeacher != null)
                        {
                            updateTeacher.Name = objTeacher.Name;
                            updateTeacher.Surname = objTeacher.Surname;
                            updateTeacher.Email = objTeacher.Email;
                            updateTeacher.Password = objTeacher.Password;
                            updateTeacher.LessonId = objTeacher.LessonId;

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
        public async Task<IActionResult> UpdateGrade(Grade gradeData)
        {
            try
            {
                int lessonId = gradeData.LessonId;
                int studentId = gradeData.StudentId;
                int GradeValue = gradeData.LessonGrade;
                int gradeId = gradeData.GradeId;
                if (gradeId != 0)
                {
                    using (Db db = new Db())
                    {
                        var updateGrade = db.Grade.Find(gradeId);
                        if (updateGrade != null)
                        {
                            updateGrade.LessonGrade = GradeValue;
                            db.SaveChanges();
                        }
                    }
                }
                else
                {
                    using (Db db = new Db())
                    {
                        Grade updateGrade = new Grade();

                        updateGrade.StudentId = studentId;
                        updateGrade.LessonId = lessonId;
                        updateGrade.LessonGrade = GradeValue;
                        if (updateGrade != null)
                        {
                            db.Grade.Add(updateGrade);
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
        public async Task<IActionResult> AddTeacher(string Name, string Surname, string Email, string Password, int LessonId, int ClassId)
        {
            try
            {
                Teacher addTeacher = new Teacher
                {
                    TeacherId = 0,
                    Name = Name,
                    Surname = Surname,
                    Email = Email,
                    Password = Password,
                    LessonId = LessonId
                };

                using (Db db = new Db())
                {
                    if (addTeacher != null && addTeacher.TeacherId == 0)
                    {
                        bool teacherExists = false;
                        List<Teacher> teachers = db.Teacher.ToList();
                        foreach (var teacher in teachers)
                        {
                            if ((teacher.Name == addTeacher.Name && teacher.Surname == addTeacher.Surname) || (teacher.Email == addTeacher.Email))
                            {
                                teacherExists = true;
                                break;
                            }
                        }

                        if (!teacherExists)
                        {
                            db.Teacher.Add(addTeacher);
                            db.SaveChanges();

                            TeacherClass teacherClass = new TeacherClass();
                            teacherClass.ClassId = ClassId;
                            teacherClass.TeacherId = addTeacher.TeacherId;
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
                        return BadRequest("No Data Received! (Cannot add Teacher)");
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Bir hata oluştu");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTeacher(int id)
        {
            try
            {
                using (Db db = new Db())
                {
                    if (id > 0)
                    {
                        var removeTeacher = db.Teacher.Find(id);
                        var teacherClasses = db.TeacherClass.ToList();
                        foreach (var item in teacherClasses)
                        {
                            if (item.TeacherId == id)
                            {
                                db.TeacherClass.Remove(item);
                            }
                        }

                        db.Teacher.Remove(removeTeacher);
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
                                Lesson = new Lesson {
                                    LessonId = teacher.LessonId,
                                    Name = teacherLesson.Name,
                                },
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

    public class TeacherDetails
    {
        public int TeacherId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int TeacherLessonId { get; set; }
        public List<dynamic> Classes { get; set; }
        public List<dynamic> Students { get; set; }
        public List<dynamic> Lessons { get; set; }
    }
}
