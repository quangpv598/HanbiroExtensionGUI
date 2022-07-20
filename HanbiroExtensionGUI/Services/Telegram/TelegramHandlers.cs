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
        public event EventHandler OnClockManually;
        public TelegramService telegramService;
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
                stringBuilder.AppendLine("Bạn hãy điền tên đăng nhập tài khoản Hanbiro");
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: stringBuilder.ToString(),
                    replyMarkup: new ReplyKeyboardRemove(),
                    cancellationToken: cancellationToken);

                currentUser.Cookie = string.Empty;
                currentUser.IsActive = false;
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
                stringBuilder.AppendLine("Bạn hãy điền mật khẩu đăng nhập");
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

                if (appSettings.IsClockingIn || appSettings.IsClockingOut || appSettings.IsAutoRestartTime)
                {
                    stringBuilder.AppendLine("Hệ thống bận!!!");
                    stringBuilder.AppendLine("Vui lòng thử lại sau 30 phút.");
                }
                else
                {
                    //stringBuilder.AppendLine("You account is verifying...");
                    //stringBuilder.AppendLine("Please wait a few minutes!!!");
                    //stringBuilder.AppendLine("If after 5 minutes it's not finished, please contact me.");

                    stringBuilder.AppendLine("Đăng ký thành công!!!");
                    stringBuilder.AppendLine("Bạn sẽ nhận được kết quả sau khi hệ thống kiểm tra thông tin tài khoản đăng nhập.");
                    stringBuilder.AppendLine("Xin cảm ơn!!!");

                    currentUser.LoginDate = DateTime.Now;
                    currentUser.IsActive = true;
                }

                Message sentMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: stringBuilder.ToString(),
                        cancellationToken: cancellationToken);

                telegramService.SendMessageToAdminitrators($"New User : {currentUser.UserName}");

                currentUser.LastCommand = string.Empty;
            }
            else if (string.IsNullOrEmpty(currentUser.UserName)
               || string.IsNullOrEmpty(currentUser.Password))
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("Bạn hãy đăng nhập để tiếp tục");
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
                    text: "Đăng xuất thành công!!!",
                    replyMarkup: replyKeyboardMarkup,
                    cancellationToken: cancellationToken);

                telegramService.SendMessageToAdminitrators($"User logout : {currentUser.UserName}-{currentUser.FullName}-{currentUser.Email}");

                currentUser.LastCommand = string.Empty;
                currentUser.IsActive = false;
            }
            else if (messageText == TelegramCommands.Active)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("Hệ thống đã được kích hoạt!!!");

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

                telegramService.SendMessageToAdminitrators($"User active bot : {currentUser.UserName}-{currentUser.FullName}-{currentUser.Email}");

                currentUser.IsActive = true;
            }
            else if (messageText == TelegramCommands.Deactive)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("Hệ thống đã huỷ kích hoạt!!!");

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

                telegramService.SendMessageToAdminitrators($"User deactive bot : {currentUser.UserName}-{currentUser.FullName}-{currentUser.Email}");

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

            long adminId = 1021122605;
            if (chatId == adminId)
            {
                if (messageText == TelegramCommands.ClockManually)
                {
                    OnClockManually?.Invoke(this, null);
                }
            }

            OnUpdatingUser?.Invoke(this, currentUser);
        }
    }
}
