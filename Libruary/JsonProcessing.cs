using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace Library
{
    /// <summary>
    /// Класс для считывания из json формата,
    /// а также вывода в json формат.
    /// </summary>
    public class JsonProcessing : TypeProcessing
    {
        public JsonProcessing() : base() { }

        public override StreamWriter Write(StreamWriter sw,List<Monument> data) 
        {
            {
                sw.WriteLine(JsonSerializer.Serialize(data, new JsonSerializerOptions 
                    { 
                        WriteIndented = true,
                        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
                    }
                ));
            }
            return sw;
        }


        public override List<Monument?>? Read(StreamReader sr)
        {
            List<Monument> data = new List<Monument>();
            try
            {
                string s = sr.ReadToEnd();
                data = JsonSerializer.Deserialize<List<Monument?>?>(s);
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