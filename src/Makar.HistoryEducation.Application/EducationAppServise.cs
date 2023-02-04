using Makar.HistoryEducation.Application.Contracts;
using Makar.HistoryEducation.Domain;

namespace Makar.HistoryEducation.Application
{
    public class EducationAppServise : IEducationServiceAppContracts
    {
        private readonly IExerciseRepository _exerciseRepository;
         
        public EducationAppServise(IExerciseRepository exerciseRepository)
        {
            _exerciseRepository = exerciseRepository;
        }

        public async Task<List<CategoryDto>> GetCatalogAsync()
        {
            var items = await _exerciseRepository.GetCategoryListAsync();

            return items
                .Select(a => new CategoryDto()
                {
                    Id = a.Id,
                    Name = a.Name,
                })
                .ToList();
        }

        public Task<List<int>> GetCategoryExercisesAsync(int categoryId)
        {
            return _exerciseRepository.GetCategoryExerciseIdsListAsync(categoryId);
        }

        public async Task<ExersiceEntityDto> GetExerciseAsync(int id)
        {
            var entity = await _exerciseRepository.GetExersiceById(id);

            return new ExersiceEntityDto()
            {
                Id = entity.Id,
                Title = entity.Title,
                Body = entity.Body,
            };
        }

        public async Task<VerifyResultDto> VerifyAnswerAsync(int exerciseId, string answer)
        {
            var entity = await _exerciseRepository.GetExersiceById(exerciseId);
            var entityAnswer = entity.CorrectAnswer;
            var entityExplanation = entity.ExplanationExercise;

            if (entityAnswer == answer)
            {
                return new VerifyResultDto()
                {
                    IsValid = true,
                };
            }

            return new VerifyResultDto()
            {
                IsValid = false,
                Explanation = entityExplanation,
            };
        }
    }
}
