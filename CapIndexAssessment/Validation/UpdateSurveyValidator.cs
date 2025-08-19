using CapIndexAssessment.DTOs;
using FluentValidation;

namespace CapIndexAssessment.Validation
{
    public class UpdateSurveyValidator : AbstractValidator<UpdateSurveyDto>
    {
        public UpdateSurveyValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Questions).NotEmpty();
            RuleForEach(x => x.Questions).SetValidator(new UpdateQuestionValidator());
        }
    }
}