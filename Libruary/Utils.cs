using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class Utils
    {
        public dynamic? Convert(string s,string type) 
        {
            switch (type) 
            {
                case ("int"): 
                    {
                        return (s != "null" && s!="")? int.Parse(s) : null;
                    }
                case ("double"): 
                    {
                        return (s != "null" && s != "") ? double.Parse(s) : null;
                    }
                case ("long"): 
                    {
                        return (s != "null" && s != "") ? long.Parse(s) : null;
                    }
                case ("string"): 
                    {
                        return (s != "null" && s != "") ? s : null;
                    }
                default: 
                    {
                        throw new Exception();
                    }
            }
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