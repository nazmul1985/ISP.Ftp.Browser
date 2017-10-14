using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ftp.Browser
{
    class Program
    {
        static void Main(string[] args)
        {
            var url = "http://www.dreamnetbd.com/link/index9.html";
            var htmlParser = new HtmlParser();
            htmlParser.ParseHtml(url);

        }
    }
}
