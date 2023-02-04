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

            var item3 = new CategoryEntity()
            {
                Id = 2,
                Name = "Исторические деятели",
                Exersices = new List<ExersiceEntity>(_exerries.Values)
            };
            _categories.Add(item3.Id, item3);
        }

        private static void GenerateExercises()
        {
            var item1 = new ExersiceEntity()
            {
                Id = 0,
                Title = "Расположите в хронологической последовательности исторические события. Запишите цифры, которыми обозначены исторические события, в правильной последовательности в таблицу.",
                Body = @"
                    1)  Крымская война

                    2)  реформа патриарха Никона

                    3)  падение Византийской империи
                ",
                CorrectAnswer = "321",
                ExplanationExercise = "Пояснение. 1.  Крымская война  — 1853—1856 гг.\r\n\r\n2.  Реформа патриарха Никона  — с 1653 г.\r\n\r\n3.  Падение Византийской империи  — 1453 г."

            };
            _exerries.Add(item1.Id, item1);

            var item2 = new ExersiceEntity()
            {
                Id = 1,
                Title = "Установите соответствие между процессами (явлениями, событиями) и фактами, относящимися к этим процессам (явлениям, событиям): к каждой позиции первого столбца подберите соответствующую позицию из второго столбца.",
                Body = @"
                    ПРОЦЕССЫ (ЯВЛЕНИЯ, СОБЫТИЯ)
                    А)  формирование и развитие законодательства Древнерусского государства

                    Б)  реформы «Избранной рады»

                    В)  проведение политики «просвещённого абсолютизма» в России

                    Г)  первые революционные преобразования большевиков

                    ФАКТЫ
                    1)  созыв Уложенной комиссии

                    2)  принятие Судебника Ивана III

                    3)  созыв первого Земского собора

                    4)  принятие Декрета о земле

                    5)  принятие Русской Правды

                    6)  создание Временного правительства

                    Запишите в ответ цифры, расположив их в порядке, соответствующем буквам:
                ",
                CorrectAnswer = "5314",
                ExplanationExercise = "Пояснение. А)  формирование и развитие законодательства Древнерусского государства осуществляется в ходе составления Русской Правды в 11 веке;\r\n\r\nБ)  реформы «Избранной рады» осуществлялись при Иване Грозном, важнейший созыв Земского собора в 1549 г.;\r\n\r\nВ)  проведение политики «просвещённого абсолютизма» в России относится к Екатерине II, которая созвала Уложенную комиссию в 1767 г.;\r\n\r\nГ)  первые революционные преобразования большевиков в 1917 г.: Декрет о земле, Декрет о мире, Декрет о власти."

            };
            _exerries.Add(item2.Id, item2);

            var item3 = new ExersiceEntity()
            {
                Id = 2,
                Title = "Установите соответствие между государственными деятелями и историческими событиями.\r\n",
                Body = @"
                    ГОСУДАРСТВЕННЫЕ ДЕЯТЕЛИ
                    A)  П. А. Столыпин

                    Б)  А. А. Аракчеев

                    B)  А. Х. Бенкендорф

                    Г)  С. Ю. Витте

                    ИСТОРИЧЕСКИЕ СОБЫТИЯ
                    1)  создание военных поселений

                    2)  введение золотого стандарта

                    3)  разрешение свободного выхода крестьян из общины

                    4)  создание корпуса жандармов

                    5)  роспуск Учредительного собрания

                    Запишите в ответ цифры, расположив их в порядке, соответствующем буквам:
                ",
                CorrectAnswer = "3142",
                ExplanationExercise = "Пояснение. А)  П. А. Столыпин являлся автором аграрной реформы, целью которой было разрушение крестьянской общины.\r\n\r\nБ)  А. А. Аракчеев проводил политику создания военных поселений в правление Александра I.\r\n\r\nВ)  А. Х. Бенкендорф возглавлял III отделение Его Императорского Величества Канцелярии и корпус жандармов.\r\n\r\nГ)  С. Ю. Витте провел денежную реформу в 1897 г., результатом которой было введение золотого стандарта рубля.\r\n\r\nРоспуск Учредительного собрания произошел при В. И. Ленине в 1918 г."
            };
            _exerries.Add(item3.Id, item3);
        }

        public Task<List<int>> GetCategoryExerciseIdsListAsync(int categoryId)
        {
            return Task.FromResult(_categories[categoryId].Exersices
                .Select(e => e.Id)
                .ToList());
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