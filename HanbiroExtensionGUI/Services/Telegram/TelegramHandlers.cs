using HanbiroExtensionGUI.Constants;
using HanbiroExtensionGUI.Models;
using HanbiroExtensionGUI.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace HanbiroExtensionGUI.Services.Telegram
{
    public class TelegramHandlers
    {
        private AppSettings appSettings;
        private List<Models.User> users => appSettings.Users;
        public event EventHandler<Models.User> OnAddingUser;
        public event EventHandler<Models.User> OnUpdatingUser;
        public TelegramHandlers(AppSettings appSettings)
        {
            this.appSettings = appSettings;
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

            if (messageText == TelegramCommands.StartCommand
                || messageText == TelegramCommands.Login
                || messageText == TelegramCommands.LoginAgain)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("Please enter your Hanbiro Username");
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: stringBuilder.ToString(),
                    cancellationToken: cancellationToken);

                currentUser.Cookie = string.Empty;

                currentUser.LastCommand = TelegramCommands.GetUsernameCommand;
            }
            else if (currentUser.LastCommand == TelegramCommands.GetUsernameCommand)
            {
                if (currentUser.UserName != messageText)
                {
                    currentUser.Cookie = string.Empty;
                }
                currentUser.UserName = messageText;

                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("Please enter your Hanbiro password");
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: stringBuilder.ToString(),
                    cancellationToken: cancellationToken);

                currentUser.LastCommand = TelegramCommands.GetPasswordCommand;
            }
            else if (currentUser.LastCommand == TelegramCommands.GetPasswordCommand)
            {
                string encryptedPassword = EncryptionUtils.Encrypt(messageText);
                currentUser.Password = encryptedPassword;

                StringBuilder stringBuilder = new StringBuilder();

                if(appSettings.IsClockingIn || appSettings.IsClockingOut)
                {
                    stringBuilder.AppendLine("System Busy!!!");
                    stringBuilder.AppendLine("Please wait a few minutes before you try again.");
                    stringBuilder.AppendLine("[After 20 minues]");
                }
                else
                {
                    stringBuilder.AppendLine("You account is verifying...");
                    stringBuilder.AppendLine("Please wait a few minutes!!!");
                    stringBuilder.AppendLine("If after 5 minutes it's not finished, please contact me.");

                    currentUser.LoginDate = DateTime.Now;
                    currentUser.IsActive = true;
                }

                Message sentMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: stringBuilder.ToString(),
                        cancellationToken: cancellationToken);

                currentUser.LastCommand = string.Empty;
            }
            else if (string.IsNullOrEmpty(currentUser.UserName)
               || string.IsNullOrEmpty(currentUser.Password))
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("You need to login first to continue");
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: stringBuilder.ToString(),
                    cancellationToken: cancellationToken);

                currentUser.IsActive = false;
                OnUpdatingUser?.Invoke(this, currentUser);
                return;
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
                    text: "Logout Success!!!",
                    replyMarkup: replyKeyboardMarkup,
                    cancellationToken: cancellationToken);

                currentUser.LastCommand = string.Empty;
                currentUser.IsActive = false;
            }
            else if (messageText == TelegramCommands.Active)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("Bot is actived!!!");

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

                Message sentMessage = await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: stringBuilder.ToString(),
                    replyMarkup: replyKeyboardMarkup,
                    cancellationToken: cancellationToken);

                currentUser.IsActive = true;
            }
            else if (messageText == TelegramCommands.Deactive)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("Bot is deactived!!!");

                ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
                {
                    new KeyboardButton[] {  TelegramCommands.Active},
                    new KeyboardButton[] {  TelegramCommands.Donate},
                    new KeyboardButton[] {  TelegramCommands.Contact},
                    new KeyboardButton[] {  TelegramCommands.Logout},
                })
                {
                    ResizeKeyboard = true
                };

                Message sentMessage = await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: stringBuilder.ToString(),
                    replyMarkup: replyKeyboardMarkup,
                    cancellationToken: cancellationToken);

                currentUser.IsActive = false;
            }
            else if (messageText == TelegramCommands.Donate)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("Hãy ủng hộ cho em để em có thêm nhiều cảm hứng và nỗ lực tạo ra các công cụ giúp ích cho mọi người nhé.");
                FileStream file = new FileStream($"{AppContext.BaseDirectory}/images/tpbank_qrcode.jpg", FileMode.Open);
                await botClient.SendPhotoAsync(
                    chatId: chatId,
                    photo: new InputOnlineFile(file),
                    caption: stringBuilder.ToString(),
                    cancellationToken: cancellationToken);

                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "01870655888",
                    cancellationToken: cancellationToken);
            }
            else if (messageText == TelegramCommands.Contact)
            {
                Message message = await botClient.SendContactAsync(
                chatId: chatId,
                phoneNumber: "+84327961199",
                firstName: "Pham Quang",
                vCard: "BEGIN:VCARD\n" +
                       "VERSION:3.0\n" +
                       "N:Pham;Quang\n" +
                       "ORG:Developer at Hanoi,Vietnam\n" +
                       "TEL;TYPE=voice,work,pref:+84327961199\n" +
                       "EMAIL:quangpv598@gmail.com\n" +
                       "END:VCARD",
                cancellationToken: cancellationToken);
            }

            OnUpdatingUser?.Invoke(this, currentUser);
        }
    }
}
