using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.Context
{
    public class Db : DbContext
    {
        public Db() : base(options: new DbContextOptionsBuilder<Db>().UseSqlServer("Server=localhost;Database=gokul;User Id=sa;Password=Guney24!!24;TrustServerCertificate=True").Options)
        {
        }

        public virtual DbSet<Admin> Admin { get; set; }
        public virtual DbSet<Teacher> Teacher { get; set; }
        public virtual DbSet<Lesson> Lesson { get; set; }
        public virtual DbSet<Student> Student { get; set; }
        public virtual DbSet<Class> Class { get; set; }
        public virtual DbSet<ClassLesson> ClassLesson { get; set; }
        public virtual DbSet<Grade> Grade { get; set; }
        public virtual DbSet<TeacherClass> TeacherClass { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
