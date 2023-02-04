namespace Makar.HistoryEducation.Application.Contracts
{
    public class ExersiceEntityDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string CorrectAnswer { get; set; }
        public string ExplanationExercise { get; set; }
    }
}