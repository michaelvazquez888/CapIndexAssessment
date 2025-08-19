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
    c.SwaggerDoc("v1", new()
    {
        Title = "Survey Tool API",
        Version = "v1",
        Description = "API for creating and managing dynamic surveys with conditional questions and automatic scoring."
    });

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/api/surveys/{id}", async (Guid id, ISurveyService surveyService) =>
{
    var survey = await surveyService.GetSurveyById(id);

    if (survey is null)
    {
        return Results.NotFound();
    }

    return Results.Ok(survey);
})
.WithTags("Surveys")
.WithOpenApi(op => {
op.Summary = "Get a survey by Id";
op.Description = """
    Retrieves the full structure of a survey, including all questions and answer options, by its unique id.

    **Example**
    `GET /api/surveys/3fa85f64-5717-4562-b3fc-2c963f66afa6`

    **Response**
    ```json
    {
      "id": "d7809dea-41e2-41de-8073-68fe9347f41e",
      "title": "Employee Satisfaction Survey",
      "questions": [
        {
          "id": "a6ea7f3d-759b-4327-a196-fcba172d1c95",
          "text": "How satisfied are you with your work environment?",
          "type": 0,
          "parentQuestionId": null,
          "triggeringAnswerOptionId": null,
          "answerOptions": [
            {
              "id": "68662f0b-a2f1-4f07-8924-9e7ac782cc94",
              "text": "Very Satisfied",
              "weight": 5
            },
            {
              "id": "e034a618-3625-4dc9-8ffb-019818bee63f",
              "text": "Satisfied",
              "weight": 4
            }
          ]
        }
      ]
    }
    ```
    """;
    return op;
});

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
.WithTags("Surveys")
.WithOpenApi(op =>
{
    op.Summary = "Create a new survey";
    op.Description = """
    Creates a new survey with a set of questions and answer options. Returns the created survey structure.

    **Example**
    `POST /api/surveys`

    **Request**
    ```json
    {
      "title": "Employee Satisfaction Survey",
      "questions": [
        {
          "localId": "q1",
          "text": "How satisfied are you with your work environment?",
          "type": 0,
          "parentLocalId": null,
          "triggeringAnswerLocalId": null,
          "answerOptions": [
            {
              "localId": "a1",
              "text": "Very Satisfied",
              "weight": 5
            },
            {
              "localId": "a2",
              "text": "Satisfied",
              "weight": 4
            }
          ]
        }
      ]
    }
    ```

    **Response**
    ```json
    {
      "id": "d7809dea-41e2-41de-8073-68fe9347f41e",
      "title": "Employee Satisfaction Survey",
      "questions": [
        {
          "id": "a6ea7f3d-759b-4327-a196-fcba172d1c95",
          "text": "How satisfied are you with your work environment?",
          "type": 0,
          "surveyId": "d7809dea-41e2-41de-8073-68fe9347f41e",
          "parentQuestionId": null,
          "triggeringAnswerOptionId": null,
          "answerOptions": [
            {
              "id": "68662f0b-a2f1-4f07-8924-9e7ac782cc94",
              "text": "Very Satisfied",
              "weight": 5,
              "questionId": "a6ea7f3d-759b-4327-a196-fcba172d1c95"
            },
            {
              "id": "e034a618-3625-4dc9-8ffb-019818bee63f",
              "text": "Satisfied",
              "weight": 4,
              "questionId": "a6ea7f3d-759b-4327-a196-fcba172d1c95"
            }
          ]
        }
      ]
    }
    ```
    """;
    return op;
});

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
.WithTags("Surveys")
.WithOpenApi(op => {
    op.Summary = "Update an existing survey";
    op.Description = """
    Updates the title, questions, and answer options of an existing survey. Only surveys without responses can be updated.

    **Example**
    `PUT /api/surveys/3fa85f64-5717-4562-b3fc-2c963f66afa6`

    **Request**
    ```json
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "title": "Updated Survey Title",
      "questions": [
        {
          "id": "b1a1c1d1-1234-5678-9abc-def012345678",
          "text": "How satisfied are you with your work environment?",
          "type": 0,
          "parentQuestionId": null,
          "triggeringAnswerOptionId": null,
          "answerOptions": [
            {
              "id": "a1b2c3d4-5678-1234-9abc-def012345678",
              "text": "Very Satisfied",
              "weight": 5
            }
          ]
        }
      ]
    }
    ```

    **Response**
    ```json
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "title": "Updated Survey Title",
      "questions": [
        {
          "id": "b1a1c1d1-1234-5678-9abc-def012345678",
          "text": "How satisfied are you with your work environment?",
          "type": 0,
          "parentQuestionId": null,
          "triggeringAnswerOptionId": null,
          "answerOptions": [
            {
              "id": "a1b2c3d4-5678-1234-9abc-def012345678",
              "text": "Very Satisfied",
              "weight": 5
            }
          ]
        }
      ]
    }
    ```
    """;
    return op;
});

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
.WithTags("Surveys")
.WithOpenApi(op => {
    op.Summary = "Delete a survey";
    op.Description = """
    Deletes a survey by its unique id. Surveys with existing responses cannot be deleted.

    **Example**
    `DELETE /api/surveys/3fa85f64-5717-4562-b3fc-2c963f66afa6`

    **Response (Success)**
    HTTP 204 No Content

    **Response (Failure)**
    ```json
    { "message": "Survey not found." }
    ```

    or

    ```json
    { "message": "Cannot delete a survey that has responses." }
    ```
    """;
    return op;
});

app.MapGet("/api/surveys/responses/{responseId}", async (Guid responseId, ISurveyService surveyService) =>
{
    var responseDto = await surveyService.GetResponseDetails(responseId);

    if (responseDto is null)
    {
        return Results.NotFound(new { message = "The requested survey response was not found." });
    }

    return Results.Ok(responseDto);
})
.WithTags("Responses")
.WithOpenApi(op => {
    op.Summary = "Get survey response details";
    op.Description = """
    Retrieves a detailed view of a single survey response, including all answered questions, selected options, and the total score.

    **Example**
    `GET /api/surveys/responses/e1f2a3b4-5678-1234-9abc-def012345678`

    **Response**
    ```json
    {
      "id": "e1f2a3b4-5678-1234-9abc-def012345678",
      "surveyId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "submittedAt": "2025-08-19T12:34:56Z",
      "answers": [
        {
          "questionId": "b1a1c1d1-1234-5678-9abc-def012345678",
          "questionText": "How satisfied are you with your work environment?",
          "freeTextAnswer": null,
          "selectedOptionTexts": [
            "Very Satisfied"
          ]
        }
      ],
      "totalScore": 5
    }
    ```
    """;
    return op;
});

app.MapPost("/api/surveys/{surveyId}/responses", async (Guid surveyId, SubmitResponseDto responseDto, ISurveyService surveyService) =>
{
    if (surveyId != responseDto.SurveyId)
    {
        return Results.BadRequest(new { message = "Survey Id in URL does not match survey Id in body." });
    }

    var (response, error) = await surveyService.SubmitResponse(surveyId, responseDto.Answers);

    if (error != null)
    {
        return Results.BadRequest(new { message = error });
    }

    return Results.Ok(new { responseId = response!.Id });
})
.AddFluentValidationAutoValidation()
.WithTags("Responses")
.WithOpenApi(op => {
    op.Summary = "Submit a survey response";
    op.Description = """
    Submits a user's answers to a survey. Validates all answers and returns the response id if successful.

    **Example**
    `POST /api/surveys/3fa85f64-5717-4562-b3fc-2c963f66afa6/responses`

    **Request**
    ```json
    {
      "surveyId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "answers": [
        {
          "questionId": "b1a1c1d1-1234-5678-9abc-def012345678",
          "selectedOptionIds": [
            "a1b2c3d4-5678-1234-9abc-def012345678"
          ],
          "freeTextAnswer": null
        }
      ]
    }
    ```

    **Response**
    ```json
    { "responseId": "e1f2a3b4-5678-1234-9abc-def012345678" }
    ```

    **Error**
    ```json
    { "message": "Survey not found." }
    ```
    """;
    return op;
});

app.Run();