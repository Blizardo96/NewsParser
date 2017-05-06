using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.IO;
using System.Globalization;

namespace NewsParser
{
    class OverclockersUA
    {
        #region CheckNeedUpdate
        /// <summary>
        /// Проверяет нужно ли выгружать данные с сайта заново. Тру - нужно, фолс - не нужно.
        /// </summary>
        /// <returns></returns>
        public static bool NeedUpdate(int mostNewOnSite)
        {
            DateTime localDate = DateTime.Now;
            string _date = null;
            _date += localDate.Date;
            string _onlyDate = null;
            for (int i = 0; i <= 10; ++i)
            {
                _onlyDate += _date[i];
            }
            string _directory = Directory.GetCurrentDirectory() + @"\" + _onlyDate;
            if (!Directory.Exists(_directory))
            {
                return true;
            }
            else
            {
                List<String> _files = new List<string>();
                foreach (var n in Directory.GetFiles(_directory, "*.txt"))
                {
                    _files.Add(n);
                }
                List<int> _news = new List<int>();
                foreach (string a in _files)
                {
                    string _stringForWork = null;
                    for (int i = a.Length - 10; i < a.Length - 4; i++)
                    {
                        _stringForWork += a[i];
                    }
                    _news.Add(Convert.ToInt32(_stringForWork));
                }

                int _mostNewLoaded = _news.Max();
                if (_mostNewLoaded >= mostNewOnSite)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            
        }
        #endregion

        #region StartParse Overclokers.ua
        /// <summary>
        /// Запускается сбор ссылок из раздела новости с передачей в метод GetSite
        /// </summary>
        public static void StartParse()
        {
            List<String> _allLinks = new List<String>();
            List<String> _links = new List<String>();
            HtmlAgilityPack.HtmlDocument doc = new HtmlDocument();

            var _web = new HtmlWeb { AutoDetectEncoding = true };
            doc = _web.Load("http://www.overclockers.ua/");
            
            foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//a[@href]"))
            {
                // Get the value of the HREF attribute
                string hrefValue = link.GetAttributeValue("href", string.Empty);
                _allLinks.Add(hrefValue);
            }

            for (int i = 0; i < _allLinks.Count; i++)
            {
                if (_allLinks[i].Contains("/news/") && _allLinks[i].Length > 32)
                {
                    _links.Add("http://www.overclockers.ua" + _allLinks[i]);
                }
            }

            List<int> _numberOfNews = new List<int>();
            for (int i = 0; i < _allLinks.Count; i++)
            {
                if (_allLinks[i].Contains("/news/") && _allLinks[i].Length > 32)
                {
                    string a = _allLinks[i];
                    string c = null;
                    for (int b = a.Length - 7; b < a.Length - 1; b++)
                    {
                        c += a[b];
                    }
                    _numberOfNews.Add(Convert.ToInt32(c));
                }
            }
            int mostNewNewsOnSite = _numberOfNews.Max();

            if (NeedUpdate(mostNewNewsOnSite))
            {
                foreach (string _link in _links)
                {
                    GetSite(_link);
                }
            }
            else
            {

            }
        }
        #endregion

        #region GetSite Overclockers.ua
        /// <summary>
        /// Функция принимает ссылку на статью и записывает в файл текст статьи, имя файла состоит из номера статьи.
        /// </summary>
        /// <param link="_html"></param>
        private static void GetSite(string _html)
        {
            List<string> _text = new List<string>();
            string _title = null;
            HtmlAgilityPack.HtmlDocument doc = new HtmlDocument();
            HtmlAgilityPack.HtmlDocument webTitle = new HtmlDocument();

            var _web = new HtmlWeb { AutoDetectEncoding = true };
            var _titleWeb = new HtmlWeb { AutoDetectEncoding = true };
            //суём в программу ссылку на пост
            doc = _web.Load(_html);
            webTitle = _titleWeb.Load(_html);
            //Выборка по id "дива"
            HtmlNodeCollection TitleNoAltElements = webTitle.DocumentNode.SelectNodes("//div[@id='post']/h1");
            if (TitleNoAltElements != null)
            {
                foreach (HtmlNode THN in TitleNoAltElements)
                {
                    //Получаем строчки
                    _text.Add("$$" + THN.InnerText + "$$");
                }
            }
            HtmlNodeCollection NoAltElements = doc.DocumentNode.SelectNodes("//div[@id='article']");

            if (NoAltElements != null)
            {
                foreach (HtmlNode HN in NoAltElements)
                {
                    //Получаем строчки
                    _text.Add(HN.InnerText);
                }
            }
            //string test = null;
            //test += String.Format(_text[0] + _text[1]);
            //Console.WriteLine(test);
            //Console.ReadKey();

            DateTime localDate = DateTime.Now;
            string _date = null;
            _date += localDate.Date;
            string _onlyDate = null;
            for(int i = 0; i <= 10; ++i)
            {
                _onlyDate += _date[i];
            }
            string _directory = Directory.GetCurrentDirectory() + @"\" + _onlyDate;
            Directory.CreateDirectory(_directory);
            int _num = _html.Length - 1;
            for (int i = _num - 6; i < _num; ++i)
            {
                _title += _html[i];
            }
            _title += ".txt";
            string path = _directory + @"\" + _title;
            File.WriteAllLines(path, _text);
        }
        #endregion
    }
}
