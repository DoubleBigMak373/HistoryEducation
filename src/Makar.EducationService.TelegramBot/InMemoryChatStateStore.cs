namespace Makar.EducationService.TelegramBot
{
    internal static class InMemoryChatStateStore
    {
        public static readonly Dictionary<long, ChatState> States = new();
    }
}
