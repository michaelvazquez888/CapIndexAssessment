using CapIndexAssessment.DTOs;
using FluentValidation;

namespace CapIndexAssessment.Validation
{
    public class UpdateAnswerOptionValidator : AbstractValidator<UpdateAnswerOptionDto>
    {
        public UpdateAnswerOptionValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Text).NotEmpty().MaximumLength(500);
            RuleFor(x => x.Weight).InclusiveBetween(-100, 100);
        }
    }
}