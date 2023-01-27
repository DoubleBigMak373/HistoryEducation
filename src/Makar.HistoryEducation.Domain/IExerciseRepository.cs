using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makar.HistoryEducation.Domain
{
    public interface IExerciseRepository
    {
        public Task<ExersiceEntity> GetExersiceById(int id);
        public Task<List<CategoryEntity>> GetCategoryListAsync();
    }
}
