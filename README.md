# CapIndexAssessment API

## Overview

CapIndexAssessment is a .NET 8 Web API for creating, managing, and responding to dynamic surveys. It supports conditional questions, multiple question types, and automatic scoring.

---

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/)

---

## How to Run the API

1. **Clone the repository:**
git clone <your-repo-url> cd CapIndexAssessment

2. **Build the solution:**
dotnet build

3. **Run the API:**
dotnet run --project CapIndexAssessment


4. **Access the API:**
   - The API will be available at `https://localhost:5001` (or the port shown in the console).
   - Swagger UI is available at `/swagger` in development mode.

---

## How to Test

- **Unit Tests:**  
  Run all tests using: dotnet test
  
The tests use the EF Core In-Memory provider for fast, isolated runs.

- **Manual Testing:**  
Use Swagger UI or tools like Postman to interact with endpoints.

---

## Architectural Decisions

- **.NET 8 Minimal API:**  
The API uses minimal API endpoints for simplicity and performance.

- **Entity Framework Core (In-Memory):**  
EF Core with the In-Memory provider is used for data persistence and easy testing. For production, swap to a persistent provider.

- **FluentValidation:**  
All DTOs are validated using FluentValidation, with automatic validation enabled for endpoints.

- **Service Layer:**  
Business logic is encapsulated in `SurveyService`, which implements `ISurveyService`. This keeps controllers/endpoints thin and testable.

- **DTOs and Entities:**  
DTOs are used for API contracts, and entities for persistence. Mapping is explicit in the service layer.

- **Separation of Concerns:** 
Validation, data access, and business logic are separated for maintainability and testability.

---

## Conditional Questions, Workflows, and Scoring

### Conditional Questions

- Each `Question` can have a `ParentQuestionId` and a `TriggeringAnswerOptionId`.
- When a user answers a question, only child questions whose `TriggeringAnswerOptionId` matches a selected answer option become visible.
- The method `GetVisibleQuestions` in `SurveyService` determines which questions are visible based on current answers.

### Workflows

- **Survey Creation:** 
Surveys are created with a hierarchy of questions and answer options. Local IDs are mapped to GUIDs for entity relationships.

- **Survey Update:**  
Surveys can be updated if they have no responses. The update process supports adding, removing, and editing questions and answer options.

- **Survey Deletion:** 
Surveys can be deleted if they have no responses. Deletion cascades to questions and answer options.

- **Survey Response:**  
When a response is submitted, the service:
  - Validates that all answered questions are visible and exist.
  - Ensures no duplicate answers for the same question.
  - Checks that all selected answer options exist for the question.
  - Stores the response and answers.

### Scoring

- Each `AnswerOption` has a `Weight`.
- The total score for a response is the sum of the weights of all selected answer options across all questions.
- The score is included in the detailed response DTO.

---

## Assumptions

- **Survey Structure:** 
All questions and answer options are created at survey creation time; dynamic addition/removal at runtime is only supported via the update endpoint and only if there are no responses.

- **Visibility:** 
Only questions whose parent/triggering conditions are met are considered visible and can be answered.

- **Validation:** 
- Duplicate answers for the same question are not allowed.
- Only valid answer options for a question can be selected.
- Free text answers are only allowed for `FreeText` questions.

- **Persistence:**  
The in-memory database is used for demonstration and testing.

- **Authentication/Authorization:** 
Not implemented; all endpoints are open for simplicity.

- **Cascade Deletes:**  
Deleting a survey will also delete all its questions and answer options, but only if there are no responses.
