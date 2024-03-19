using CsvHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;
using CsvHelper.Configuration;

namespace Library
{
    /// <summary>
    /// Класс для считывания из сsv формата,
    /// а также вывода в csv формат.
    /// </summary>
    public class CsvProcessing: TypeProcessing
    {
        public CsvProcessing() :base() { }
        public override StreamWriter Write(StreamWriter sw,List<Monument> data)
        {
            sw.WriteLine(TextMessages._headerEng);
            sw.WriteLine(TextMessages._headerRus);
            for (int i = 0; i < data.Count; i++) 
            {
                sw.WriteLine(data[i].ToCsv());
            }
            
            return sw;
        }
        public override List<Monument> Read(StreamReader sr)
        {
            List<Monument> data =  new List<Monument>();
            DataProcessing dp = new DataProcessing();
            try 
            {
                data = dp.FiniteFromCsv(sr);
            } 
            catch (Exception)
            {
                throw new FileLoadException();
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