using CapIndexAssessment.Data;
using CapIndexAssessment.DTOs;
using CapIndexAssessment.Entities;
using FluentValidation;

namespace CapIndexAssessment.Validation
{
    public class SubmitAnswerValidator : AbstractValidator<SubmitAnswerDto>
    {
        private readonly SurveyDbContext _dbContext;

        public SubmitAnswerValidator(SurveyDbContext dbContext)
        {
            _dbContext = dbContext;

            RuleFor(x => x.QuestionId)
                .Must(BeAValidQuestion)
                .WithMessage("The specified question does not exist.");

            RuleFor(a => a.SelectedOptionIds)
                .Must(ids => ids.Count() == ids.Distinct().Count())
                .WithMessage("Duplicate answer options are not allowed in a single answer.");

            When(IsQuestionType(QuestionType.SingleChoice), () =>
            {
                RuleFor(a => a.SelectedOptionIds)
                    .Must(ids => ids != null && ids.Count == 1).WithMessage("Single choice questions must have exactly one answer.");
                RuleFor(a => a.FreeTextAnswer)
                    .Empty().WithMessage("Single choice questions cannot have a free text answer.");
            });

            When(IsQuestionType(QuestionType.MultipleChoice), () =>
            {
                RuleFor(a => a.SelectedOptionIds)
                    .NotEmpty().WithMessage("Multiple choice questions must have at least one answer.");
                RuleFor(a => a.FreeTextAnswer)
                    .Empty().WithMessage("Multiple choice questions cannot have a free text answer.");
            });

            When(IsQuestionType(QuestionType.FreeText), () =>
            {
                RuleFor(a => a.SelectedOptionIds)
                    .Empty().WithMessage("Free text questions cannot have selected options.");
                RuleFor(a => a.FreeTextAnswer)
                    .NotEmpty().WithMessage("Free text questions must have a text answer.");
            });
        }

        private bool BeAValidQuestion(Guid questionId)
        {
            return _dbContext.Questions.Any(q => q.Id == questionId);
        }

        private Func<SubmitAnswerDto, bool> IsQuestionType(QuestionType type)
        {
            return answer =>
            {
                var question = _dbContext.Questions.Find(answer.QuestionId);
                return question != null && question.Type == type;
            };
        }
    }
}