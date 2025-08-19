using CapIndexAssessment.DTOs;
using CapIndexAssessment.Entities;
using FluentValidation;

namespace CapIndexAssessment.Validation
{
    public class UpdateQuestionValidator : AbstractValidator<UpdateQuestionDto>
    {
        public UpdateQuestionValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Text).NotEmpty().MaximumLength(1000);
            RuleFor(x => x.AnswerOptions)
                .NotEmpty()
                .When(q => q.Type != QuestionType.FreeText)
                .WithMessage("Non-free text questions must have answer options.");
            RuleForEach(x => x.AnswerOptions).SetValidator(new UpdateAnswerOptionValidator());
        }
    }
}