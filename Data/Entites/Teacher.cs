using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace Data.Entities
{
    public class Teacher
    {
        [Key]
        [JsonPropertyName("Öğretmen ID")]
        public int TeacherId { get; set; }


        [JsonPropertyName("Öğretmen Adı")]
        public string Name { get; set; }


        [JsonPropertyName("Öğretmen Soyadı")]
        public string Surname { get; set; }


        [JsonPropertyName("Öğretmen Mail")]
        public string Email { get; set; }


        [JsonPropertyName("Öğretmen Şifre")]
        public string Password { get; set; }


        [JsonPropertyName("Resim")]
        public string? Image { get; set; }


        [JsonPropertyName("Ders")]
        public int LessonId { get; set; }
        public Lesson? Lesson { get; set; }

        public ICollection<ClassLesson>? ClassLesson { get; set; }

        [NotMapped]
        public string Type { get; set; } = "Öğretmen";
    }
}