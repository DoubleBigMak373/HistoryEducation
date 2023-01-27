using Makar.HistoryEducation.Domain;
using System.Security.Cryptography.X509Certificates;

namespace Makar.HistoryEducation.Infrastructure
{
    public class ExerciseRepository : IExerciseRepository
    {
        private static Dictionary<int,ExersiceEntity> _exerries = new();
        private static Dictionary<int,CategoryEntity> _categories = new();

        static ExerciseRepository()
        {
            GenerateExercises();
            GenerateCategories();
        }

        private static void GenerateCategories()
        {
            var item1 = new CategoryEntity()
            {
                Id = 0,
                Name = "Хронология",
                Exersices = new List<ExersiceEntity>(_exerries.Values)
            };
            _categories.Add(item1.Id, item1);

            var item2 = new CategoryEntity()
            {
                Id = 1,
                Name = "Систематизация исторической информации",
                Exersices = new List<ExersiceEntity>(_exerries.Values)
            };
            _categories.Add(item2.Id, item2);
        }

        private static void GenerateExercises()
        {
            var item1 = new ExersiceEntity()
            {
                Id = 0,
                Title = "fdfdfdffdfd",
                Body = @"
                    1)  Крымская война

                    2)  реформа патриарха Никона

                    3)  падение Византийской империи
                "
            };
            _exerries.Add(item1.Id, item1);

            var item2 = new ExersiceEntity()
            {
                Id = 1,
                Title = "fdfdfdffdfd",
                Body = @"
                    1)  Крымская война

                    2)  реформа патриарха Никона

                    3)  падение Византийской империи
                "
            };
            _exerries.Add(item2.Id, item2);
        }


        public Task<List<CategoryEntity>> GetCategoryListAsync()
        {
            return Task.FromResult(_categories.Values.ToList());
        }

        public Task<ExersiceEntity> GetExersiceById(int id)
        {
            var entity = _exerries[id];
            return Task.FromResult(entity);
        }
    }
}