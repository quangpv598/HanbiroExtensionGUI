using HanbiroExtensionConsole.Constants;
using HanbiroExtensionConsole.Models;
using HanbiroExtensionConsole.Services.Telegram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace HanbiroExtensionConsole.Services
{
    public class TelegramService
    {
        #region Fields
        private readonly string telegramToken;
        private readonly TelegramHandlers telegramHandlers;
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly TelegramBotClient telegramBotClient;
        #endregion

        #region Properties

        #endregion

        #region Constructors
        public TelegramService(string telegramToken, TelegramHandlers telegramHandlers)
        {
            this.telegramToken = telegramToken;
            this.telegramHandlers = telegramHandlers;

            telegramBotClient = new TelegramBotClient(telegramToken);

            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { } // receive all update types
            };

            telegramBotClient.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken: cancellationTokenSource.Token);

        }

        ~TelegramService()
        {
            // Send cancellation request to stop bot
            cancellationTokenSource.Cancel();
        }

        #endregion

        #region Events
        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            await telegramHandlers.HandleUpdateAsync(botClient, update, cancellationToken);
        }

        Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
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

        #endregion

        #region Methods
        public async void SendMessageToUser(Models.User user, string message)
        {
            await telegramBotClient.SendTextMessageAsync(
                chatId: user.TelegramId,
                text: message);
        }

        public async void LogoutUser(Models.User user, string message)
        {
            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
                {
                    new KeyboardButton[] {  TelegramCommands.Login},
                })
            {
                ResizeKeyboard = true
            };

            Message sentMessage = await telegramBotClient.SendTextMessageAsync(
                chatId: user.TelegramId,
                text: message,
                replyMarkup: replyKeyboardMarkup);

            user.LastCommand = string.Empty;
            user.IsActive = false;
            user.UserName = string.Empty;
            user.Password = string.Empty;
            user.Cookie = string.Empty;
        }
        #endregion
    }
}
