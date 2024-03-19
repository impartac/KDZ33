using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    /// <summary>
    /// Класс взаимодействия с базой данных.
    /// </summary>
    public class DataProcessing
    {
        /// <summary>
        /// Состояние считываняи файла для конечного автомата.
        /// </summary>
        enum State
        {
            Start,
            InField,
            InQuotedField,
            EndField,
            EndRecord,
            EndOfFile
        }
        enum Symbols
        {
            Quote = '"',
            Separator = ';',
            Endl = '\n'
        }
        public DataProcessing() { }

        /// <summary>
        /// Вывод базы данных в формате текстовом формате.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string ToCsv(List<Monument?>? data)
        {
            string answer = TextMessages._headerEng + Symbols.Endl +
                            TextMessages._headerRus + Symbols.Endl;
            foreach (Monument monument in data)
            {
                answer += monument.ToString();
            }
            return answer;
        }
        /// <summary>
        /// Сортировка по определенному полю.
        /// </summary>
        /// <param name="records"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public List<Monument?>? Sort(List<Monument?>? records, string field)
        {
            return field switch
            {
                "SculpName" => records.OrderBy(x => x.SculpName).ToList(),
                "ManufactYear" => records?.OrderBy(x => -x?.ManufactYear).ToList(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        /// <summary>
        /// Выборка по определенному полю.
        /// </summary>
        /// <param name="records"></param>
        /// <param name="field"></param>
        /// <param name="sample"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public List<Monument?>? Sample(List<Monument?>? records, string field, string sample)
        {
            return field switch
            {
                "SculpName" => records?.Where(x => x?.SculpName == sample).ToList(),
                "LocationPlace" => records?.Where(x => x?.LocationPlace == sample).ToList(),
                "ManufactYear" => records?.Where(x => x?.ManufactYear.ToString() == sample).ToList(),
                "Material" => records?.Where(x => x?.Material == sample).ToList(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        public List<Monument?>? Sample2Fields(List<Monument?>? records, 
                                            string sampleManufact, 
                                            string sampleMaterial)
        {
            return records?.Where(
                x=>x?.ManufactYear.ToString()==sampleManufact && 
                x.Material==sampleMaterial).ToList();
        }
        /// <summary>
        /// Конечный автомат считывания из StreamReader'a.
        /// </summary>
        /// <param name="sr"></param>
        /// <returns></returns>
        public List<Monument?>? FiniteFromCsv(StreamReader sr) 
        {

            State currentState = State.Start;
            string temp = "";
            sr.ReadLine();
            sr.ReadLine();
            List<Monument> data = new List<Monument>();
            List<string> fields = new List<string>();
            while (currentState != State.EndOfFile)
            {
                char c = (char)sr.Read();
                switch (currentState)
                {
                    case State.Start:
                    { 
                        switch (c)
                        {
                            case (char)Symbols.Quote:
                                {
                                    currentState = State.InQuotedField;
                                    break;
                                }
                            case (char)Symbols.Separator:
                                {
                                    currentState = State.EndField;
                                    break;
                                }
                            case (char)Symbols.Endl:
                                {
                                    currentState = State.EndRecord;
                                    break;
                                }
                            default:
                                {
                                    if (sr.EndOfStream)
                                    {
                                        currentState = State.EndOfFile;
                                    }
                                    else
                                    {
                                        temp += c;
                                        currentState = State.InField;
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                    case State.InField:
                        {
                            switch (c)
                            {
                                case (char)Symbols.Separator:
                                    {
                                        // Т.к. temp может быть пустым, делаю его проверку.
                                        // В temp всегда хранится последняя кавычка, поэтому ее убираю.
                                        fields.Add(temp.Length>0?temp[..^1]:"");
                                        temp = "";
                                        currentState = State.Start;
                                        break;
                                    }
                                case (char)Symbols.Endl:
                                    {
                                        currentState = State.EndRecord;
                                        break;
                                    }
                                default:
                                    {
                                        if (sr.EndOfStream)
                                        {
                                            currentState = State.EndOfFile;
                                        }
                                        else 
                                        {
                                            temp += c;
                                        }
                                        break;
                                    }
                            }
                            break;
                        }
                    case State.InQuotedField:
                        {
                            if (c == (char)Symbols.Quote)
                            {
                                currentState = State.InField;
                                temp += c;
                            }
                            else 
                            {
                                temp += c;
                            }
                            break;
                        }
                    case State.EndField:
                        {
                            currentState = State.Start;
                            break;
                        }

                    case State.EndRecord:
                        {
                            data.Add(new Monument(fields.ToArray()));
                            fields = new List<string>();
                            currentState = State.Start;
                            break;
                        }
                    case State.EndOfFile:
                        break;
                }

            }
            return data;
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