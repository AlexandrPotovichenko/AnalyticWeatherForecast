using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using WeatherForecast.Models.Weather;
using WeatherForecast.Models.Log;
using System.ComponentModel.DataAnnotations;
using System.Threading;

namespace WeatherForecast.Models.WeatherSources
{
    public class WeatherSource
    {
        static WeatherSource()
        {
            client = new WebClient();
        }
        public WeatherSource()
        {
            this.IsCurrentWeatherAvailable = false;
        }
        protected WeatherSource(bool isCurrentWeatherAvailable)
        {
            this.IsCurrentWeatherAvailable = isCurrentWeatherAvailable;
        }
        [Key]
        public string Name { get; protected set; }
        public string ForecastURL { get; protected set; }
        public string DailyIconURL { get; protected set; }
        public string CurrentWeatherURL { get; protected set; }
        public bool IsCurrentWeatherAvailable { get; protected set; }

        private static WebClient client; 

        protected const int CountOfForecasts = 40; 

        private static State CurrentState;

        protected static readonly System.DateTime UnixEpoch = new DateTime(1970, 1, 1); 

        public State SourceState
        {
            get
            {
                return CurrentState;
            }
            set
            {
                CurrentState = value;
            }
        }
        public virtual IEnumerable<ForecastFromSource> GetForecasts(DateTime deadline, ref List<DailyIcon> icons) { icons = null; return null; }
        public virtual IEnumerable<DailyIcon> GetDailyIcons() { return null; }
        public virtual WeatherCondition GetCurrentWeather() { return null; }

        protected static JObject JsonFromUrl(string URL,int numberOfAttempts = 5)
        {
            lock (client)
            { 
            
            while (numberOfAttempts>0)
            { 
                try
                {
                    var source = client.DownloadString(URL);
                    return JObject.Parse(source);
                }
                catch (Exception ex)
                {
                    Logger.Log.Error(String.Format("An error occurred while trying to retrieve data in the method : {0}; by the address {1}", "JsonFromUrl",URL), ex);
                }
                    Thread.Sleep(1000 * 60);// wait a minute
                    numberOfAttempts--;
            }
            return null;
            }
        }

        public enum State
        {
            Active,
            Passive,
            WaitingForStationAnswer
        }

        public static DateTime DateTimeFromUnixTimestampSeconds(long seconds)
        {
            return UnixEpoch.AddSeconds(seconds).ToLocalTime();
        }
        public static long UnixTimestampSecondsFromDateTime(DateTime dt)
        {
            return (long)(dt.ToUniversalTime().Subtract(UnixEpoch).TotalSeconds);
        }
        public static long UnixTimestampSecondsFromUTC(DateTime dt)
        {
            return (long)(dt.Subtract(UnixEpoch).TotalSeconds);
        }

    }
}