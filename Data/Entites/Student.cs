using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Data.Entities
{
    public class Student
    {
        [Key]
        [JsonPropertyName("Öğrenci ID")]
        public int StudentId { get; set; }

        [JsonPropertyName("Öğrenci Adı")]
        public string Name { get; set; }

        [JsonPropertyName("Öğrenci Soyadı")]
        public string Surname { get; set; }

        [JsonPropertyName("Öğrenci Mail")]
        public string Email { get; set; }

        [JsonPropertyName("Öğrenci Şifre")]
        public string Password { get; set; }

        [JsonPropertyName("Ekstra Ders")]
        public int ExtraLessonId { get; set; }

        [JsonPropertyName("Resim")]
        public string? Image { get; set; }

        [JsonPropertyName("Sınıf")]
        public int ClassId { get; set; }
        public Class? Class { get; set; }

        public ICollection<Grade>? Grade { get; set; }

        [NotMapped]
        public string Type { get; set; } = "Öğrenci";
    }
}