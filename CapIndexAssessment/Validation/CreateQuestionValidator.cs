using CapIndexAssessment.DTOs;
using CapIndexAssessment.Entities;
using FluentValidation;

namespace CapIndexAssessment.Validation
{
    public class CreateQuestionValidator : AbstractValidator<CreateQuestionDto>
    {
        public CreateQuestionValidator()
        {
            RuleFor(q => q.Text)
                .NotEmpty().WithMessage("Question text cannot be empty.");

            RuleFor(q => q.LocalId)
                .NotEmpty().WithMessage("Question localId cannot be empty.");

            RuleFor(q => q.AnswerOptions)
                .NotEmpty().WithMessage("Choice-based questions must have answer options.")
                .When(q => q.Type == QuestionType.SingleChoice || q.Type == QuestionType.MultipleChoice);

            RuleForEach(q => q.AnswerOptions).SetValidator(new CreateAnswerOptionValidator());
        }
    }
}