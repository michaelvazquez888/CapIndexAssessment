namespace CapIndexAssessment.DTOs
{
    public record DetailedAnswerDto(Guid QuestionId, string QuestionText, string? FreeTextAnswer, ICollection<string> SelectedOptionTexts);
    public record SurveyResponseDetailDto(Guid ResponseId, Guid SurveyId, DateTime SubmittedAt, ICollection<DetailedAnswerDto> Answers, int TotalScore);
}