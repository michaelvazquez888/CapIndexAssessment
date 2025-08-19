using System.Text.Json.Serialization;

namespace CapIndexAssessment.Entities
{
    public class AnswerOption
    {
        public Guid Id { get; set; }
        public required string Text { get; set; }
        public int Weight { get; set; }
        public Guid QuestionId { get; set; }
        [JsonIgnore]
        public Question? Question { get; set; }
    }
}