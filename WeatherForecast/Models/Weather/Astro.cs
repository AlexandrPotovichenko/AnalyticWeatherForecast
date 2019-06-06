using Newtonsoft.Json.Linq;
using Quartz;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading;
using WeatherForecast.Models.Context;
using WeatherForecast.Models.Log;
using WeatherForecast.Models.WeatherSources;

namespace WeatherForecast.Models.Weather
{
    public class Astro : IJob
    {

        private static List<OneDayAstro> AllDaysAstro = null;
        private int counter = 5; //number of attempts in case of failure
        private int repeatPeriodInHours = 2;//in case of failure, repeat through

        static Astro()
        {
            using (WeatherContext db = new WeatherContext())
            {
                if (db.AstroByDays.Count() > 0)
                {
                    AllDaysAstro = db.AstroByDays.ToList();
                }
                else
                {
                    AllDaysAstro = new List<OneDayAstro>();
                }
            }
        }
        public static OneDayAstro GetAstroByDate(DateTime date)//returns Astro by date
        {
            if (AllDaysAstro == null)
                return null;

            OneDayAstro dayAstro = AllDaysAstro.Where(x => x.Sunrise.HasValue && x.Sunrise.Value.DayOfYear == date.DayOfYear).FirstOrDefault(); //looking for the object of list by date

            if (dayAstro != null)
            {
                return dayAstro;
            }
            else //there is no record on this date
            {
                Logger.Log.Error(String.Format("An error occurred while trying to receive astro data by date. In the method : {0}", "GetAstroByDate"));
                return null;
            }
        }
        public void Execute(IJobExecutionContext context) // starts once a day (at the beginning) and updates the list of Astro
        {
            Logger.Log.Debug("Starting of astro on schedule in " + DateTime.Now.ToString());

            List<OneDayAstro> currentAstroData = null;
            currentAstroData = GetAstroFromApixu();
            while (currentAstroData == null || counter == 0)
            {
                Thread.Sleep(1000 * 60 * 60 * repeatPeriodInHours);
                currentAstroData = GetAstroFromApixu();
                counter--;
            }

            if (currentAstroData != null)
            {
                using (WeatherContext db = new WeatherContext())
                {
                    var deleteAstroData =
                       from astro in db.AstroByDays
                       where astro.Sunrise.Value < DateTime.Today && db.OneDayAnalyticForecast.Where(x=>x.astro==astro).Count()==0
                       select astro;

                    foreach (var astro in deleteAstroData)
                    {
                        db.AstroByDays.Remove(astro);
                    }

                    foreach (var astro in currentAstroData)
                    {
                        if (db.AstroByDays.Where(x=>x.Sunrise==astro.Sunrise).Count()==0)
                            db.AstroByDays.Add(astro);
                    }
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        Logger.Log.Error("An error occurred while trying to save astro", ex);
                    }

                    AllDaysAstro = db.AstroByDays.ToList();
                }
            }
            else
            {
                Logger.Log.Error(String.Format("An error occurred while trying to receive astro data on schedule"));
            }
        }
        private List<OneDayAstro> GetAstroFromApixu()
        {
            string apixuURL = @"http://api.apixu.com/v1/forecast.json?key=ae97e7b3f0894de58ea153737171909&q=Odessa,ua&days=6";
            dynamic data = JsonFromUrl(apixuURL, "Apixu");

            if (data == null)
                return null;

            List<OneDayAstro> sixDayAstro = new List<OneDayAstro>();
            try
            {
                DateTime timeValue;
                string format = "yyyy-MM-dd hh:mm tt";
                foreach (var day in data.forecast.forecastday)
                {
                    OneDayAstro astro = new OneDayAstro();

                    if (DateTime.TryParseExact(day["date"].ToString() + " " + day.astro.sunrise.ToString(), format, CultureInfo.InvariantCulture, DateTimeStyles.None, out timeValue))
                        astro.Sunrise = timeValue;

                    if (DateTime.TryParseExact(day["date"].ToString() + " " + day.astro.sunset.ToString(), format, CultureInfo.InvariantCulture, DateTimeStyles.None, out timeValue))
                        astro.Sunset = timeValue;

                    if (DateTime.TryParseExact(day["date"].ToString() + " " + day.astro.moonrise.ToString(), format, CultureInfo.InvariantCulture, DateTimeStyles.None, out timeValue))
                        astro.Moonrise = timeValue;

                    if (DateTime.TryParseExact(day["date"].ToString() + " " + day.astro.moonset.ToString(), format, CultureInfo.InvariantCulture, DateTimeStyles.None, out timeValue))
                        astro.Moonset = timeValue;

                    sixDayAstro.Add(astro);
                }

                if (sixDayAstro.Count > 0)
                    return sixDayAstro;
                else
                {
                    Logger.Log.Error(String.Format("An error occurred while trying to receive astro data on schedule from the source: {0}; in the method : {1}; by the address {2}", "Apixu", "GetAstroFromApixu", apixuURL));
                    return null;
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(String.Format("An error occurred while trying to receive astro data on schedule from the source: {0}; in the method : {1}; by the address {2}", "Apixu", "GetAstroFromApixu", apixuURL), ex);
                return null;
            }
        }
        protected JObject JsonFromUrl(string URL, string sourceName)
        {
            try
            {
                WebClientWithDecompression WebClient = new WebClientWithDecompression();
                var source = WebClient.DownloadString(URL);
                return JObject.Parse(source);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(String.Format("An error occurred while trying to retrieve data from the source: {0}; in the method : {1}; by the address {2}", sourceName, "JsonFromUrl", URL), ex);
            }
            return null;
        }
    }
}