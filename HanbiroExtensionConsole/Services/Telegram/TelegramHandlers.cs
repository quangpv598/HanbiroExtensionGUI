using HanbiroExtensionConsole.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace HanbiroExtensionConsole.Services.Telegram
{
    public class TelegramHandlers
    {
        private readonly List<Models.User> users;
        public event EventHandler<Models.User> OnAddingUser;
        public event EventHandler<Models.User> OnUpdatingUser;
        public TelegramHandlers(List<Models.User> users)
        {
            this.users = users;
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Only process Message updates: https://core.telegram.org/bots/api#message
            if (update.Type != UpdateType.Message)
                return;
            // Only process text messages
            if (update.Message!.Type != MessageType.Text)
                return;

            var chatId = update.Message.Chat.Id;
            var messageText = update.Message.Text;

            if (users.Count(u => u.TelegramId == chatId) == 0)
            {
                OnAddingUser?.Invoke(this, new Models.User
                {
                    TelegramId = chatId
                });
            }

            var currentUser = users.FirstOrDefault(u => u.TelegramId == chatId);
            if (currentUser == null) return;

            if (messageText == TelegramCommands.StartCommand)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("Welcome to Hanbiro Extension Bot!");
                stringBuilder.AppendLine("Please enter your username");
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: stringBuilder.ToString(),
                    cancellationToken: cancellationToken);

                currentUser.LastCommand = TelegramCommands.GetUsernameCommand;
            }
            else if (currentUser.LastCommand == TelegramCommands.GetUsernameCommand)
            {
                currentUser.UserName = messageText;

                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("Please enter your password");
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: stringBuilder.ToString(),
                    cancellationToken: cancellationToken);

                currentUser.LastCommand = TelegramCommands.GetPasswordCommand;
            }
            else if (currentUser.LastCommand == TelegramCommands.GetPasswordCommand)
            {
                currentUser.Password = messageText;

                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("Register Success!!!");
                stringBuilder.AppendLine("It will clock in at 6 AM and clock out at 6:15 PM every day.");
                stringBuilder.AppendLine("You will receive the photo of result.");

                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: stringBuilder.ToString(),
                    cancellationToken: cancellationToken);

                ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
                {
                    new KeyboardButton[] {  TelegramCommands.Logout},
                })
                {
                    ResizeKeyboard = true
                };

                Message sentMessage = await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "You can press the menu to see the option.",
                    replyMarkup: replyKeyboardMarkup,
                    cancellationToken: cancellationToken);

                currentUser.LastCommand = string.Empty;
                currentUser.IsActive = true;
            }
            else if (messageText == TelegramCommands.Logout)
            {
                ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
                {
                    new KeyboardButton[] {  TelegramCommands.Login},
                })
                {
                    ResizeKeyboard = true
                };

                Message sentMessage = await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "Press Login button to use",
                    replyMarkup: replyKeyboardMarkup,
                    cancellationToken: cancellationToken);

                currentUser.LastCommand = string.Empty;
                currentUser.IsActive = false;
                currentUser.UserName = string.Empty;
                currentUser.Password = string.Empty;
            }
            else if (messageText == TelegramCommands.Login)
            {
                Message sentMessage = await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "Hello!!! Thank for using me.",
                    replyMarkup: new ReplyKeyboardRemove(),
                    cancellationToken: cancellationToken);

                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("Please enter your username");
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: stringBuilder.ToString(),
                    cancellationToken: cancellationToken);

                currentUser.LastCommand = TelegramCommands.GetUsernameCommand;
            }

            OnUpdatingUser?.Invoke(this, currentUser);
        }
    }
}
