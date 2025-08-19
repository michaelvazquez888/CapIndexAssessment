using CapIndexAssessment.Entities;

namespace CapIndexAssessment.DTOs
{
    /// <summary>
    /// Represents a question within a survey, including its answer options and conditional logic.
    /// </summary>
    /// <param name="Id">The unique identifier of the question.</param>
    /// <param name="Text">The text of the question.</param>
    /// <param name="Type">The type of the question (e.g., SingleChoice, MultipleChoice, FreeText).</param>
    /// <param name="ParentQuestionId">The ID of the parent question, if this is a conditional question.</param>
    /// <param name="TriggeringAnswerOptionId">The ID of the answer option that triggers this question to be shown.</param>
    /// <param name="AnswerOptions">The list of possible answer options for this question.</param>
    public record QuestionDto(
        Guid Id,
        string Text,
        QuestionType Type,
        Guid? ParentQuestionId,
        Guid? TriggeringAnswerOptionId,
        ICollection<AnswerOptionDto> AnswerOptions
    );

    /// <summary>
    /// Represents the data required to create a new question for a survey.
    /// </summary>
    /// <param name="LocalId">A temporary local identifier for mapping relationships during creation.</param>
    /// <param name="Text">The text of the question.</param>
    /// <param name="Type">The type of the question (e.g., SingleChoice, MultipleChoice, FreeText).</param>
    /// <param name="ParentLocalId">The local ID of the parent question, if this is a conditional question.</param>
    /// <param name="TriggeringAnswerLocalId">The local ID of the answer option that triggers this question to be shown.</param>
    /// <param name="AnswerOptions">The list of answer options for this question.</param>
    public record CreateQuestionDto(
        string LocalId,
        string Text,
        QuestionType Type,
        string? ParentLocalId,
        string? TriggeringAnswerLocalId,
        ICollection<CreateAnswerOptionDto> AnswerOptions
    );

    /// <summary>
    /// Represents the data required to update an existing question in a survey.
    /// </summary>
    /// <param name="Id">The unique identifier of the question to update.</param>
    /// <param name="Text">The new text for the question.</param>
    /// <param name="Type">The type of the question (e.g., SingleChoice, MultipleChoice, FreeText).</param>
    /// <param name="ParentQuestionId">The ID of the parent question, if this is a conditional question.</param>
    /// <param name="TriggeringAnswerOptionId">The ID of the answer option that triggers this question to be shown.</param>
    /// <param name="AnswerOptions">The updated list of answer options for this question.</param>
    public record UpdateQuestionDto(
        Guid Id,
        string Text,
        QuestionType Type,
        Guid? ParentQuestionId,
        Guid? TriggeringAnswerOptionId,
        ICollection<UpdateAnswerOptionDto> AnswerOptions
    );
}