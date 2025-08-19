using System.Text.Json.Serialization;

namespace CapIndexAssessment.Entities
{
    public class Question
    {
        public Guid Id { get; set; }
        public required string Text { get; set; }
        public QuestionType Type { get; set; }
        public Guid SurveyId { get; set; }
        [JsonIgnore]
        public Survey? Survey { get; set; }
        public Guid? ParentQuestionId { get; set; }
        public Guid? TriggeringAnswerOptionId { get; set; }
        public ICollection<AnswerOption> AnswerOptions { get; set; } = new List<AnswerOption>();
    }
}