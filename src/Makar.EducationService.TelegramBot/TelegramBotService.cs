using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Makar.HistoryEducation.Application.Contracts;
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
            if (update.Message is not { } message)
                return;
            // Only process text messages
            if (message.Text is not { } messageText)
                return;

            var chatId = message.Chat.Id;

            Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

            // Echo received message text
            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: $@"
                    Здорова, {message.Chat.FirstName} {message.Chat.LastName}, я - Макар, а это - мой бот.
                    Отправь мне <b>/categories</b> для старта
                ",
                parseMode: ParseMode.Html,
                cancellationToken: cancellationToken);

            if (messageText.ToLower() == "/categories")
            {
                var categories = await _applicaiton.GetCatalogAsync();

                var categoryReplies = categories
                    .Select(category => InlineKeyboardButton.WithCallbackData(text: category.Name, callbackData: category.Id.ToString()))
                    .ToList();

                InlineKeyboardMarkup inlineKeyboard = new(categoryReplies);

                await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Вот мои категории заданий",
                replyMarkup: inlineKeyboard,
                cancellationToken: cancellationToken);
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