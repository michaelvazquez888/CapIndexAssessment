namespace CapIndexAssessment.DTOs
{
    /// <summary>
    /// Represents a possible answer option for a survey question.
    /// </summary>
    /// <param name="Id">The unique identifier of the answer option.</param>
    /// <param name="Text">The display text for the answer option.</param>
    /// <param name="Weight">The weight or score associated with this answer option.</param>
    public record AnswerOptionDto(Guid Id, string Text, int Weight);

    /// <summary>
    /// Represents the data required to create a new answer option for a question.
    /// </summary>
    /// <param name="LocalId">A temporary local identifier for mapping relationships during creation.</param>
    /// <param name="Text">The display text for the answer option.</param>
    /// <param name="Weight">The weight or score associated with this answer option.</param>
    public record CreateAnswerOptionDto(string LocalId, string Text, int Weight);

    /// <summary>
    /// Represents the data required to update an existing answer option.
    /// </summary>
    /// <param name="Id">The unique identifier of the answer option to update.</param>
    /// <param name="Text">The new display text for the answer option.</param>
    /// <param name="Weight">The new weight or score for the answer option.</param>
    public record UpdateAnswerOptionDto(Guid Id, string Text, int Weight);
}