using CapIndexAssessment.Data;
using CapIndexAssessment.DTOs;
using CapIndexAssessment.Services;
using CapIndexAssessment.Validation;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<SurveyDbContext>(options => options.UseInMemoryDatabase("SurveyDb"));
builder.Services.AddScoped<ISurveyService, SurveyService>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateAnswerOptionValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateQuestionValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateSurveyValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateSurveyValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<SubmitResponseValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<SubmitAnswerValidator>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Survey Tool API", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// GET /api/surveys/{id}
// Retrieves a full survey structure by its Id.
app.MapGet("/api/surveys/{id}", async (Guid id, ISurveyService surveyService) =>
{
    var survey = await surveyService.GetSurveyById(id);

    if (survey is null)
    {
        return Results.NotFound();
    }

    return Results.Ok(survey);
})
.WithTags("Surveys");

// POST /api/surveys
// Creates a new survey structure.
app.MapPost("/api/surveys", async (CreateSurveyDto createDto, ISurveyService surveyService) =>
{
    var (newSurvey, error) = await surveyService.CreateSurvey(createDto);

    if (error != null)
    {
        return Results.BadRequest(new { message = error });
    }

    return Results.Created($"/api/surveys/{newSurvey!.Id}", newSurvey);
})
.AddFluentValidationAutoValidation()
.WithTags("Surveys"); // Group endpoints in Swagger UI

// PUT /api/surveys/{id}
// Updates an existing survey structure.
app.MapPut("/api/surveys/{id}", async (Guid id, UpdateSurveyDto updateDto, ISurveyService surveyService) =>
{
    if (id != updateDto.Id)
    {
        return Results.BadRequest(new { message = "Survey Id in URL does not match Survey Id in body." });
    }

    var (survey, error) = await surveyService.UpdateSurvey(updateDto);

    if (error != null)
    {
        return Results.BadRequest(new { message = error });
    }

    return Results.Ok(survey);
})
.AddFluentValidationAutoValidation()
.WithTags("Surveys");

// DELETE /api/surveys/{id}
// Deletes a survey if it has no responses.
app.MapDelete("/api/surveys/{id}", async (Guid id, ISurveyService surveyService) =>
{
    var (success, error) = await surveyService.DeleteSurveyAsync(id);

    if (!success)
    {
        if (error!.Contains("not found"))
        {
            return Results.NotFound(new { message = error });
        }

        return Results.BadRequest(new { message = error });
    }

    return Results.NoContent();
})
.WithTags("Surveys");

// GET /api/surveys/responses/{responseId}
// Retrieves a detailed view of a single survey response, including all questions, answers, and the total score.
app.MapGet("/api/surveys/responses/{responseId}", async (Guid responseId, ISurveyService surveyService) =>
{
    var responseDto = await surveyService.GetResponseDetails(responseId);

    if (responseDto is null)
    {
        return Results.NotFound(new { message = "The requested survey response was not found." });
    }

    return Results.Ok(responseDto);
})
.WithTags("Responses");

// POST /api/surveys/{surveyId}/responses
// Submits a user's response to a survey.
app.MapPost("/api/surveys/{surveyId}/responses", async (Guid surveyId, SubmitResponseDto responseDto, ISurveyService surveyService) =>
{
    if (surveyId != responseDto.SurveyId)
    {
        return Results.BadRequest(new { message = "Survey Id in URL does not match Survey Id in body." });
    }

    var (response, error) = await surveyService.SubmitResponse(surveyId, responseDto.Answers);

    if (error != null)
    {
        return Results.BadRequest(new { message = error });
    }

    return Results.Ok(new { responseId = response!.Id });
})
.AddFluentValidationAutoValidation()
.WithTags("Responses");

app.Run();