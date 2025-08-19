using CapIndexAssessment.Data;
using CapIndexAssessment.DTOs;
using CapIndexAssessment.Entities;
using CapIndexAssessment.Services;
using Microsoft.EntityFrameworkCore;

namespace CapIndexAssessmentTests
{
    public class SurveyServiceUnitTests
    {
        private SurveyDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<SurveyDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new SurveyDbContext(options);
        }

        [Fact]
        public async Task GetSurveyById_ReturnsNull_WhenSurveyDoesNotExist()
        {
            using var db = CreateInMemoryDbContext();
            var service = new SurveyService(db);

            var result = await service.GetSurveyById(Guid.NewGuid());

            Assert.Null(result);
        }

        [Fact]
        public async Task GetSurveyById_ReturnsSurveyDto_WhenSurveyExists()
        {
            using var db = CreateInMemoryDbContext();
            var survey = new Survey
            {
                Id = Guid.NewGuid(),
                Title = "Test Survey",
                Questions = new List<Question>
                {
                    new Question
                    {
                        Id = Guid.NewGuid(),
                        Text = "Q1",
                        Type = QuestionType.SingleChoice,
                        AnswerOptions = new List<AnswerOption>
                        {
                            new AnswerOption { Id = Guid.NewGuid(), Text = "A1", Weight = 1 }
                        }
                    }
                }
            };
            db.Surveys.Add(survey);
            db.SaveChanges();

            var service = new SurveyService(db);

            var result = await service.GetSurveyById(survey.Id);

            Assert.NotNull(result);
            Assert.Equal("Test Survey", result.Title);
            Assert.Single(result.Questions);
        }

        [Fact]
        public async Task UpdateSurvey_ReturnsError_WhenSurveyNotFound()
        {
            using var db = CreateInMemoryDbContext();
            var service = new SurveyService(db);

            var updateDto = new UpdateSurveyDto(
                Guid.NewGuid(),
                "Updated Title",
                new List<UpdateQuestionDto>()
            );

            var (survey, error) = await service.UpdateSurvey(updateDto);

            Assert.Null(survey);
            Assert.Equal("Survey not found.", error);
        }

        [Fact]
        public async Task UpdateSurvey_ReturnsError_WhenSurveyHasResponses()
        {
            using var db = CreateInMemoryDbContext();
            var survey = new Survey
            {
                Id = Guid.NewGuid(),
                Title = "Original",
                Questions = new List<Question>()
            };
            db.Surveys.Add(survey);
            db.SurveyResponses.Add(new SurveyResponse
            {
                Id = Guid.NewGuid(),
                SurveyId = survey.Id,
                SubmittedAt = DateTime.UtcNow
            });
            db.SaveChanges();

            var service = new SurveyService(db);

            var updateDto = new UpdateSurveyDto(
                survey.Id,
                "Updated Title",
                new List<UpdateQuestionDto>()
            );

            var (updatedSurvey, error) = await service.UpdateSurvey(updateDto);

            Assert.Null(updatedSurvey);
            Assert.Equal("Cannot update a survey that has responses.", error);
        }

        [Fact]
        public async Task UpdateSurvey_UpdatesTitleAndQuestions()
        {
            using var db = CreateInMemoryDbContext();
            var questionId = Guid.NewGuid();
            var optionId = Guid.NewGuid();
            var survey = new Survey
            {
                Id = Guid.NewGuid(),
                Title = "Original",
                Questions = new List<Question>
                {
                    new Question
                    {
                        Id = questionId,
                        Text = "Old Question",
                        Type = QuestionType.SingleChoice,
                        AnswerOptions = new List<AnswerOption>
                        {
                            new AnswerOption { Id = optionId, Text = "Old Option", Weight = 1 }
                        }
                    }
                }
            };
            db.Surveys.Add(survey);
            db.SaveChanges();

            var service = new SurveyService(db);

            var updateDto = new UpdateSurveyDto(
                survey.Id,
                "Updated Title",
                new List<UpdateQuestionDto>
                {
                    new UpdateQuestionDto(
                        questionId,
                        "Updated Question",
                        QuestionType.SingleChoice,
                        null,
                        null,
                        new List<UpdateAnswerOptionDto>
                        {
                            new UpdateAnswerOptionDto(optionId, "Updated Option", 5)
                        }
                    )
                }
            );

            var (updatedSurvey, error) = await service.UpdateSurvey(updateDto);

            Assert.NotNull(updatedSurvey);
            Assert.Null(error);
            Assert.Equal("Updated Title", updatedSurvey.Title);
            Assert.Single(updatedSurvey.Questions);
            Assert.Equal("Updated Question", updatedSurvey.Questions.First().Text);
            Assert.Single(updatedSurvey.Questions.First().AnswerOptions);
            Assert.Equal("Updated Option", updatedSurvey.Questions.First().AnswerOptions.First().Text);
            Assert.Equal(5, updatedSurvey.Questions.First().AnswerOptions.First().Weight);
        }

        [Fact]
        public async Task UpdateSurvey_RemovesQuestionsAndOptionsNotInDto()
        {
            using var db = CreateInMemoryDbContext();
            var q1Id = Guid.NewGuid();
            var q2Id = Guid.NewGuid();
            var o1Id = Guid.NewGuid();
            var o2Id = Guid.NewGuid();
            var survey = new Survey
            {
                Id = Guid.NewGuid(),
                Title = "Original",
                Questions = new List<Question>
                {
                    new Question
                    {
                        Id = q1Id,
                        Text = "Q1",
                        Type = QuestionType.SingleChoice,
                        AnswerOptions = new List<AnswerOption>
                        {
                            new AnswerOption { Id = o1Id, Text = "O1", Weight = 1 }
                        }
                    },
                    new Question
                    {
                        Id = q2Id,
                        Text = "Q2",
                        Type = QuestionType.SingleChoice,
                        AnswerOptions = new List<AnswerOption>
                        {
                            new AnswerOption { Id = o2Id, Text = "O2", Weight = 2 }
                        }
                    }
                }
            };
            db.Surveys.Add(survey);
            db.SaveChanges();

            var service = new SurveyService(db);

            // Only keep q1 and o1
            var updateDto = new UpdateSurveyDto(
                survey.Id,
                "Updated Title",
                new List<UpdateQuestionDto>
                {
                    new UpdateQuestionDto(
                        q1Id,
                        "Q1",
                        QuestionType.SingleChoice,
                        null,
                        null,
                        new List<UpdateAnswerOptionDto>
                        {
                            new UpdateAnswerOptionDto(o1Id, "O1", 1)
                        }
                    )
                }
            );

            var (updatedSurvey, error) = await service.UpdateSurvey(updateDto);

            Assert.NotNull(updatedSurvey);
            Assert.Null(error);
            Assert.Single(updatedSurvey.Questions);
            Assert.Equal(q1Id, updatedSurvey.Questions.First().Id);
            Assert.Single(updatedSurvey.Questions.First().AnswerOptions);
            Assert.Equal(o1Id, updatedSurvey.Questions.First().AnswerOptions.First().Id);
        }

        [Fact]
        public async Task UpdateSurvey_AddsNewQuestionsAndOptions()
        {
            using var db = CreateInMemoryDbContext();
            var survey = new Survey
            {
                Id = Guid.NewGuid(),
                Title = "Original",
                Questions = new List<Question>()
            };
            db.Surveys.Add(survey);
            db.SaveChanges();

            var service = new SurveyService(db);

            var newQId = Guid.NewGuid();
            var newOId = Guid.NewGuid();

            var updateDto = new UpdateSurveyDto(
                survey.Id,
                "Updated Title",
                new List<UpdateQuestionDto>
                {
                    new UpdateQuestionDto(
                        newQId,
                        "New Question",
                        QuestionType.SingleChoice,
                        null,
                        null,
                        new List<UpdateAnswerOptionDto>
                        {
                            new UpdateAnswerOptionDto(newOId, "New Option", 10)
                        }
                    )
                }
            );

            var (updatedSurvey, error) = await service.UpdateSurvey(updateDto);

            Assert.NotNull(updatedSurvey);
            Assert.Null(error);
            Assert.Single(updatedSurvey.Questions);
            Assert.Equal(newQId, updatedSurvey.Questions.First().Id);
            Assert.Single(updatedSurvey.Questions.First().AnswerOptions);
            Assert.Equal(newOId, updatedSurvey.Questions.First().AnswerOptions.First().Id);
        }

        [Fact]
        public async Task DeleteSurvey_ShouldSucceed_WhenSurveyExistsAndHasNoResponses()
        {
            using var dbContext = CreateInMemoryDbContext();
            var surveyService = new SurveyService(dbContext);

            var surveyId = Guid.NewGuid();
            var surveyToDelete = new Survey { Id = surveyId, Title = "Test Survey" };

            dbContext.Surveys.Add(surveyToDelete);
            await dbContext.SaveChangesAsync();

            var (success, error) = await surveyService.DeleteSurveyAsync(surveyId);

            Assert.True(success);
            Assert.Null(error);

            var surveyInDb = await dbContext.Surveys.FindAsync(surveyId);
            Assert.Null(surveyInDb);
        }

        [Fact]
        public async Task DeleteSurvey_ShouldFail_WhenSurveyNotFound()
        {
            using var dbContext = CreateInMemoryDbContext();
            var surveyService = new SurveyService(dbContext);
            var nonExistentSurveyId = Guid.NewGuid();

            var (success, error) = await surveyService.DeleteSurveyAsync(nonExistentSurveyId);

            Assert.False(success);
            Assert.Equal("Survey not found.", error);
        }

        [Fact]
        public async Task DeleteSurvey_ShouldFail_WhenSurveyHasResponses()
        {
            using var dbContext = CreateInMemoryDbContext();
            var surveyService = new SurveyService(dbContext);

            var surveyId = Guid.NewGuid();
            var survey = new Survey { Id = surveyId, Title = "Test Survey With Response" };
            var response = new SurveyResponse { Id = Guid.NewGuid(), SurveyId = surveyId };

            dbContext.Surveys.Add(survey);
            dbContext.SurveyResponses.Add(response);
            await dbContext.SaveChangesAsync();

            var (success, error) = await surveyService.DeleteSurveyAsync(surveyId);

            Assert.False(success);
            Assert.Equal("Cannot delete a survey that has responses.", error);

            var surveyInDb = await dbContext.Surveys.FindAsync(surveyId);
            Assert.NotNull(surveyInDb);
        }

        [Fact]
        public async Task GetResponseDetails_ReturnsNull_WhenResponseNotFound()
        {
            using var db = CreateInMemoryDbContext();
            var service = new SurveyService(db);

            var result = await service.GetResponseDetails(Guid.NewGuid());

            Assert.Null(result);
        }

        [Fact]
        public async Task SubmitResponse_ReturnsError_WhenSurveyNotFound()
        {
            using var db = CreateInMemoryDbContext();
            var service = new SurveyService(db);

            var (response, error) = await service.SubmitResponse(Guid.NewGuid(), new List<SubmitAnswerDto>());

            Assert.Null(response);
            Assert.Equal("Survey not found.", error);
        }

        [Fact]
        public async Task SubmitResponse_ReturnsError_WhenAnswerForNonVisibleQuestion()
        {
            using var db = CreateInMemoryDbContext();
            var survey = new Survey
            {
                Id = Guid.NewGuid(),
                Title = "Test",
                Questions = new List<Question>
                {
                    new Question
                    {
                        Id = Guid.NewGuid(),
                        Text = "Q1",
                        Type = QuestionType.SingleChoice,
                        AnswerOptions = new List<AnswerOption>
                        {
                            new AnswerOption { Id = Guid.NewGuid(), Text = "A1", Weight = 1 }
                        }
                    }
                }
            };
            db.Surveys.Add(survey);
            db.SaveChanges();

            var service = new SurveyService(db);

            var (response, error) = await service.SubmitResponse(survey.Id, new List<SubmitAnswerDto>
            {
                new SubmitAnswerDto(Guid.NewGuid(), new List<Guid>(), null)
            });

            Assert.Null(response);
            Assert.Contains("does not exist or is not visible", error);
        }

        [Fact]
        public async Task SubmitResponse_Succeeds_WhenAllAnswersValid()
        {
            using var db = CreateInMemoryDbContext();
            var questionId = Guid.NewGuid();
            var optionId = Guid.NewGuid();
            var survey = new Survey
            {
                Id = Guid.NewGuid(),
                Title = "Test",
                Questions = new List<Question>
                {
                    new Question
                    {
                        Id = questionId,
                        Text = "Q1",
                        Type = QuestionType.SingleChoice,
                        AnswerOptions = new List<AnswerOption>
                        {
                            new AnswerOption { Id = optionId, Text = "A1", Weight = 1 }
                        }
                    }
                }
            };
            db.Surveys.Add(survey);
            db.SaveChanges();

            var service = new SurveyService(db);

            var answers = new List<SubmitAnswerDto>
            {
                new SubmitAnswerDto(questionId, new List<Guid> { optionId }, null)
            };

            var (response, error) = await service.SubmitResponse(survey.Id, answers);

            Assert.NotNull(response);
            Assert.Null(error);
            Assert.Single(response.Answers);
        }

        [Fact]
        public async Task SubmitResponse_ReturnsError_WhenDuplicateAnswersForSameQuestion()
        {
            using var db = CreateInMemoryDbContext();
            var questionId = Guid.NewGuid();
            var optionId = Guid.NewGuid();
            var survey = new Survey
            {
                Id = Guid.NewGuid(),
                Title = "Test",
                Questions = new List<Question>
                {
                    new Question
                    {
                        Id = questionId,
                        Text = "Q1",
                        Type = QuestionType.SingleChoice,
                        AnswerOptions = new List<AnswerOption>
                        {
                            new AnswerOption { Id = optionId, Text = "A1", Weight = 1 }
                        }
                    }
                }
            };
            db.Surveys.Add(survey);
            db.SaveChanges();

            var service = new SurveyService(db);

            var answers = new List<SubmitAnswerDto>
            {
                new SubmitAnswerDto(questionId, new List<Guid> { optionId }, null),
                new SubmitAnswerDto(questionId, new List<Guid> { optionId }, null)
            };

            var (response, error) = await service.SubmitResponse(survey.Id, answers);

            Assert.Null(response);
            Assert.Contains("Duplicate answers found", error);
        }

        [Fact]
        public async Task SubmitResponse_ReturnsError_WhenAnswerOptionDoesNotExist()
        {
            using var db = CreateInMemoryDbContext();
            var questionId = Guid.NewGuid();
            var validOptionId = Guid.NewGuid();
            var invalidOptionId = Guid.NewGuid();
            var survey = new Survey
            {
                Id = Guid.NewGuid(),
                Title = "Test",
                Questions = new List<Question>
                {
                    new Question
                    {
                        Id = questionId,
                        Text = "Q1",
                        Type = QuestionType.SingleChoice,
                        AnswerOptions = new List<AnswerOption>
                        {
                            new AnswerOption { Id = validOptionId, Text = "A1", Weight = 1 }
                        }
                    }
                }
            };
            db.Surveys.Add(survey);
            db.SaveChanges();

            var service = new SurveyService(db);

            var answers = new List<SubmitAnswerDto>
            {
                new SubmitAnswerDto(questionId, new List<Guid> { invalidOptionId }, null)
            };

            var (response, error) = await service.SubmitResponse(survey.Id, answers);

            Assert.Null(response);
            Assert.Contains("Invalid answer option", error);
            Assert.Contains(invalidOptionId.ToString(), error);
        }
    }
}