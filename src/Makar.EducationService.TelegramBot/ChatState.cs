using System.Diagnostics.CodeAnalysis;

namespace Makar.EducationService.TelegramBot
{
    internal class ChatState
    {
        public long ChatId { get; private set; }
        public int? LastSendedExerciesId { get; private set; }

        private readonly Stack<int> _exerciseIds;

        public ChatState(
            long chatId,
            IEnumerable<int> exerciseIds)
        {
            ChatId = chatId;
            _exerciseIds = new Stack<int>(exerciseIds);
        }

        public bool TryGetNextExerciseId([NotNullWhen(true)]out int? exersiceId)
        {
            if (_exerciseIds.TryPop(out var value))
            {
                exersiceId = LastSendedExerciesId = value;

                return true;
            }

            exersiceId = null;
            return false;
        }

        public int GetExerciseCount()
        {
            return _exerciseIds.Count;
        }
    }
}

