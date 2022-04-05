using CefSharp;
using CefSharp.OffScreen;
using HanbiroExtensionGUI.Constants;
using HanbiroExtensionGUI.Models;
using HanbiroExtensionGUI.Services;
using HanbiroExtensionGUI.Services.JobSchedulerServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using File = System.IO.File;
using Message = Telegram.Bot.Types.Message;
using User = HanbiroExtensionGUI.Models.User;

namespace HanbiroExtensionGUI
{
    public class TelegramAppService : TelegramBotClient
    {
        #region Fields
        private UserSettings currentUserSettings = null;
        private string uesrSettingsPath = @"UserSettings.json";
        private JobSchedulerService jobScheduler;
        private CheckingPasswordService checkingPasswordService;
        private CheckHealthService healthService;
        private CheckInCheckOutService checkInCheckOutService;
        int countReadyToUse = 0;
        public event EventHandler OnReady;
        public readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        #endregion

        #region Properties
        public UserSettings CurrentUserSettings => currentUserSettings;
        public JobSchedulerService JobScheduler => jobScheduler;
        #endregion

        #region Constructors
        public TelegramAppService(string token,
            HttpClient httpClient = null,
            string baseUrl = null)
            : base(token, httpClient, baseUrl)
        {
            LoadUserSettings();
            InitCefSharp();
            
            checkingPasswordService = new CheckingPasswordService(currentUserSettings);
            healthService = new CheckHealthService(currentUserSettings);
            checkInCheckOutService = new CheckInCheckOutService(currentUserSettings);

            jobScheduler = new JobSchedulerService(currentUserSettings, checkInCheckOutService);

            checkingPasswordService.OnReadyToUse += CheckingPasswordService_OnReadyToUse;
            healthService.OnReadyToUse += CheckingPasswordService_OnReadyToUse;
            checkInCheckOutService.OnReadyToUse += CheckingPasswordService_OnReadyToUse;

            checkingPasswordService.OnSuccess += CheckingPasswordService_OnSuccess;
            checkingPasswordService.OnError += CheckingPasswordService_OnError;

            checkInCheckOutService.OnSuccess += CheckInCheckOutService_OnSuccess;
            checkInCheckOutService.OnError += CheckInCheckOutService_OnError;

            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { } // receive all update types
            };

            this.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken: cancellationTokenSource.Token);
        }

        ~TelegramAppService()
        {
            // Send cancellation request to stop bot
            cancellationTokenSource.Cancel();
        }

        #endregion

        #region Events

        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Only process Message updates: https://core.telegram.org/bots/api#message
            if (update.Type != UpdateType.Message)
                return;
            // Only process text messages
            if (update.Message!.Type != MessageType.Text)
                return;

            var chatId = update.Message.Chat.Id;
            var messageText = update.Message.Text;

            if (currentUserSettings.Users.Count(u => u.TelegramId == chatId) == 0)
            {
                currentUserSettings.Users.Add(new User
                {
                    TelegramId = chatId
                });
            }

            var currentUser = currentUserSettings.Users.FirstOrDefault(u => u.TelegramId == chatId);
            if (currentUser == null) return;

            if (messageText == TelegramCommands.StartCommand)
            {
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "Hello!!! Thank for using me.",
                    cancellationToken: cancellationToken);

                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("Please enter your username");
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: stringBuilder.ToString(),
                    cancellationToken: cancellationToken);

                currentUser.LastCommand = TelegramCommands.GetUsernameCommand;
            }
            else if(currentUser.LastCommand == TelegramCommands.GetUsernameCommand)
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

                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "Register Success!!!",
                    cancellationToken: cancellationToken);

                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "It will clock in at 6 AM and clock out at 6:15 PM every day.",
                    cancellationToken: cancellationToken);

                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "You will receive the photo of result.",
                    cancellationToken: cancellationToken);

                //var FileUrl = @"Images/sample.jpg";
                //using (var stream = System.IO.File.Open(FileUrl, FileMode.Open))
                //{
                //    await botClient.SendPhotoAsync(chatId,
                //    new InputOnlineFile(stream),
                //    "Like this !!!");
                //}

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

            SaveUserSettings();

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

        private void CheckingPasswordService_OnReadyToUse(object sender, EventArgs e)
        {
            countReadyToUse++;
            if (countReadyToUse == 3)
            {
                OnReady?.Invoke(this, new EventArgs());
            }
        }

        private void CheckInCheckOutService_OnError(object sender, BrowserEventArgs e)
        {
            var service = sender as CheckInCheckOutService;

            if (e.ErrorArgs is not null)
            {
                if (e.ErrorArgs.ErrorType == Enums.ErrorType.WrongUsernameOrPassword)
                {
                    SendErrorMessageToUser(service, "Your username/password is not valid. Please Logout and try again!!!");
                }
            }
            else
            {
                SendErrorMessageToUser(service, "Some things went wrong. Please Clock In/Clock Out manually!!!");
            }

            service.Browser.CurrentUser.IsActive = false;

            string fileName = string.Format("ERROR_{0}_{1}.txt", service.Browser.CurrentUser.UserName, Guid.NewGuid().ToString());
            System.IO.File.WriteAllText($"{Application.StartupPath}/Logs/{fileName}", service.Browser.CheckHealthResult.ToString());
        }

        private void CheckInCheckOutService_OnSuccess(object sender, BrowserEventArgs e)
        {
            var service = sender as CheckInCheckOutService;

            SendSuccessImage(service);

            string fileName = string.Format("SUCCESS_{0}_{1}.txt", service.Browser.CurrentUser.UserName, Guid.NewGuid().ToString());
            System.IO.File.WriteAllText($"{Application.StartupPath}/Logs/{fileName}", service.Browser.CheckHealthResult.ToString());
        }

        private void CheckingPasswordService_OnError(object sender, EventArgs e)
        {
            MessageBox.Show("Login Fail.");
        }

        private void CheckingPasswordService_OnSuccess(object sender, EventArgs e)
        {
            MessageBox.Show("Login Success.");
        }

        #endregion

        #region Methods

        private void InitCefSharp()
        {
            var settings = new CefSettings();
            settings.DisableGpuAcceleration();
            settings.SetOffScreenRenderingBestPerformanceArgs();
            Cef.Initialize(settings);
        }

        public void LoadUserSettings()
        {
            if (File.Exists(uesrSettingsPath))
            {
                var json = File.ReadAllText(uesrSettingsPath);
                currentUserSettings = JsonSerializer.Deserialize<Models.UserSettings>(json);
            }
        }
        public void SaveUserSettings(TimeWork timeWork = null)
        {
            if (currentUserSettings is null)
            {
                currentUserSettings = new UserSettings();
            }
            if(timeWork is not null)
            {
                currentUserSettings.TimeWork = timeWork;
            }
            
            checkInCheckOutService.CurrentUserSettings = currentUserSettings;
            healthService.CurrentUserSettings = currentUserSettings;
            checkingPasswordService.CurrentUserSettings = currentUserSettings;
            jobScheduler.CurrentUserSettings = currentUserSettings;
            string json = JsonSerializer.Serialize(currentUserSettings);
            File.WriteAllText(uesrSettingsPath, json);
        }

        public async void CheckPassword(User user)
        {
            await checkingPasswordService.DoWorkAsync(user);
        }

        public async void AddNewUser(User user)
        {
            currentUserSettings.Users.Add(user);
            SaveUserSettings();
        }


        private async void SendErrorMessageToUser(CheckInCheckOutService checkInCheckOutService, 
            string messgae)
        {
            await this.SendTextMessageAsync(
                    chatId: checkInCheckOutService.Browser.CurrentUser.TelegramId,
                    text: messgae,
                    cancellationToken: cancellationTokenSource.Token);
        }

        private async void SendSuccessImage(CheckInCheckOutService checkInCheckOutService)
        {
            var bytes = await checkInCheckOutService.Browser.CaptureScreenshotAsync();
            Stream stream = new MemoryStream(bytes);
            await this.SendPhotoAsync(checkInCheckOutService.Browser.CurrentUser.TelegramId,
                    new InputOnlineFile(stream),
                    "Raise on : " + DateTime.Now.ToString());
        }

        #endregion
    }
}
