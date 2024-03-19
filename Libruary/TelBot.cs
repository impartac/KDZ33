using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Microsoft.Extensions.Logging;
using CsvHelper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace Library
{
    public class TelBot
    {
        const string _token = "6606151629:AAFcLaqwft3HE2XSsqJm33fXp_Zbv2vxhAA";

        string destinationFilePath;

        TelegramBotClient _bot = new TelegramBotClient(_token);

        CancellationTokenSource cts = new CancellationTokenSource();

        ReceiverOptions receiverOptions = new ReceiverOptions()
        {
            AllowedUpdates = Array.Empty<UpdateType>()
        };
        CsvProcessing csvProc = new CsvProcessing();
        JsonProcessing jsonProc = new JsonProcessing();
        DataProcessing dataProc = new DataProcessing();
        TextMessages textMessages = new TextMessages();
        internal enum StateOfDialog
        {
            Start,
            InputFile,
            GetFileCsv,
            GetFileJson,
            ChooseAction,
            Action,
            Sample,
            SampleSculp,
            SampleLocation,
            SampleManufact,
            SampleMaterial,
            SampleManufactAndMaterial,
            SampleAction,
            Sort,
            SortSculp,
            SortManufact,
            OutputFile,
            End
        }
        Dictionary<ChatId, StateOfDialog> personalTrack = new Dictionary<ChatId, StateOfDialog>();
        Dictionary<ChatId, List<Monument?>?> usersData = new Dictionary<ChatId, List<Monument?>?>();
        Dictionary<ChatId, List<Monument?>?> usersNewData = new Dictionary<ChatId, List<Monument?>?>();
        DecompositionBot dec = new DecompositionBot();
        StreamWriter loggerFile;

        public TelBot() { }

        public async Task Start()
        {

            
            using CancellationTokenSource cts = new();

            ReceiverOptions receiverOptions = new()
            {
                AllowedUpdates = Array.Empty<UpdateType>()
            };
            _bot.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token
            );

            User me = await _bot.GetMeAsync();

            Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();
            cts.Cancel();

        }
        
        
        async Task HandleUpdateAsync(ITelegramBotClient botClient,
                                    Update update,
                                    CancellationToken cancellationToken)
        {
            using StreamWriter loggerFile = new StreamWriter(textMessages.GetPath(),append:true);
            loggerFile.WriteLine(textMessages.UpdateHandle(update));
            loggerFile.Dispose();
            ChatId chatId= null;
            // В зависимости от типа обновления, определяют id чата.
            switch (update.Type)
            {
                case UpdateType.CallbackQuery:
                    {
                        chatId = update.CallbackQuery.From.Id;
                        break;
                    }
                case UpdateType.Message:
                    {
                        chatId = update.Message.Chat.Id;
                        break;
                    }
                default:
                    {
                        return;
                    }
            }
            dec.CreateChat(chatId, personalTrack, usersData, usersNewData);
            switch (personalTrack[chatId])
            {
                case StateOfDialog.Start:
                    {
                        await dec.StartMenu(botClient,
                                update,
                                cancellationToken,
                                chatId,
                                personalTrack,
                                textMessages);
                        return;
                    }
                case StateOfDialog.InputFile:
                    {
                        await dec.InputFileMenu(botClient,
                                update,
                                cancellationToken,
                                chatId,
                                personalTrack);
                        return;
                    }
                case StateOfDialog.GetFileCsv:
                    {
                        await dec.GetFileTyped(botClient,
                                update,
                                cancellationToken,
                                chatId,
                                personalTrack,
                                usersData,
                                textMessages,
                                csvProc);
                        return;
                    }
                case StateOfDialog.GetFileJson:
                    {
                        await dec.GetFileTyped(botClient,
                                update,
                                cancellationToken,
                                chatId,
                                personalTrack,
                                usersData,
                                textMessages,
                                jsonProc);
                        return;
                    }
                case StateOfDialog.ChooseAction:
                    {
                        await dec.ChooseAction(botClient,
                                update,
                                cancellationToken,
                                chatId,
                                personalTrack,
                                textMessages);
                        return;
                    }
                case StateOfDialog.Action:
                    {
                        await dec.Action(botClient,
                                update,
                                cancellationToken,
                                chatId,
                                personalTrack,
                                usersData,
                                usersNewData,
                                dataProc,
                                textMessages);
                        return;
                    }
                case (StateOfDialog.SampleMaterial):
                    {
                        usersNewData[chatId] = dataProc.Sample(usersData[chatId],
                                            TextMessages.menuMaterial,
                                            update.Message.Text);
                        await botClient.SendTextMessageAsync(
                                            chatId: chatId,
                                            text: TextMessages.requestValueOfField,
                                            replyMarkup: textMessages.keyboardOutMenu,
                                            cancellationToken: cancellationToken);
                        personalTrack[chatId] = StateOfDialog.OutputFile;
                        return;
                    }
                case (StateOfDialog.SampleManufactAndMaterial): 
                {
                        await botClient.SendTextMessageAsync(
                                            chatId: chatId,
                                            text: TextMessages.requestValueOfField,
                                            cancellationToken: cancellationToken);
                        usersData[chatId] = dataProc.Sample(usersData[chatId],
                                            TextMessages.menuManufactYear,
                                            update.Message.Text);
                        personalTrack[chatId] = StateOfDialog.SampleMaterial;
                        break;
                }
                case (StateOfDialog.SampleSculp):
                    {
                        usersNewData[chatId] = dataProc.Sample(usersData[chatId],
                                            TextMessages.menuSculpName,
                                            update.Message.Text);
                        await botClient.SendTextMessageAsync(
                                            chatId: chatId,
                                            text: TextMessages.chooseFileType,
                                            replyMarkup: textMessages.keyboardOutMenu,
                                            cancellationToken: cancellationToken);
                        personalTrack[chatId] = StateOfDialog.OutputFile;
                        return;
                    }
                case (StateOfDialog.SampleManufact):
                    {
                        usersNewData[chatId] = dataProc.Sample(usersData[chatId],
                                            TextMessages.menuManufactYear,
                                            update.Message.Text);
                        await botClient.SendTextMessageAsync(
                                            chatId: chatId,
                                            text: TextMessages.requestValueOfField,
                                            cancellationToken: cancellationToken);
                        personalTrack[chatId] = StateOfDialog.SampleMaterial;
                        return;
                    }
                case (StateOfDialog.SampleLocation):
                    {
                        usersNewData[chatId] = dataProc.Sample(usersData[chatId],
                                            TextMessages.menuLocationPlace,
                                            update.Message.Text);
                        await botClient.SendTextMessageAsync(
                                            chatId: chatId,
                                            text: TextMessages.chooseFileType,
                                            replyMarkup: textMessages.keyboardOutMenu,
                                            cancellationToken: cancellationToken);
                        personalTrack[chatId] = StateOfDialog.OutputFile;
                        return;
                    }
                case (StateOfDialog.OutputFile):
                    {
                        await dec.Output(botClient,
                                        update,
                                        cancellationToken,
                                        chatId,
                                        personalTrack,
                                        usersData,
                                        usersNewData,
                                        jsonProc,
                                        csvProc);
                        return;
                    }
            }
        }
        Task HandlePollingErrorAsync(ITelegramBotClient botClient,
                                    Exception exception,
                                    CancellationToken cancellationToken)
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
/*
░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░
░░░░ЗАПУСКАЕМ░ГУСЕЙ-РАЗВЕДЧИКОВ░░░░
░░░░░▄▀▀▀▄░░░▄▀▀▀▀▄░░░▄▀▀▀▄░░░░░
▄███▀░◐░░░▌░▐0░░░░0▌░▐░░░◐░▀███▄
░░░░▌░░░░░▐░▌░▐▀▀▌░▐░▌░░░░░▐░░░░
░░░░▐░░░░░▐░▌░▌▒▒▐░▐░▌░░░░░▌░░░░
*/