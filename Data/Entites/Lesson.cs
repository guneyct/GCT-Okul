using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Data.Entities
{
    public class Lesson
    {
        [Key]
        [JsonPropertyName("Ders ID")]
        public int LessonId { get; set; }

        [JsonPropertyName("Ders Adı")]
        public string Name { get; set; }

        public int IsExtra { get; set; }

        public ICollection<ClassLesson> ClassLesson { get; set; }
        public ICollection<Grade> Grade { get; set; }
    }
}