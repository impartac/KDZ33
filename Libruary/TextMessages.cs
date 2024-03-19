using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Library
{
    /// <summary>
    /// Класс всех строк вывода.
    /// </summary>
    internal class TextMessages
    {
        internal TextMessages() { }

        /// <summary>
        /// Шапка таблицы на английском языке.
        /// </summary>
        public const string _headerEng = "\"ID\";" +
                           "\"SculpName\";" +
                           "\"Photo\";" +
                           "\"Author\";" +
                           "\"ManufactYear\";" +
                           "\"Material\";" +
                           "\"Description\";" +
                           "\"LocationPlace\";" +
                           "\"Longitude_WGS84\";" +
                           "\"Latitude_WGS84\";" +
                           "\"global_id\";" +
                           "\"geodata_center\";" +
                           "\"geoarea\";";
        /// <summary>
        /// Шапка таблицы на русском языке.
        /// </summary>
        public const string _headerRus = "\"Код\";" +
                            "\"Наименование скульптуры\";" +
                            "\"Фотография\";" +
                            "\"Автор\";" +
                            "\"Год изготовления\";" +
                            "\"Материал изготовления\";" +
                            "\"Описание\";" +
                            "\"Месторасположение\";" +
                            "\"Долгота в WGS-84\";" +
                            "\"Широта в WGS-84\";" +
                            "\"global_id\";" +
                            "\"geodata_center\";" +
                            "\"geoarea\";";

        // Строки, которые будут использоваться в Inline меню.
        internal const string menuJson = "JSON file";
        internal const string menuCsv = "CSV file";
        internal const string menuSample = "Sample";
        internal const string menuSort = "Sort";
        internal const string menuSculpName = "SculpName";
        internal const string menuLocationPlace = "LocationPlace";
        internal const string menuManufactYear = "ManufactYear";
        internal const string menuMaterial = "Material";
        internal const string menuManufactAndMaterial = "ManufactYear &\n Material";
        internal const string menuSortSculpName = "SculpName by alphabet";
        internal const string menuSortManufactYear = "ManufactYear descending";

        // Строки диалога с пользователем.
        internal const string chooseFileType = "Choose type of file";
        internal const string end = "See you later... goodbuy";
        internal const string errorMessage = "I don't know this command";
        internal const string requestJson = "Send me .Json file please";
        internal const string requestCsv = "Send me .Csv file please";
        internal const string tryAgain = "Try again please";
        internal const string accepted = "File kept on";
        internal const string badFile = "Bad file. Try again";
        internal const string chooseAction = "Choose action please";
        internal const string chooseFiedSample = "Choose field for sample please";
        internal const string chooseFieldSort = "Choose field for sorting please";
        internal const string actionDone = "Action done";
        internal const string actionFailed = "Action failed";
        internal const string requestValueOfField = "Write value of this field please";
        internal const string askRepeat = "Write \"/start\" to work again";

        // Команды бота, посылаемые inline кнопками.
        internal const string start = "/start";
        internal const string stop = "/stop";
        internal const string injson = "/injson";
        internal const string incsv = "/incsv";
        internal const string sample = "/sample";
        internal const string sort = "/sort";
        internal const string sampleSculpName = "/sampleSculpName";
        internal const string sampleLocationPlace = "/sampleLocationPlace";
        internal const string sampleManufactYear = "/sampleManufactYear";
        internal const string sampleMaterial = "/sampleMaterial";
        internal const string sampleManufactAndMaterial = "/sampleManufactAndMaterial";
        internal const string sortSculpName = "/sortSculpName";
        internal const string sortManufactYear = "/sortManufactYear";
        internal const string outjson = "/outjson";
        internal const string outcsv = "/outcsv";

        // Названия классов взаимодействия в типами файлов.
        internal const string nameCsvClass = "CsvProcessing";
        internal const string nameJsonClass = "JsonProcessing";
        internal const string nameProcessing = "processing";

        /// <summary>
        /// Inline меню выборки .
        /// </summary>
        internal InlineKeyboardMarkup keyboardSample = new(new[]
                                    {
                                        InlineKeyboardButton.WithCallbackData(
                                        text:TextMessages.menuSculpName,
                                        callbackData:TextMessages.sampleSculpName
                                        ),
                                        InlineKeyboardButton.WithCallbackData(
                                        text:TextMessages.menuLocationPlace,
                                        callbackData:TextMessages.sampleLocationPlace
                                        ),
                                        InlineKeyboardButton.WithCallbackData(
                                        text:TextMessages.menuManufactAndMaterial,
                                        callbackData:TextMessages.sampleManufactAndMaterial
                                        ),
                                        });
        /// <summary>
        /// Inline меню сортировки .
        /// </summary>
        internal InlineKeyboardMarkup keyboardSort = new(new[]
                                    {
                                        InlineKeyboardButton.WithCallbackData(
                                        text:TextMessages.menuSortSculpName,
                                        callbackData:TextMessages.sortSculpName
                                        ),
                                        InlineKeyboardButton.WithCallbackData(
                                        text:TextMessages.menuSortManufactYear,
                                        callbackData:TextMessages.sortManufactYear
                                        ),
                                    });
        /// <summary>
        /// Inline меню выбора действия .
        /// </summary>
        internal InlineKeyboardMarkup keyboardActionType = new(new[]
                                    {
                                        InlineKeyboardButton.WithCallbackData(
                                        text:TextMessages.menuSample,
                                        callbackData:TextMessages.sample
                                        ),
                                        InlineKeyboardButton.WithCallbackData(
                                        text:TextMessages.menuSort,
                                        callbackData:TextMessages.sort
                                        ),
                                    });
        /// <summary>
        /// Inline меню выбора типа выходного файла.
        /// </summary>
        internal InlineKeyboardMarkup keyboardOutMenu = new(new[]
                                    {
                                        InlineKeyboardButton.WithCallbackData(
                                        text:TextMessages.menuJson,
                                        callbackData:TextMessages.outjson
                                        ),
                                        InlineKeyboardButton.WithCallbackData(
                                        text:TextMessages.menuCsv,
                                        callbackData:TextMessages.outcsv
                                        ),
                                    });
        /// <summary>
        /// Inline меню выбора типа входного файла.
        /// </summary>
        internal InlineKeyboardMarkup keyboardInMenu = new(new[]
                                    {
                                        InlineKeyboardButton.WithCallbackData(
                                        text:TextMessages.menuJson,
                                        callbackData:TextMessages.injson
                                        ),
                                        InlineKeyboardButton.WithCallbackData(
                                        text:TextMessages.menuCsv,
                                        callbackData:TextMessages.incsv
                                        ),
                                    });

        /// <summary>
        /// Метод вывода информации об обновлении чата.
        /// </summary>
        /// <param name="update"></param>
        internal string UpdateHandle(Update update) 
        {
            return($"chat with " +
                $"{(update.Message is null ? update.MyChatMember : update.Message.Chat.FirstName)} ," +
                $" message type = {update.Type} ," +
                $" date = {DateTime.Now}");
        }
        internal string GetPath() 
        {
            return Directory.GetParent(Directory.GetParent(
                    Directory.GetParent(
                    Directory.GetParent(
                    Directory.GetCurrentDirectory()).FullName)
                    .FullName).FullName).FullName+"\\var\\var.txt";
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