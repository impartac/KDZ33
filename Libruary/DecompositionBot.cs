using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using static Library.TelBot;

namespace Library
{
    /// <summary>
    /// Класс декомпозиции работы тг-бота.
    /// </summary>
    internal class DecompositionBot
    {
        internal DecompositionBot() { }

        /// <summary>
        /// Метод, отправляющий пользователю сообщение об ошибке.
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="chatId"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="replyToMessageId"></param>
        /// <returns></returns>
        private async Task SendMessageError(ITelegramBotClient botClient,
                            ChatId chatId,
                            CancellationToken cancellationToken,
                            int? replyToMessageId) 
        {
            await botClient.SendTextMessageAsync(
                                        chatId: chatId,
                                        text: TextMessages.errorMessage,
                                        replyToMessageId: replyToMessageId,
                                        cancellationToken: cancellationToken);
            return;
        }
        /// <summary>
        /// Метод, отправляющий пользователю сообщение о приостановлении работы бота.
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="chatId"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="replyToMessageId"></param>
        /// <returns></returns>
        private async Task SendStopMessage(ITelegramBotClient botClient,
                            ChatId chatId,
                            CancellationToken cancellationToken,
                            int? replyToMessageId)
        {
            await botClient.SendTextMessageAsync(
                                        chatId: chatId,
                                        text: TextMessages.end,
                                        replyToMessageId: replyToMessageId,
                                        cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Метод, спрашивающий у пользователя тип входного файла.
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="chatId"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="replyToMessageId"></param>
        /// <param name="textMessages"></param>
        /// <returns></returns>
        private async Task MenuInputfile(ITelegramBotClient botClient,
                            ChatId chatId,
                            CancellationToken cancellationToken,
                            int? replyToMessageId,
                            TextMessages textMessages)
        {
            
            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: TextMessages.chooseFileType,
                replyToMessageId: replyToMessageId,
                replyMarkup: textMessages.keyboardInMenu,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Действия во время стартового состояния диалога.
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="update"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="chatId"></param>
        /// <param name="personalTrack"></param>
        /// <param name="textMessages"></param>
        /// <returns></returns>
        internal async Task StartMenu(ITelegramBotClient botClient,
                             Update update,
                             CancellationToken cancellationToken,
                             ChatId chatId,
                             Dictionary<ChatId,StateOfDialog>personalTrack,
                             TextMessages textMessages) 
        {
            if (update.Message is null) return;
            switch (update.Message.Text)
            {
                case (TextMessages.start):
                    {
                        await MenuInputfile(botClient,
                                        chatId,
                                        cancellationToken,
                                        update.Message.MessageId,
                                        textMessages);
                        personalTrack[chatId] = StateOfDialog.InputFile;
                        return;
                    }
                case (TextMessages.stop):
                    {
                        await SendStopMessage(botClient,
                                            chatId,
                                            cancellationToken,
                                            update.Message.MessageId);
                        personalTrack[chatId] = StateOfDialog.Start;
                        return;
                    }
                default:
                    {
                        await SendMessageError(botClient,
                                                   chatId,
                                                   cancellationToken,
                                                   update.Message.MessageId);
                        return;
                    }
            }
        }
        /// <summary>
        /// Действие во время введения файла.
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="update"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="chatId"></param>
        /// <param name="personalTrack"></param>
        /// <returns></returns>
        internal async Task InputFileMenu(ITelegramBotClient botClient,
                             Update update,
                             CancellationToken cancellationToken,
                             ChatId chatId,
                             Dictionary<ChatId, StateOfDialog> personalTrack) 
        {
            if (update.Type != UpdateType.CallbackQuery)
            {
                await SendMessageError(botClient,
                                    chatId,
                                    cancellationToken,
                                    update.Message.MessageId);
                return;
            }
            var callbackQuery = update.CallbackQuery;
            if (callbackQuery != null && callbackQuery.Message != null)
            {
                switch (callbackQuery.Data)
                {
                    case (TextMessages.injson):
                        {
                            await botClient.SendTextMessageAsync(
                                            chatId: chatId,
                                            text: TextMessages.requestJson,
                                            cancellationToken: cancellationToken);
                            personalTrack[chatId] = StateOfDialog.GetFileJson;
                            return;
                        }
                    case (TextMessages.incsv):
                        {
                            await botClient.SendTextMessageAsync(
                                        chatId: chatId,
                                        text: TextMessages.requestCsv,
                                        cancellationToken: cancellationToken);
                            personalTrack[chatId] = StateOfDialog.GetFileCsv;
                            return;
                        }
                    default:
                        {
                            await botClient.SendTextMessageAsync(
                                        chatId: chatId,
                                        text: TextMessages.tryAgain,
                                        cancellationToken: cancellationToken);
                            return;
                        }
                }
            }
        }
        /// <summary>
        /// Задача получения файла определенного типа.
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="update"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="chatId"></param>
        /// <param name="personalTrack"></param>
        /// <param name="usersData"></param>
        /// <param name="textMessages"></param>
        /// <param name="typeProc"></param>
        /// <returns></returns>
        private async Task GetFile(ITelegramBotClient botClient,
                             Update update,
                             CancellationToken cancellationToken,
                             ChatId chatId,
                             Dictionary<ChatId, StateOfDialog> personalTrack,
                             Dictionary<ChatId, List<Monument?>?> usersData,
                             TextMessages textMessages,
                             TypeProcessing typeProc)
        {
            string destinationFilePath = Directory.GetCurrentDirectory() + $"/downloaded{chatId}.file";
            string fileId = update.Message.Document.FileId;
            var fileInfo = await botClient.GetFileAsync(fileId);
            var filePath = fileInfo.FilePath;
            await using Stream fileStream = System.IO.File.Create(destinationFilePath);
            await botClient.DownloadFileAsync(
                filePath: filePath,
                destination: fileStream,
                cancellationToken: cancellationToken);
            fileStream.Dispose();
            StreamReader fs = new StreamReader(destinationFilePath);
            try
            {
                usersData[chatId] = typeProc.Read(fs);
                await botClient.SendTextMessageAsync(
                                        chatId: chatId,
                                        text: TextMessages.accepted,
                                        replyMarkup: textMessages.keyboardActionType,
                                        cancellationToken: cancellationToken);
                personalTrack[chatId] = StateOfDialog.ChooseAction;
                fs.Dispose();
                fileStream.Dispose();
            }
            catch (Exception ex)
            {
                fs.Dispose();
                await botClient.SendTextMessageAsync(
                                        chatId: chatId,
                                        text: TextMessages.badFile,
                                        cancellationToken: cancellationToken);
            }
        }
        internal async Task GetFileTyped(ITelegramBotClient botClient,
                             Update update,
                             CancellationToken cancellationToken,
                             ChatId chatId,
                             Dictionary<ChatId, StateOfDialog> personalTrack,
                             Dictionary<ChatId,List<Monument?>?> usersData,
                             TextMessages textMessages,
                             TypeProcessing proc)
        {
            if (update.Message.Document is null)
            {
                if (proc.GetType().Name == TextMessages.nameCsvClass)
                {
                    await botClient.SendTextMessageAsync(
                                            chatId: chatId,
                                            text: TextMessages.requestCsv,
                                            cancellationToken: cancellationToken);
                    personalTrack[chatId] = StateOfDialog.GetFileCsv;
                }
                else if (proc.GetType().Name == TextMessages.nameJsonClass)
                {
                    await botClient.SendTextMessageAsync(
                                           chatId: chatId,
                                           text: TextMessages.requestJson,
                                           cancellationToken: cancellationToken);
                    personalTrack[chatId] = StateOfDialog.GetFileJson;
                }
                return;
            }
            await GetFile(botClient,  
                    update,
                    cancellationToken,
                    chatId,
                    personalTrack,
                    usersData,
                    textMessages,
                    proc);
        }
        /// <summary>
        /// Метод создания и проверки на существования данных о чате.ы
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="personalTrack"></param>
        /// <param name="usersData"></param>
        /// <param name="usersNewData"></param>
        internal void CreateChat(ChatId chatId,
                            Dictionary<ChatId,StateOfDialog> personalTrack,
                            Dictionary<ChatId, List<Monument?>?> usersData,
                            Dictionary<ChatId, List<Monument?>?> usersNewData) 
        {
            if (!personalTrack.TryGetValue(chatId, out _))
            {
                personalTrack.Add(chatId, StateOfDialog.Start);
                usersData.Add(chatId, new List<Monument?>());
                usersNewData.Add(chatId, new List<Monument?>());
            }
        }
        /// <summary>
        /// Дейсвтие во время выбора пользователем операции над базой данных.
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="update"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="chatId"></param>
        /// <param name="personalTrack"></param>
        /// <param name="usersData"></param>
        /// <param name="textMessages"></param>
        /// <returns></returns>
        internal async Task ChooseAction(ITelegramBotClient botClient,
                             Update update,
                             CancellationToken cancellationToken,
                             ChatId chatId,
                             Dictionary<ChatId, StateOfDialog> personalTrack,
                             TextMessages textMessages)
        {
            if (update.Type != UpdateType.CallbackQuery)
            {
                await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: TextMessages.errorMessage,
                            replyToMessageId: update.Message.MessageId,
                            cancellationToken: cancellationToken);
                return;
            }
            var callbackQuery = update.CallbackQuery;
            if (callbackQuery != null && callbackQuery.Message != null)
            {
                switch (callbackQuery.Data)
                {
                    case (TextMessages.sample):
                        {
                            await botClient.SendTextMessageAsync(
                                                        chatId: chatId,
                                                        replyMarkup: textMessages.keyboardSample,
                                                        text: TextMessages.chooseFiedSample,
                                                        cancellationToken: cancellationToken);
                            personalTrack[chatId] = StateOfDialog.Action;
                            return;
                        }
                    case (TextMessages.sort):
                        {
                            await botClient.SendTextMessageAsync(
                                                        chatId: chatId,
                                                        replyMarkup: textMessages.keyboardSort,
                                                        text: TextMessages.chooseFieldSort,
                                                        cancellationToken: cancellationToken);
                            personalTrack[chatId] = StateOfDialog.Action;
                            return;
                        }
                    default:
                        {

                            await botClient.SendTextMessageAsync(
                                        chatId: chatId,
                                        text: TextMessages.tryAgain,
                                        cancellationToken: cancellationToken);
                            return;
                        }
                }
            }
        }
        /// <summary>
        /// Исполнение дейтсвия над базой данных.
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="update"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="chatId"></param>
        /// <param name="personalTrack"></param>
        /// <param name="usersData"></param>
        /// <param name="usersNewData"></param>
        /// <param name="dataProc"></param>
        /// <param name="textMessages"></param>
        /// <returns></returns>
        internal async Task Action(ITelegramBotClient botClient,
                             Update update,
                             CancellationToken cancellationToken,
                             ChatId chatId,
                             Dictionary<ChatId, StateOfDialog> personalTrack,
                             Dictionary<ChatId, List<Monument?>?> usersData,
                             Dictionary<ChatId,List<Monument?>?> usersNewData,
                             DataProcessing dataProc,
                             TextMessages textMessages)
        {
            if (update.Type != UpdateType.CallbackQuery)
            {
                await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: TextMessages.errorMessage,
                            replyToMessageId: update.Message.MessageId,
                            cancellationToken: cancellationToken);
                return;
            }
            var callbackQuery = update.CallbackQuery;
            if (callbackQuery != null && callbackQuery.Message != null)
            {
                switch (callbackQuery.Data)
                {
                    case (TextMessages.sortSculpName):
                        {
                            try
                            {
                                usersNewData[chatId] = dataProc.Sort(
                                                        usersData[chatId],
                                                        TextMessages.menuSculpName
                                                        );
                                await botClient.SendTextMessageAsync(
                                    chatId: chatId,
                                    text: TextMessages.actionDone,
                                    replyMarkup: textMessages.keyboardOutMenu,
                                    cancellationToken: cancellationToken);

                            }
                            catch (Exception)
                            {
                                await botClient.SendTextMessageAsync(
                                    chatId: chatId,
                                    text: TextMessages.actionFailed,
                                    cancellationToken: cancellationToken);
                            }
                            personalTrack[chatId] = StateOfDialog.OutputFile;
                            return;
                        }
                    case (TextMessages.sortManufactYear):
                        {
                            try
                            {
                                usersNewData[chatId] = dataProc.Sort(
                                                        usersData[chatId],
                                                        TextMessages.menuManufactYear
                                                        );
                                await botClient.SendTextMessageAsync(
                                    chatId: chatId,
                                    text: TextMessages.actionDone,
                                    replyMarkup: textMessages.keyboardOutMenu,
                                    cancellationToken: cancellationToken);
                            }
                            catch (Exception)
                            {
                                await botClient.SendTextMessageAsync(
                                    chatId: chatId,
                                    text: TextMessages.actionFailed,
                                    cancellationToken: cancellationToken);
                            }
                            personalTrack[chatId] = StateOfDialog.OutputFile;
                            return;
                        }
                    case (TextMessages.sampleSculpName):
                        {
                            await botClient.SendTextMessageAsync(
                                    chatId: chatId,
                                    text: TextMessages.requestValueOfField,
                                    cancellationToken: cancellationToken);
                            personalTrack[chatId] = StateOfDialog.SampleSculp;
                            return;
                        }
                    case (TextMessages.sampleLocationPlace):
                        {
                            await botClient.SendTextMessageAsync(
                                    chatId: chatId,
                                    text: TextMessages.requestValueOfField,
                                    cancellationToken: cancellationToken);
                            personalTrack[chatId] = StateOfDialog.SampleLocation;
                            return;
                        }
                    case (TextMessages.sampleManufactAndMaterial):
                        {
                            await botClient.SendTextMessageAsync(
                                    chatId: chatId,
                                    text: TextMessages.requestValueOfField,
                                    cancellationToken: cancellationToken);
                            personalTrack[chatId] = StateOfDialog.SampleManufactAndMaterial;
                            return;
                        }
                    case (TextMessages.sampleManufactYear):
                        {
                            await botClient.SendTextMessageAsync(
                                    chatId: chatId,
                                    text: TextMessages.requestValueOfField,
                                    cancellationToken: cancellationToken);
                            personalTrack[chatId] = StateOfDialog.SampleManufact;
                            return;
                        }
                    case (TextMessages.sampleMaterial):
                        {
                            await botClient.SendTextMessageAsync(
                                    chatId: chatId,
                                    text: TextMessages.requestValueOfField,
                                    cancellationToken: cancellationToken);
                            personalTrack[chatId] = StateOfDialog.SampleMaterial;
                            return;
                        }
                }
            }
            return;
        }

        /// <summary>
        /// Возвращение измененнного файла определенного типа пользователю.
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="chatId"></param>
        /// <param name="personalTrack"></param>
        /// <param name="usersData"></param>
        /// <param name="usersNewData"></param>
        /// <param name="typeProc"></param>
        /// <returns></returns>
        internal async Task OutputFile(ITelegramBotClient botClient,
                             ChatId chatId,
                             Dictionary<ChatId, StateOfDialog> personalTrack,
                             Dictionary<ChatId, List<Monument?>?> usersData,
                             Dictionary<ChatId,List<Monument?>?> usersNewData,
                             TypeProcessing typeProc)
        {
            StreamWriter temp = new StreamWriter(Directory.GetCurrentDirectory() + $"/{chatId}." +
                                    $"{typeProc.GetType().Name.ToLower().Replace(TextMessages.nameProcessing, "")}");
            typeProc.Write(temp, usersNewData[chatId]);
            temp.Dispose();
            await using Stream stream = System.IO.File.OpenRead(Directory.GetCurrentDirectory() + $"/{chatId}." +
                $"{typeProc.GetType().Name.ToLower().Replace(TextMessages.nameProcessing, "")}");
            Message message = await botClient.SendDocumentAsync(
                chatId: chatId,
                document: InputFile.FromStream(stream: stream, fileName: $"{chatId}." +
                $"{typeProc.GetType().Name.ToLower().Replace(TextMessages.nameProcessing, "")}")
                );
            usersNewData.Remove(chatId);
            usersData.Remove(chatId);
            personalTrack[chatId] = StateOfDialog.Start;
            stream.Dispose();
        }
        /// <summary>
        /// Отправка файла пользователю. Очистка памяти о диалоге.
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="update"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="chatId"></param>
        /// <param name="personalTrack"></param>
        /// <param name="usersData"></param>
        /// <param name="usersNewData"></param>
        /// <param name="jsonProc"></param>
        /// <param name="csvProc"></param>
        /// <returns></returns>
        internal async Task Output(ITelegramBotClient botClient,
                             Update update,
                             CancellationToken cancellationToken,
                             ChatId chatId,
                             Dictionary<ChatId, StateOfDialog> personalTrack,
                             Dictionary<ChatId, List<Monument?>?> usersData,
                             Dictionary<ChatId, List<Monument?>?> usersNewData,
                             JsonProcessing jsonProc,
                             CsvProcessing csvProc)
        {
            if (update.Type != UpdateType.CallbackQuery)
            {
                await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: TextMessages.errorMessage,
                            replyToMessageId: update.Message.MessageId,
                            cancellationToken: cancellationToken);
                return;
            }
            var callbackQuery = update.CallbackQuery;
            if (callbackQuery != null && callbackQuery.Message != null)
            {
                switch (callbackQuery.Data)
                {
                    case (TextMessages.outcsv):
                        {
                            await OutputFile(botClient,
                                                chatId,
                                                personalTrack,
                                                usersData,
                                                usersNewData,
                                                csvProc);
                            return;
                        }
                    case (TextMessages.outjson):
                        {
                            await OutputFile(botClient,
                                                chatId,
                                                personalTrack,
                                                usersData,
                                                usersNewData,
                                                jsonProc);
                            return;
                        }
                }
            }

            return;
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