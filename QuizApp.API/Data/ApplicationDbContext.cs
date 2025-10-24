using Microsoft.EntityFrameworkCore;
using QuizApp.API.Models;

namespace QuizApp.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Option> Options { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<QuizQuestion> QuizQuestions { get; set; }
        public DbSet<QuizAssignment> QuizAssignments { get; set; }
        public DbSet<QuizResult> QuizResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Composite key for QuizQuestion
            modelBuilder.Entity<QuizQuestion>()
                .HasKey(qq => new { qq.QuizId, qq.QuestionId });

            modelBuilder.Entity<QuizQuestion>()
                .HasOne(qq => qq.Quiz)
                .WithMany(q => q.QuizQuestions)
                .HasForeignKey(qq => qq.QuizId);

            modelBuilder.Entity<QuizQuestion>()
                .HasOne(qq => qq.Question)
                .WithMany(q => q.QuizQuestions)
                .HasForeignKey(qq => qq.QuestionId);

            // QuizAssignment relationships
            modelBuilder.Entity<QuizAssignment>()
                .HasOne(qa => qa.Quiz)
                .WithMany(q => q.Assignments)
                .HasForeignKey(qa => qa.QuizId);

            modelBuilder.Entity<QuizAssignment>()
                .HasOne(qa => qa.User)
                .WithMany(u => u.QuizAssignments)
                .HasForeignKey(qa => qa.UserId);

            // QuizResult relationships
            modelBuilder.Entity<QuizResult>()
                .HasOne(qr => qr.Quiz)
                .WithMany(q => q.QuizResults)
                .HasForeignKey(qr => qr.QuizId);

            modelBuilder.Entity<QuizResult>()
                .HasOne(qr => qr.User)
                .WithMany(u => u.QuizResults)
                .HasForeignKey(qr => qr.UserId);

            // -----------------------------
            // Seed Data
            // -----------------------------

            // Users
            // Users - static hashed passwords and fixed CreatedAt
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    PasswordHash = "$2a$11$R5E0uX1h3Q6q9x8T2zE3ROzQs3X6XlV7S1f7QcE5qH9B8u6xYlY1G", // hash for "Admin@123"
                    Role = "Admin",
                    CreatedAt = new DateTime(2025, 10, 22)
                },
                new User
                {
                    Id = 2,
                    Username = "Neeraj",
                    PasswordHash = "$2a$11$F3Yp2X7n4Q0r1k8H2bD6S.Ox1kPqXlM9Y4s9PbF3vZ1Q5r6LkT9V2", // hash for "Nrj@123"
                    Role = "User",
                    CreatedAt = new DateTime(2025, 10, 22)
                }
            );



            // Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Math" },
                new Category { Id = 2, Name = "Science" }
            );

            // Questions
            modelBuilder.Entity<Question>().HasData(
                new Question { Id = 1, Text = "2 + 2 = ?", CategoryId = 1 },
                new Question { Id = 2, Text = "H2O is known as?", CategoryId = 2 }
            );

            // Options
            modelBuilder.Entity<Option>().HasData(
                new Option { Id = 1, QuestionId = 1, Text = "3", IsCorrect = false },
                new Option { Id = 2, QuestionId = 1, Text = "4", IsCorrect = true },
                new Option { Id = 3, QuestionId = 2, Text = "Water", IsCorrect = true },
                new Option { Id = 4, QuestionId = 2, Text = "Oxygen", IsCorrect = false }
            );

            // Quizzes
            modelBuilder.Entity<Quiz>().HasData(
    new Quiz { Id = 1, Title = "Sample Quiz", StartTime = new DateTime(2025, 10, 22, 10, 0, 0), EndTime = new DateTime(2025, 10, 22, 11, 0, 0) }
);

            // QuizQuestions
            modelBuilder.Entity<QuizQuestion>().HasData(
                new QuizQuestion { QuizId = 1, QuestionId = 1 },
                new QuizQuestion { QuizId = 1, QuestionId = 2 }
            );
        }

    }
}
