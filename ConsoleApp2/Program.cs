using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.IO;


namespace NewsParser
{
    class Program
    {
        static void Main(string[] args)
        {
            OverclockersUA a = new OverclockersUA();
            OverclockersUA.StartParse();
        }
        
    }
}
