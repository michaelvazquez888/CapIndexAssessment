using CapIndexAssessment.DTOs;
using FluentValidation;

namespace CapIndexAssessment.Validation
{
    public class CreateSurveyValidator : AbstractValidator<CreateSurveyDto>
    {
        public CreateSurveyValidator()
        {
            RuleFor(s => s.Title)
                .NotEmpty().WithMessage("Survey title cannot be empty.")
                .MaximumLength(200);

            RuleFor(s => s.Questions)
                .NotEmpty().WithMessage("A survey must have at least one question.");

            RuleForEach(s => s.Questions).SetValidator(new CreateQuestionValidator());
        }
    }
}