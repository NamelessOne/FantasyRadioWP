using FantasyRadio.DataModel;
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
        public Task<List<ArchiveEntity>> ParseArchiveAsync(string login, string password) //Вместо bool - List<ArchiveEntity>
        {
            return Task.Run(() => ParseArchive(login, password));
        }

        private List<ArchiveEntity> ParseArchive(string login, string password)
        {
            var result = new List<ArchiveEntity>();
            try
            {
                var handler = new HttpClientHandler { CookieContainer = new CookieContainer(), AllowAutoRedirect = true, UseCookies = true };
                HtmlNode.ElementsFlags.Remove("form");
                using (HttpClient http = new HttpClient(handler))
                {
                    http.DefaultRequestHeaders.Add("User-Agent",
                                 "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:5.0) Gecko/20100101 Firefox/5.0");
                    Task<string> response = http.GetStringAsync("http://fantasyradio.ru/");
                    response.Wait();
                    // -------------------------------------
                    var document = new HtmlDocument();
                    document.LoadHtml(response.Result);

                    var loginForm = document.GetElementbyId("login-form");
                    List<HtmlNode> hiddenInputs = new List<HtmlNode>();
                    var inputs = loginForm.Descendants("input");
                    foreach (var node in inputs)
                    {
                        if (node.GetAttributeValue("type", "").Equals("hidden"))
                            hiddenInputs.Add(node);
                    }
                    var parameters = new List<KeyValuePair<string, string>>();
                    parameters.Add(new KeyValuePair<string, string>("return", hiddenInputs[2].GetAttributeValue("value", "")));
                    parameters.Add(new KeyValuePair<string, string>(hiddenInputs[3].GetAttributeValue("name", ""), "1"));
                    parameters.Add(new KeyValuePair<string, string>("username", login));
                    parameters.Add(new KeyValuePair<string, string>("password", password));
                    parameters.Add(new KeyValuePair<string, string>("task", "user.login"));
                    parameters.Add(new KeyValuePair<string, string>("option", "com_users"));
                    Task<HttpResponseMessage> postResponse;
                    using (HttpClient postHttp = new HttpClient(handler))
                    {
                        postHttp.DefaultRequestHeaders.Add("User-Agent",
                                 "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:5.0) Gecko/20100101 Firefox/5.0");
                        var content = new FormUrlEncodedContent(parameters);
                        postResponse = postHttp.PostAsync("http://fantasyradio.ru/index.php/vojti-na-sajt?task=user.login", content);
                        postResponse.Wait();
                        var task = postResponse.Result.Content.ReadAsStringAsync();
                        task.Wait();
                        string body = task.Result;
                        if (!body.Contains("Имя пользователя и пароль не совпадают или у вас еще нет учетной записи на сайте"))
                        { //Проверка на на правильность логина/пароля    
                            Task<string> getResponse;
                            using (HttpClient getHttp = new HttpClient(handler))
                            {
                                getHttp.DefaultRequestHeaders.Add("User-Agent",
                                         "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:5.0) Gecko/20100101 Firefox/5.0");
                                getResponse = getHttp.GetStringAsync("http://fantasyradio.ru/index.php/component/content/article/2-uncategorised/14-stranitsa-2");
                                getResponse.Wait();
                                document = new HtmlDocument();
                                document.LoadHtml(getResponse.Result);
                                List<HtmlNode> mp3Elems = new List<HtmlNode>();
                                var inputNodes = document.DocumentNode.Descendants("param");
                                foreach (var node in inputNodes)
                                {
                                    if (node.GetAttributeValue("name", "").Equals("FlashVars"))
                                        mp3Elems.Add(node);
                                }
                                //List<HtmlNode> tableElems = new List<HtmlNode>();
                                var tableElems = document.DocumentNode.Descendants("table");
                                var trElems = new List<HtmlNode>();
                                foreach (var node in tableElems)
                                {
                                    if (node.GetAttributeValue("style", "")
                                            .Equals("width: 687px; margin-left: auto; margin-right: auto;"))
                                    {
                                        trElems.AddRange(node.Descendants("tr"));
                                    }
                                }
                                for (int i = 0; i < mp3Elems.Count; i++)
                                {

                                    var ae = new ArchiveEntity();
                                    int x = mp3Elems[i].GetAttributeValue("value", "").LastIndexOf('=');
                                    if (x >= 0)
                                        ae.URL = mp3Elems[i].GetAttributeValue("value", "").Substring(x + 1);
                                    var trlist = new List<HtmlNode>();
                                    trlist.AddRange(trElems[i].Descendants("td"));
                                    ae.Time = trlist[0].InnerText;
                                    ae.Name = trlist[1].InnerText.Replace("&nbsp;", "");
                                    result.Add(ae);
                                }
                            }
                        }
                        else
                        {//TODO Залогиниться не удалось, TT. Кидаем Exception=)
                            throw new WrongLoginOrPasswordException();
                        }
                    }
                }
            }
            catch (Exception e)
            {
            }
            return result;
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
