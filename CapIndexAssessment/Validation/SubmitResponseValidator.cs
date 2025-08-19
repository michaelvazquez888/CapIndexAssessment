using CapIndexAssessment.DTOs;
using FluentValidation;

namespace CapIndexAssessment.Validation
{
    public class SubmitResponseValidator : AbstractValidator<SubmitResponseDto>
    {
        public SubmitResponseValidator(SubmitAnswerValidator answerValidator)
        {
            RuleFor(r => r.Answers)
                .NotEmpty().WithMessage("A survey response must contain at least one answer.");

            RuleForEach(r => r.Answers)
                .SetValidator(answerValidator);
        }
    }
}