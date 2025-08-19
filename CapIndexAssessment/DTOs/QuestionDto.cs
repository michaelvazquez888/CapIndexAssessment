using CapIndexAssessment.Entities;

namespace CapIndexAssessment.DTOs
{
    public record QuestionDto(Guid Id, string Text, QuestionType Type, Guid? ParentQuestionId, Guid? TriggeringAnswerOptionId, ICollection<AnswerOptionDto> AnswerOptions);
    public record CreateQuestionDto(string LocalId, string Text, QuestionType Type, string? ParentLocalId, string? TriggeringAnswerLocalId, ICollection<CreateAnswerOptionDto> AnswerOptions);
    public record UpdateQuestionDto(Guid Id, string Text, QuestionType Type, Guid? ParentQuestionId, Guid? TriggeringAnswerOptionId, ICollection<UpdateAnswerOptionDto> AnswerOptions);
}