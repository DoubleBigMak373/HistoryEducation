namespace Makar.HistoryEducation.Domain
{
    public interface IExerciseRepository
    {
        Task<ExersiceEntity> GetExersiceById(int id);
        Task<List<CategoryEntity>> GetCategoryListAsync();
        Task<List<int>> GetCategoryExerciseIdsListAsync(int categoryId);
    }
}
