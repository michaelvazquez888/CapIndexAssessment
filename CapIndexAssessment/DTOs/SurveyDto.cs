namespace CapIndexAssessment.DTOs
{
    /// <summary>
    /// Represents a survey with its questions and answer options.
    /// </summary>
    /// <param name="Id">The unique identifier of the survey.</param>
    /// <param name="Title">The title of the survey.</param>
    /// <param name="Questions">The list of questions included in the survey.</param>
    public record SurveyDto(Guid Id, string Title, ICollection<QuestionDto> Questions);

    /// <summary>
    /// Represents the data required to create a new survey.
    /// </summary>
    /// <param name="Title">The title of the new survey.</param>
    /// <param name="Questions">The list of questions to include in the survey.</param>
    public record CreateSurveyDto(string Title, ICollection<CreateQuestionDto> Questions);

    /// <summary>
    /// Represents the data required to update an existing survey.
    /// </summary>
    /// <param name="Id">The unique identifier of the survey to update.</param>
    /// <param name="Title">The new title for the survey.</param>
    /// <param name="Questions">The updated list of questions for the survey.</param>
    public record UpdateSurveyDto(Guid Id, string Title, ICollection<UpdateQuestionDto> Questions);
}