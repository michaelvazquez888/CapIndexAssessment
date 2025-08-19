namespace CapIndexAssessment.DTOs
{
    /// <summary>
    /// Represents an answer to a single survey question.
    /// </summary>
    /// <param name="QuestionId">The unique identifier of the question being answered.</param>
    /// <param name="SelectedOptionIds">The list of selected answer option IDs (for choice questions).</param>
    /// <param name="FreeTextAnswer">The free text answer (for free text questions).</param>
    public record SubmitAnswerDto(Guid QuestionId, ICollection<Guid> SelectedOptionIds, string? FreeTextAnswer);

    /// <summary>
    /// Represents a user's submission of answers to a survey.
    /// </summary>
    /// <param name="SurveyId">The unique identifier of the survey being answered.</param>
    /// <param name="Answers">The list of answers submitted by the user.</param>
    public record SubmitResponseDto(Guid SurveyId, ICollection<SubmitAnswerDto> Answers);
}