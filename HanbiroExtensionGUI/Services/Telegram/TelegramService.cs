using HanbiroExtensionGUI.Constants;
using HanbiroExtensionGUI.Models;
using HanbiroExtensionGUI.Services.Telegram;
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

namespace HanbiroExtensionGUI.Services
{
    public class TelegramService
    {
        #region Fields
        private readonly List<long> adminítrators;
        private readonly string telegramToken;
        private readonly TelegramHandlers telegramHandlers;
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly TelegramBotClient telegramBotClient;
        #endregion

        #region Properties

        #endregion

        #region Constructors
        public TelegramService(string telegramToken, TelegramHandlers telegramHandlers, List<long> adminítrators)
        {
            this.telegramToken = telegramToken;
            this.telegramHandlers = telegramHandlers;
            this.adminítrators = adminítrators;

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
        public async void SendMessageToAdminitrators(string message, Models.User sender = null)
        {
            StringBuilder stringBuilder = new StringBuilder();
            //stringBuilder.AppendLine("[Admin]");
            stringBuilder.AppendLine(message);
            if(sender is not null)
            {
                stringBuilder.AppendLine($"[Sender : {sender.UserName}-{sender.Email}-{sender.FullName}]");
            }
            foreach (long id in adminítrators)
            {
                await telegramBotClient.SendTextMessageAsync(
                chatId: id,
                text: stringBuilder.ToString());
            }
        }
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
        }

        public async void SendLoginSuccess(Models.User user)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"Hi {user.FullName}!!!");
            stringBuilder.AppendLine("The system will randomly clock in between 7:30 and 7:40 AM and randomly clock out between 6:03 and 6:08 PM every day");

            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
            {
                    new KeyboardButton[] {  TelegramCommands.Deactive},
                    new KeyboardButton[] {  TelegramCommands.Donate},
                    new KeyboardButton[] {  TelegramCommands.Contact},
                    new KeyboardButton[] {  TelegramCommands.Logout},
                })
            {
                ResizeKeyboard = true
            };

            Message sentMessage = await telegramBotClient.SendTextMessageAsync(
                chatId: user.TelegramId,
                text: stringBuilder.ToString(),
                replyMarkup: replyKeyboardMarkup);

            user.IsActive = true;

            SendMessageToAdminitrators($"User Register Success : {user.UserName}-{user.Email}-{user.FullName}");
        }

        public async void SendTryAgain(Models.User user)
        {
            StringBuilder message = new StringBuilder();
            message.AppendLine("Your username or password is incorrect !!!");

            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
            {
                    new KeyboardButton[] {  TelegramCommands.LoginAgain}
                })
            {
                ResizeKeyboard = true
            };

            Message sentMessage = await telegramBotClient.SendTextMessageAsync(
                chatId: user.TelegramId,
                text: message.ToString(),
                replyMarkup: replyKeyboardMarkup);

            user.IsActive = false;
        }
        #endregion
    }
}
