using System.ComponentModel.DataAnnotations;


namespace Data.Entities
{
    public class TeacherClass
    {
        [Key]
        public int TeacherClassId { get; set; }

        public int TeacherId { get; set; }

        public int ClassId { get; set; }
    }
}
