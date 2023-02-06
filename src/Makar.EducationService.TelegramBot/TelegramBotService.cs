using Makar.HistoryEducation.Application.Contracts;
using Makar.HistoryEducation.Shared;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Makar.EducationService.TelegramBot
{
    public class TelegramBotService
    {
        private readonly IEducationServiceAppContracts _applicaiton;

        public TelegramBotService(IEducationServiceAppContracts applicaiton)
        {
            _applicaiton = applicaiton;
        }

        public async Task InitializeAsync()
        {
            var botClient = new TelegramBotClient("5860903492:AAHPRb43zmRYKN8EGSa5hKy-xsVGTA4mGuk"); // !!!!!

            using CancellationTokenSource cts = new();

            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
            ReceiverOptions receiverOptions = new()
            {
                AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
            };

            botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token
            );

            var me = await botClient.GetMeAsync();

            Console.WriteLine($"Start listening for @{me.Username}");
        }

        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // тут проверка что в бота прилетело обычное текстовое что-то
            if (update.Type == UpdateType.Message)
            {
                await HandleMessageAsync(botClient, update, cancellationToken);

                
                return;
            }

            // если сюда дошло, значит предыдущая проверка не прошла и прилетело в бота не текст
            // проверяю: а вдруг это колбек
            if (update.CallbackQuery is not null)
            {
                // вызов метода обработки колбеков
                await HandleCallbackAsync(botClient, update, cancellationToken);

                return;
            }
        }

        private async Task HandleCallbackAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.CallbackQuery?.Message is null)
            {
                throw new InvalidOperationException($"Обрабатываемое сообщение не является типом {UpdateType.CallbackQuery}");
            }

            if (DateTime.UtcNow - update.CallbackQuery.Message.Date > TimeSpan.FromMinutes(10))
            {
                return;
            }

            var chatId = update.CallbackQuery.Message.Chat.Id;

            var callbackData = update.CallbackQuery.Data;

            if (callbackData is not null && callbackData.StartsWith("category_"))
            {
                var categoryId = int.Parse(callbackData.Split("_").Last());
                var exerciseIds = await _applicaiton.GetCategoryExercisesAsync(categoryId);

                if (InMemoryChatStateStore.States.TryGetValue(chatId, out var state) && state.LastSendedExerciesId.HasValue)
                {
                    // TODO написать пользователю вы правда хотите задание по новой категории?
                    // new_category_1

                    Console.WriteLine("Кто-то нажал повторно кнопку с категорией, когда уже пошли задания");

                    await botClient.SendTextMessageAsync(
                       chatId: chatId,
                       text: $"Вы уже выбрали категорию, пожалуйста завершите все задания. Осталось {state.GetExerciseCount()}",
                       cancellationToken: cancellationToken);

                    return;
                }
                else
                {
                    state = new ChatState(chatId, exerciseIds.Mix());
                    InMemoryChatStateStore.States[chatId] = state;
                }

                if (state.TryGetNextExerciseId(out var exerciseId))
                {
                    var exersice = await _applicaiton.GetExerciseAsync(exerciseId.Value);

                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: $@"
                            Начнем выполнение заданий!
                            {exersice.Title}
                            {exersice.Body}
                        ",
                        cancellationToken: cancellationToken);
                }

                Console.WriteLine($"Pressed button = {callbackData}");
            }
        }

        private async Task HandleMessageAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Message is null)
            {
                throw new InvalidOperationException($"Обрабатываемое сообщение не является типом {UpdateType.Message}");
            }

            if (DateTime.UtcNow - update.Message.Date > TimeSpan.FromMinutes(10))
            {
                return;
            }

            // Only process text messages
            if (update.Message.Text is not { } messageText)
                return;

            var chatId = update.Message.Chat.Id;

            Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

            if (messageText == "/start")
            {
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: $@"
                        Приветствую! Я - Макар Андреевич, а это - мой бот.
                        Сие творение поможет вам достойно сдать ЕГЭ. 
                        Отправь мне <b>/categories</b> для старта
                    ",
                    parseMode: ParseMode.Html,
                    cancellationToken: cancellationToken);

                return;
            }

            if (messageText.ToLower() == "/categories")
            {
                var categories = await _applicaiton.GetCatalogAsync();

                var categoryReplies = categories
                    .Select(category => InlineKeyboardButton.WithCallbackData(text: category.Name, callbackData: $"category_{category.Id}"))
                    .ToList();

                InlineKeyboardMarkup keyboard = new(categoryReplies);

                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "Вот мои категории заданий",
                    replyMarkup: keyboard,
                    cancellationToken: cancellationToken);

                return;
            }

            if (InMemoryChatStateStore.States.TryGetValue(chatId, out var state) && state.LastSendedExerciesId.HasValue)
            {
                var verifyResult = await _applicaiton.VerifyAnswerAsync(state.LastSendedExerciesId.Value, messageText);

                if (verifyResult.IsValid)
                {
                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "Правильный ответ!",
                        cancellationToken: cancellationToken);

                    // следующее задание
                    if (state.TryGetNextExerciseId(out var nextExerciseId))
                    {
                        var exersice = await _applicaiton.GetExerciseAsync(nextExerciseId.Value);

                        await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: $@"
                                {exersice.Title}
                                {exersice.Body}
                            ",
                            parseMode: ParseMode.Html,
                            cancellationToken: cancellationToken);
                    }
                    else
                    {
                        InMemoryChatStateStore.States.Remove(chatId);

                        await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "Все задания по категории закончились, поздравляю! Можете выбрать другую .....",
                            cancellationToken: cancellationToken);
                    }
                }
                else
                {
                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: $@"
                            Неправильный ответ! Вот объяснение, посмотри внимательно и реши снова!
                            {verifyResult.Explanation}
                        ",
                        cancellationToken: cancellationToken);
                }
            }
        }

        Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
    }
}
