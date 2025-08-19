namespace CapIndexAssessment.DTOs
{
    public record AnswerOptionDto(Guid Id, string Text, int Weight);
    public record CreateAnswerOptionDto(string LocalId, string Text, int Weight);
    public record UpdateAnswerOptionDto(Guid Id, string Text, int Weight);
}