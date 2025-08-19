namespace CapIndexAssessment.DTOs
{
    public record SurveyDto(Guid Id, string Title, ICollection<QuestionDto> Questions);
    public record CreateSurveyDto(string Title, ICollection<CreateQuestionDto> Questions);
    public record UpdateSurveyDto(Guid Id, string Title, ICollection<UpdateQuestionDto> Questions);
}