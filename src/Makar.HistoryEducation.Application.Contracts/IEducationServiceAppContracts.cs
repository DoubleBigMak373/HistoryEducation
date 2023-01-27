namespace Makar.HistoryEducation.Application.Contracts
{
    public interface IEducationServiceAppContracts
    {
        public Task<ExersiceEntityDto> GetExerciseAsync(int id);
        public Task<List<CategoryDto>> GetCatalogAsync();
        public Task<List<int>> GetCategoryExercisesAsync(int categoryId);
        public Task<VerifyResultDto> VerifyAnswerAsync(int exerciseId, string answer);
    }
}