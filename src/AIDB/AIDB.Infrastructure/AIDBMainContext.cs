using AIDB.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace AIDB.Infrastructure
{
    public class AIDBMainContext : DbContext
    {
        public AIDBMainContext() { }
        public AIDBMainContext(DbContextOptions<AIDBMainContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Grade>()
                .HasOne<Student>(g => g.Student)
                .WithMany(s => s.Grades);

            modelBuilder.Entity<Grade>()
                .HasOne<Teacher>(g => g.Teacher);

            modelBuilder.Entity<Grade>()
                .HasOne<Subject>(g => g.Subject);

            modelBuilder.Entity<Student>()
                .HasOne<Person>(s => s.Person);

            modelBuilder.Entity<Student>()
                .HasMany<Grade>(s => s.Grades);

            modelBuilder.Entity<Teacher>()
                .HasOne<Person>(t => t.Person);

            modelBuilder.Entity<Teacher>()
                .HasOne<Title>(t => t.Title);

            modelBuilder.Entity<Person>()
                .HasMany<Subject>(p => p.Subjects)
                .WithMany(s => s.Persons);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Grade> Grades { get; set; } = default!;
        public DbSet<Person> Persons { get; set; } = default!;
        public DbSet<Student> Students { get; set; } = default!;
        public DbSet<Subject> Subjects { get; set; } = default!;
        public DbSet<Teacher> Teachers { get; set; } = default!;
        public DbSet<Title> Titles { get; set; } = default!;
        public DbSet<AiDbCommand> AiDbCommands { get; set; } = default!;
    }
}