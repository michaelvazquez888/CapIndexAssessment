using CapIndexAssessment.Entities;
using Microsoft.EntityFrameworkCore;

namespace CapIndexAssessment.Data
{
    public class SurveyDbContext(DbContextOptions<SurveyDbContext> options) : DbContext(options)
    {
        public DbSet<Survey> Surveys { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<AnswerOption> AnswerOptions { get; set; }
        public DbSet<SurveyResponse> SurveyResponses { get; set; }
        public DbSet<SubmittedAnswer> SubmittedAnswers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the many-to-many relationship between SubmittedAnswer and AnswerOption.
            modelBuilder.Entity<SubmittedAnswer>()
                .HasMany(sa => sa.SelectedOptions)
                .WithMany();
        }
    }
}