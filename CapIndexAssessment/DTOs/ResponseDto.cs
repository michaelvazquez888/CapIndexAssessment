namespace CapIndexAssessment.DTOs
{
    public record SubmitAnswerDto(Guid QuestionId, ICollection<Guid> SelectedOptionIds, string? FreeTextAnswer);
    public record SubmitResponseDto(Guid SurveyId, ICollection<SubmitAnswerDto> Answers);
}