namespace CapIndexAssessment.Entities
{
    public class Survey
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public ICollection<Question> Questions { get; set; } = new List<Question>();
    }
}