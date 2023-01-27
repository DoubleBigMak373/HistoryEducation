using Makar.HistoryEducation.Application.Contracts;
using Makar.HistoryEducation.Domain;

namespace Makar.HistoryEducation.Application
{
    public class EducationAppServise : IEducationServiceAppContracts
    {
        private readonly IExerciseRepository exerciseRepository;
         
        public EducationAppServise(IExerciseRepository exerciseRepository)
        {
            this.exerciseRepository = exerciseRepository;
        }

        public async Task<List<CategoryDto>> GetCatalogAsync()
        {
            var items = await exerciseRepository.GetCategoryListAsync();

            return items
                .Select(a => new CategoryDto()
                {
                    Id = a.Id,
                    Name = a.Name,
                }).ToList();
        }

        public Task<List<int>> GetCategoryExercisesAsync(int categoryId)
        {
            throw new NotImplementedException();
        }

        public async Task<ExersiceEntityDto> GetExerciseAsync(int id)
        {
            var entity = await exerciseRepository.GetExersiceById(id);

            return new ExersiceEntityDto()
            {
                Id = entity.Id,
                Title = entity.Title,
                Body = entity.Body,
            };
        }

        public Task<VerifyResultDto> VerifyAnswerAsync(int exerciseId, string answer)
        {
            throw new NotImplementedException();
        }


    }
}