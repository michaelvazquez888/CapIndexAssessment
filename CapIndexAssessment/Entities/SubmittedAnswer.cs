namespace CapIndexAssessment.Entities
{
    public class SubmittedAnswer
    {
        public Guid Id { get; set; }
        public Guid SurveyResponseId { get; set; }
        public SurveyResponse? SurveyResponse { get; set; }
        public Guid QuestionId { get; set; }
        public Question? Question { get; set; }
        public string? FreeTextAnswer { get; set; }
        public ICollection<AnswerOption> SelectedOptions { get; set; } = new List<AnswerOption>();
    }
}