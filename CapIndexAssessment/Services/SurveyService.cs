using CapIndexAssessment.Data;
using CapIndexAssessment.DTOs;
using CapIndexAssessment.Entities;
using Microsoft.EntityFrameworkCore;

namespace CapIndexAssessment.Services
{
    public class SurveyService : ISurveyService
    {
        private readonly SurveyDbContext _context;

        public SurveyService(SurveyDbContext context)
        {
            _context = context;
        }

        public async Task<SurveyDto?> GetSurveyById(Guid surveyId)
        {
            var survey = await _context.Surveys
                .Include(s => s.Questions)
                .ThenInclude(q => q.AnswerOptions)
                .FirstOrDefaultAsync(s => s.Id == surveyId);

            if (survey is null)
            {
                return null;
            }

            var surveyDto = new SurveyDto(
                survey.Id,
                survey.Title,
                survey.Questions.Select(q => new QuestionDto(
                    q.Id,
                    q.Text,
                    q.Type,
                    q.ParentQuestionId,
                    q.TriggeringAnswerOptionId,
                    q.AnswerOptions.Select(o => new AnswerOptionDto(o.Id, o.Text, o.Weight)).ToList()
                )).ToList()
            );

            return surveyDto;
        }

        public async Task<(Survey? survey, string? error)> CreateSurvey(CreateSurveyDto createDto)
        {
            try
            {
                var questionIdMap = new Dictionary<string, Guid>();
                var answerIdMap = new Dictionary<string, Guid>();

                var newSurvey = new Survey
                {
                    Id = Guid.NewGuid(),
                    Title = createDto.Title
                };

                // Create entities and map local IDs to new Guids
                foreach (var qDto in createDto.Questions)
                {
                    var newQuestion = new Question
                    {
                        Id = Guid.NewGuid(),
                        Text = qDto.Text,
                        Type = qDto.Type,
                        Survey = newSurvey
                    };

                    questionIdMap[qDto.LocalId] = newQuestion.Id;
                    newSurvey.Questions.Add(newQuestion);

                    foreach (var oDto in qDto.AnswerOptions)
                    {
                        var newOption = new AnswerOption
                        {
                            Id = Guid.NewGuid(),
                            Text = oDto.Text,
                            Weight = oDto.Weight,
                            Question = newQuestion
                        };
                        answerIdMap[oDto.LocalId] = newOption.Id;
                        newQuestion.AnswerOptions.Add(newOption);
                    }
                }

                // Link the entities using the ID maps
                foreach (var qDto in createDto.Questions)
                {
                    var questionToUpdate = newSurvey.Questions.First(q => q.Id == questionIdMap[qDto.LocalId]);

                    if (!string.IsNullOrEmpty(qDto.ParentLocalId) && questionIdMap.TryGetValue(qDto.ParentLocalId, out var parentGuid))
                    {
                        questionToUpdate.ParentQuestionId = parentGuid;
                    }

                    if (!string.IsNullOrEmpty(qDto.TriggeringAnswerLocalId) && answerIdMap.TryGetValue(qDto.TriggeringAnswerLocalId, out var answerGuid))
                    {
                        questionToUpdate.TriggeringAnswerOptionId = answerGuid;
                    }
                }

                _context.Surveys.Add(newSurvey);
                await _context.SaveChangesAsync();

                return (newSurvey, null);
            }

            catch (Exception ex)
            {
                return (null, $"Failed to create survey: {ex.Message}");
            }
        }

        public async Task<(Survey? survey, string? error)> UpdateSurvey(UpdateSurveyDto updateDto)
        {
            try
            {
                var survey = await _context.Surveys
                    .Include(s => s.Questions)
                    .ThenInclude(q => q.AnswerOptions)
                    .FirstOrDefaultAsync(s => s.Id == updateDto.Id);

                if (survey == null)
                {
                    return (null, "Survey not found.");
                }

                if (await _context.SurveyResponses.AnyAsync(sr => sr.SurveyId == survey.Id))
                {
                    return (null, "Cannot update a survey that has responses.");
                }

                survey.Title = updateDto.Title;

                var questionsToRemove = survey.Questions
                    .Where(q => !updateDto.Questions.Any(dto => dto.Id == q.Id))
                    .ToList();

                _context.Questions.RemoveRange(questionsToRemove);

                foreach (var questionDto in updateDto.Questions)
                {
                    var question = survey.Questions.FirstOrDefault(q => q.Id == questionDto.Id);

                    if (question == null)
                    {
                        question = new Question
                        {
                            Id = questionDto.Id,
                            Text = questionDto.Text,
                            SurveyId = survey.Id,
                            Type = questionDto.Type,
                            ParentQuestionId = questionDto.ParentQuestionId,
                            TriggeringAnswerOptionId = questionDto.TriggeringAnswerOptionId
                        };

                        _context.Questions.Add(question);
                    }
                    else
                    {
                        question.Text = questionDto.Text;
                        question.Type = questionDto.Type;
                        question.ParentQuestionId = questionDto.ParentQuestionId;
                        question.TriggeringAnswerOptionId = questionDto.TriggeringAnswerOptionId;
                    }

                    var optionsToRemove = question.AnswerOptions
                        .Where(o => !questionDto.AnswerOptions.Any(dto => dto.Id == o.Id))
                        .ToList();

                    _context.AnswerOptions.RemoveRange(optionsToRemove);

                    foreach (var optionDto in questionDto.AnswerOptions)
                    {
                        var option = question.AnswerOptions.FirstOrDefault(o => o.Id == optionDto.Id);

                        if (option == null)
                        {
                            option = new AnswerOption
                            {
                                Id = optionDto.Id,
                                Text = optionDto.Text,
                                Weight = optionDto.Weight,
                                QuestionId = question.Id
                            };
                            _context.AnswerOptions.Add(option);
                        }

                        else 
                        {
                            option.Text = optionDto.Text;
                            option.Weight = optionDto.Weight;
                        }
                    }
                }

                await _context.SaveChangesAsync();
                return (survey, null);
            }

            catch (Exception ex)
            {
                return (null, $"Failed to update survey: {ex.Message}");
            }
        }

        public async Task<(bool success, string? error)> DeleteSurveyAsync(Guid surveyId)
        {
            try
            {
                var survey = await _context.Surveys.FindAsync(surveyId);

                if (survey is null)
                {
                    return (false, "Survey not found.");
                }

                if (await _context.SurveyResponses.AnyAsync(sr => sr.SurveyId == surveyId))
                {
                    return (false, "Cannot delete a survey that has responses.");
                }

                _context.Surveys.Remove(survey);

                await _context.SaveChangesAsync();

                return (true, null);
            }

            catch (Exception ex)
            {
                return (false, $"Failed to delete survey: {ex.Message}");
            }
        }

        public async Task<(SurveyResponse? response, string? error)> SubmitResponse(Guid surveyId, ICollection<SubmitAnswerDto> submittedAnswers)
        {
            try
            {
                var survey = await _context.Surveys
                    .Include(s => s.Questions)
                    .ThenInclude(q => q.AnswerOptions)
                    .FirstOrDefaultAsync(s => s.Id == surveyId);

                if (survey == null)
                {
                    return (null, "Survey not found.");
                }

                var duplicateQuestionIds = submittedAnswers
                    .GroupBy(a => a.QuestionId)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key)
                    .ToList();

                if (duplicateQuestionIds.Any())
                {
                    return (null, $"Duplicate answers found for question(s): {string.Join(", ", duplicateQuestionIds)}.");
                }

                var answersForValidation = submittedAnswers.ToDictionary(a => a.QuestionId, a => a.SelectedOptionIds.ToList());
                var visibleQuestions = GetVisibleQuestions(survey, answersForValidation);
                var visibleQuestionIds = new HashSet<Guid>(visibleQuestions.Select(q => q.Id));

                foreach (var submittedAnswer in submittedAnswers)
                {
                    var question = survey.Questions.FirstOrDefault(q => q.Id == submittedAnswer.QuestionId);

                    if (question == null)
                    {
                        return (null, $"An answer was submitted for question '{submittedAnswer.QuestionId}', which does not exist or is not visible.");
                    }

                    var validOptionIds = question.AnswerOptions.Select(o => o.Id).ToHashSet();
                    var invalidOptionIds = submittedAnswer.SelectedOptionIds.Where(id => !validOptionIds.Contains(id)).ToList();

                    if (invalidOptionIds.Any())
                    {
                        return (null, $"Invalid answer option(s) {string.Join(", ", invalidOptionIds)} for question '{submittedAnswer.QuestionId}'.");
                    }

                    if (!visibleQuestionIds.Contains(submittedAnswer.QuestionId))
                    {
                        return (null, $"An answer was submitted for question '{submittedAnswer.QuestionId}', which does not exist or is not visible.");
                    }
                }

                var surveyResponse = new SurveyResponse
                {
                    Id = Guid.NewGuid(),
                    SurveyId = surveyId,
                    SubmittedAt = DateTime.UtcNow
                };

                foreach (var answer in submittedAnswers)
                {
                    var question = survey.Questions.First(q => q.Id == answer.QuestionId);
                    var selectedOptions = question.AnswerOptions.Where(ao => answer.SelectedOptionIds.Contains(ao.Id)).ToList();

                    surveyResponse.Answers.Add(new SubmittedAnswer
                    {
                        Id = Guid.NewGuid(),
                        QuestionId = answer.QuestionId,
                        SelectedOptions = selectedOptions,
                        FreeTextAnswer = answer.FreeTextAnswer
                    });
                }

                _context.SurveyResponses.Add(surveyResponse);
                await _context.SaveChangesAsync();

                return (surveyResponse, null);
            }

            catch (Exception ex)
            {
                return (null, $"Failed to create survey response: {ex.Message}");
            }
        }

        public async Task<SurveyResponseDetailDto?> GetResponseDetails(Guid responseId)
        {
            var response = await _context.SurveyResponses
                .Include(r => r.Answers)
                .ThenInclude(a => a.Question)
                .Include(r => r.Answers)
                .ThenInclude(a => a.SelectedOptions)
                .FirstOrDefaultAsync(r => r.Id == responseId);

            if (response is null)
            {
                return null;
            }

            var responseDetailDto = new SurveyResponseDetailDto(
                response.Id,
                response.SurveyId,
                response.SubmittedAt,
                response.Answers.Select(a => new DetailedAnswerDto(
                    a.QuestionId,
                    a.Question!.Text,
                    a.FreeTextAnswer,
                    a.SelectedOptions.Select(so => so.Text).ToList()
                )).ToList(),
                response.Answers.SelectMany(a => a.SelectedOptions).Sum(o => o.Weight)
            );

            return responseDetailDto;
        }

        private static List<Question> GetVisibleQuestions(Survey survey, Dictionary<Guid, List<Guid>> answers)
        {
            var rootQuestions = survey.Questions.Where(q => q.ParentQuestionId == null);
            var visible = new List<Question>(rootQuestions);
            var queue = new Queue<Question>(rootQuestions);

            while (queue.Count > 0)
            {
                var parent = queue.Dequeue();
                var children = survey.Questions.Where(q => q.ParentQuestionId == parent.Id);

                // Check if the user's answer to the parent question triggers any children
                if (answers.TryGetValue(parent.Id, out var selectedOptionIds))
                {
                    foreach (var child in children)
                    {
                        // If the child's trigger is in the list of selected options, it's visible
                        if (child.TriggeringAnswerOptionId.HasValue && selectedOptionIds.Contains(child.TriggeringAnswerOptionId.Value))
                        {
                            visible.Add(child);
                            queue.Enqueue(child);
                        }
                    }
                }
            }

            return visible;
        }
    }
}