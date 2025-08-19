namespace CapIndexAssessment.Entities
{
    public class SurveyResponse
    {
        public Guid Id { get; set; }
        public DateTime SubmittedAt { get; set; }
        public Guid SurveyId { get; set; }
        public Survey? Survey { get; set; }
        public ICollection<SubmittedAnswer> Answers { get; set; } = new List<SubmittedAnswer>();
    }
}