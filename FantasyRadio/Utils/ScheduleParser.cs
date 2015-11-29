using FantasyRadio.DataModel;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

namespace FantasyRadio.Utils
{
    public class ScheduleParser
    {
        private const string URL = "https://www.googleapis.com/calendar/v3/calendars/fantasyradioru@gmail.com/events?key=AIzaSyDam413Hzm4l8GOEEg-NF8w8wdAbUsKEjM&maxResults=50&singleEvents=true&orderBy=startTime";

        public Task<ObservableCollection<KeyedList<string, ScheduleEntity>>> ParseScheduleAsync() //Вместо bool - List<ArchiveEntity>
        {
            return Task.Run(() => ParseSchedule());
        }
        private ObservableCollection<KeyedList<string, ScheduleEntity>> ParseSchedule()
        {
            var result = new ObservableCollection<KeyedList<string, ScheduleEntity>>();
            try
            {
                using (HttpClient http = new HttpClient())
                {
                    // -----------------------------------------------
                    DateTime ld = DateTime.Now;
                    string pattern = "yyyy'-'MM'-'dd'T'";
                    string urlMin = "&timeMin=" + ld.ToString(pattern) + "00:00:00.000Z";
                    string urlMax = "&timeMax=" + ld.AddDays(3).ToString(pattern)
                            + "00:00:00.000Z";
                    // -----------------------------------------------
                    // -------------------------------------

                    Task<string> response = http.GetStringAsync(URL + urlMin + urlMax);
                    response.Wait();
                    JObject jObject = JObject.Parse(response.Result);
                    JArray entryArray = (JArray)jObject["items"];
                    var entityes = new List<ScheduleEntity>();
                    for (int i = 0; i < entryArray.Count; i++)
                    {
                        ScheduleEntity se = new ScheduleEntity();
                        try
                        {
                            se.Title = entryArray[i]["summary"].ToString();
                            //----------------------------------------------
                            HtmlDocument document = new HtmlDocument();
                            document.LoadHtml(entryArray[i]["description"].ToString().Replace("ПОДРОБНЕЕ", ""));
                            var tds = document.DocumentNode.Descendants("td");
                            var imgs = document.DocumentNode.Descendants("img");
                            bool tdsEmty = true;
                            HtmlNode tdsLast = null;
                            foreach(var td in tds)
                            {
                                tdsEmty = false;
                                tdsLast = td;
                            }
                            HtmlNode imgFirst = null;
                            foreach(var img in imgs)
                            {
                                imgFirst = img;
                                break;
                            }
                            if (!tdsEmty)
                            {
                                se.ImageURL = imgFirst.Attributes["src"].Value;
                                se.Text = tdsLast.InnerText;
                            }
                            else
                            {
                                se.Text = entryArray[i]["description"].ToString().Replace("ПОДРОБНЕЕ", "");
                            }
                            // ---------------------------------------------
                            string startDate = entryArray[i]["start"]["dateTime"].ToString();
                            //startDate = startDate.Substring(0, startDate.IndexOf("+")); //TOD Exception
                            string endDate = entryArray[i]["end"]["dateTime"].ToString();
                            //endDate = endDate.Substring(0, endDate.IndexOf("+"));

                            se.StartDate = DateTime.Parse(startDate);
                            se.EndDate = DateTime.Parse(endDate);

                            entityes.Add(se);
                        }
                        catch (Exception e)
                        {
                            string jsonerr = e.StackTrace;
                        }
                    }
                    //entityes.Reverse();
                    var groupedEntityes =
                    from entity in entityes
                    //orderby entity.Day
                    group entity by entity.Day into entityesByDay
                    select new KeyedList<string, ScheduleEntity>(entityesByDay);
                    result = new ObservableCollection<KeyedList<string, ScheduleEntity>>(groupedEntityes);
                }
            }
            catch (Exception e)
            {
                string err = e.StackTrace;
            }
            return result;
        }
    }
}
