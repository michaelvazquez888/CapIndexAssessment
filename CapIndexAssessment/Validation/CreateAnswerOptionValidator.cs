using CapIndexAssessment.DTOs;
using FluentValidation;

namespace CapIndexAssessment.Validation
{
    public class CreateAnswerOptionValidator : AbstractValidator<CreateAnswerOptionDto>
    {
        public CreateAnswerOptionValidator()
        {
            RuleFor(o => o.Text)
                .NotEmpty().WithMessage("Answer option text cannot be empty.");

            RuleFor(o => o.LocalId)
                .NotEmpty().WithMessage("Answer option localId cannot be empty.");

            RuleFor(o => o.Weight)
                .GreaterThanOrEqualTo(0).WithMessage("Weight cannot be negative.");
        }
    }
}