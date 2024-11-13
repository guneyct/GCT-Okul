using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Data.Entities
{
    public class Admin
    {
        [Key]
        [JsonPropertyName("Müdür Id")]
        public int AdminId { get; set; }

        [JsonPropertyName("Müdür Adı")]
        public string Name { get; set; }

        [JsonPropertyName("Müdür Soyadı")]
        public string Surname { get; set; }

        [JsonPropertyName("Müdür Mail")]
        public string Email { get; set; }

        [JsonPropertyName("Müdür Şifre")]
        public string Password { get; set; }

        [JsonPropertyName("Resim")]
        public string? Image { get; set; }

        [NotMapped]
        public string Type { get; set; } = "Müdür";
    }
}