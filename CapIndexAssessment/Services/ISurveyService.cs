using CapIndexAssessment.DTOs;
using CapIndexAssessment.Entities;

namespace CapIndexAssessment.Services
{
    public interface ISurveyService
    {
        Task<SurveyDto?> GetSurveyById(Guid surveyId);
        Task<(Survey? survey, string? error)> CreateSurvey(CreateSurveyDto createDto);
        Task<(Survey? survey, string? error)> UpdateSurvey(UpdateSurveyDto updateDto);
        Task<(bool success, string? error)> DeleteSurveyAsync(Guid surveyId);
        Task<(SurveyResponse? response, string? error)> SubmitResponse(Guid surveyId, ICollection<SubmitAnswerDto> submittedAnswers);
        Task<SurveyResponseDetailDto?> GetResponseDetails(Guid responseId);
    }
}
