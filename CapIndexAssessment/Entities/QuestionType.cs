namespace CapIndexAssessment.Entities
{
    /// <summary>
    /// The type of question in a survey.
    /// 
    /// 0 is Single Choice - A question with a single selectable answer, like a radio button.
    /// 
    /// 1 is Multiple Choice - A question with multiple selectable answers, like checkboxes.
    /// 
    /// 2 is Free Text - A question that expects a free text answer, like textboxes.
    /// </summary>
    public enum QuestionType
    {
        SingleChoice = 0,
        MultipleChoice = 1,
        FreeText = 2
    }
}