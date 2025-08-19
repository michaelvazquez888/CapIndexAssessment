namespace CapIndexAssessment.DTOs
{
    /// <summary>
    /// Represents a detailed answer to a survey question, including text and selected options.
    /// </summary>
    /// <param name="QuestionId">The unique identifier of the question.</param>
    /// <param name="QuestionText">The text of the question.</param>
    /// <param name="FreeTextAnswer">The free text answer, if applicable.</param>
    /// <param name="SelectedOptionTexts">The display texts of the selected answer options.</param>
    public record DetailedAnswerDto(
        Guid QuestionId,
        string QuestionText,
        string? FreeTextAnswer,
        ICollection<string> SelectedOptionTexts
    );

    /// <summary>
    /// Represents the details of a submitted survey response, including answers and total score.
    /// </summary>
    /// <param name="Id">The unique identifier of the survey response.</param>
    /// <param name="SurveyId">The unique identifier of the survey.</param>
    /// <param name="SubmittedAt">The date and time when the response was submitted (UTC).</param>
    /// <param name="Answers">The list of answers provided in this response.</param>
    /// <param name="TotalScore">The total score calculated from the selected answer options.</param>
    public record SurveyResponseDetailDto(
        Guid Id,
        Guid SurveyId,
        DateTime SubmittedAt,
        ICollection<DetailedAnswerDto> Answers,
        int TotalScore
    );
}