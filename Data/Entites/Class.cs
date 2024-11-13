using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Data.Entities
{
    public class Class
    {
        [Key]
        [JsonPropertyName("Sınıf ID")]
        public int ClassId { get; set; }

        [JsonPropertyName("Sınıf Adı")]
        public string Name { get; set; }

        public ICollection<ClassLesson> ClassLesson { get; set; }
    }
}