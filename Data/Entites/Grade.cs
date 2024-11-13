using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Data.Entities
{
    public class Grade
    {
        [Key]
        [JsonPropertyName("Not ID")]
        public int GradeId { get; set; }

        [JsonPropertyName("Ders ID")]
        public int LessonId { get; set; }
        public Lesson Lesson { get; set; }

        [JsonPropertyName("Öğrenci ID")]
        public int StudentId { get; set; }
        public Student Student { get; set; }

        [JsonPropertyName("Öğrenci Notu")]
        public int LessonGrade { get; set; }
    }
}
