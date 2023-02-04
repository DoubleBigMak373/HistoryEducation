namespace Makar.HistoryEducation.Shared
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<int> Mix(this List<int> source)
        {
            HashSet<int> newList = new(source.Count);

            var rnd = new Random();

            while (newList.Count != source.Count)
            {
                var randomIndex = rnd.Next(0, source.Count);

                newList.Add(source[randomIndex]);
            }

            return newList;
        }
    }
}
