using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace FantasyRadio.Utils
{
    class ArchiveParser
    {
        public static CookieContainer cookies = new CookieContainer();
        public static Dictionary<string, string> parameters = new Dictionary<string, string>();

        public static void ParseArchieve(string login, string password)
        {
            try
            {
                var handler = new HttpClientHandler { CookieContainer = cookies, AllowAutoRedirect = true };
                using (HttpClient http = new HttpClient())
                {
                    http.DefaultRequestHeaders.Add("User-Agent",
                                 "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:5.0) Gecko/20100101 Firefox/5.0");
                    Task<string> response = http.GetStringAsync("http://fantasyradio.ru/");
                    response.Wait();
                    // -------------------------------------
                    var document = new HtmlDocument();
                    document.LoadHtml(response.Result);
                    /*
                    Elements forms = document.getElementsByClass("form-inline");
                    Elements hiddens = forms.get(0).getElementsByAttributeValue("type", "hidden");
                    //--------
                    parameters.put("return", hiddens.get(2).attr("value"));
                    parameters.put(hiddens.get(3).attr("name"), "1");
                    parameters.put("username", login);
                    parameters.put("password", password);
                    parameters.put("task", "user.login");
                    parameters.put("option", "com_users");
                    //--------
                    res = Jsoup
                            .connect("http://fantasyradio.ru/index.php/vojti-na-sajt?task=user.login").userAgent("Mozilla/5.0 (Windows NT 6.1; WOW64; rv:5.0) Gecko/20100101 Firefox/5.0")
                            .timeout(45 * 1000).followRedirects(true).cookies(cookies)
                            .data(parameters)
                            .method(Method.POST)
                            .ignoreContentType(true).execute();
                    cookies.putAll(res.cookies());//!!!!!!!!!!!
                    if (!res.body().contains("Имя пользователя и пароль не совпадают или у вас еще нет учетной записи на сайте"))
                    { //Проверка на на правильность логина/пароля
                        doc = Jsoup.parse(Jsoup
                                .connect("http://fantasyradio.ru/index.php/component/content/article/2-uncategorised/14-stranitsa-2").cookies(cookies).userAgent("Mozilla/5.0 (Windows NT 6.1; WOW64; rv:5.0) Gecko/20100101 Firefox/5.0")
                                .timeout(45 * 1000).followRedirects(true)
                                .ignoreContentType(true).get().toString());
                        Elements mp3Elems = doc.getElementsByAttributeValue("name", "FlashVars");
                        Log.v("count flash vars", String.valueOf(mp3Elems.size()));//8 - норма
                        Elements tableElems = doc.getElementsByTag("table");
                        Log.v("count table elems", String.valueOf(tableElems.size()));//
                        Elements trElems = new Elements();
                        for (Element element : tableElems)
                        {
                            if (element.attributes().get("style")
                                    .equalsIgnoreCase("width: 687px; margin-left: auto; margin-right: auto;"))
                            {
                                trElems = element.getElementsByTag("tr");
                            }
                        }
                        Log.v("count tr elems", String.valueOf(trElems.size()));//
                        for (int i = 0; i < mp3Elems.size(); i++)
                        {
                            ArchieveEntity ae = new ArchieveEntity();
                            int x = mp3Elems.get(i).attr("value").lastIndexOf('=');
                            ae.setURL(mp3Elems.get(i).attr("value").substring(x + 1));
                            ae.setTime(trElems.get(i).getElementsByTag("td").get(0).text());
                            ae.setName(trElems.get(i).getElementsByTag("td").get(1).text());
                            ArchieveEntityesCollection.getEntityes().add(ae);
                        }
                    }
                    else
                    {//TODO Залогиниться не удалось, TT. Кидаем Exception=)
                        throw new WrongLoginOrPasswordException();
                    }*/
                }
            }
            catch (Exception e)
            {
            }
        }

        public class WrongLoginOrPasswordException : Exception
        {
            public WrongLoginOrPasswordException()
            {
            }

            public WrongLoginOrPasswordException(string message) : base(message)
            {
            }

            public WrongLoginOrPasswordException(string message, Exception innerException) : base(message, innerException)
            {
            }
        }
    }
}
