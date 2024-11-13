using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
    public class ClassLesson
    {
        [Key]
        public int ClassLessonId { get; set; }


        public int ClassId { get; set; }
        public Class Class { get; set; }


        public int LessonId { get; set; }
        public Lesson Lesson { get; set; }
    }
}