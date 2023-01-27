namespace Makar.HistoryEducation.Domain
{
    public class CategoryEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<ExersiceEntity> Exersices { get; set; }
    }
}