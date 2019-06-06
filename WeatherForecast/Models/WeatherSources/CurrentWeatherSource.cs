using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using WeatherForecast.Models.Weather;
using WeatherForecast.Models.Log;
using Quartz;
using WeatherForecast.Models.Context;
using WeatherForecast.Models.ViewModels;
using WeatherForecast.Models.WeatherForecast;

namespace WeatherForecast.Models.WeatherSources
{
    public class CurrentWeatherSource : IJob
    { 
        private static WeatherCondition currentWeather;
        private List<WeatherSource> weatherSources;
        public static WeatherCondition CurrentWeather
        {
            get
            {
                if (currentWeather == null || currentWeather.date == DateTime.MinValue)
                    return null;
                lock (currentWeather)
                    return currentWeather;
            }
            protected set
            {
                lock (currentWeather)
                    currentWeather = value;
            }
        }
        private readonly string apixuURL = @"http://api.apixu.com/v1/current.json?key=ae97e7b3f0894de58ea153737171909&q=46.47%2C30.73";
        private static WebClientWithDecompression client;
        public CurrentWeatherSource()
        {
            using (WeatherContext db = new WeatherContext())
            {
               weatherSources = db.WeatherSources.Where(source => source.IsCurrentWeatherAvailable).ToList();
            }
        }
        static CurrentWeatherSource()
        {
            client = new WebClientWithDecompression();
            currentWeather = new WeatherCondition();
        }
        public void Execute(IJobExecutionContext context)
        {
            Logger.Log.Debug("Starting of current weather on schedule in " + DateTime.Now.ToString());

            WeatherCondition currentWeatherFromSource = GetCurrentWeatherFromApixu();

            if (currentWeatherFromSource != null && (DateTime.Now - currentWeatherFromSource.date).TotalMinutes < 30)
            {
                CurrentWeather = currentWeatherFromSource;
            }
            else
            {    
                foreach (var weatherSource in weatherSources)
                {
                    currentWeatherFromSource = weatherSource.GetCurrentWeather();
                    if (CurrentWeather == null || currentWeatherFromSource == null || currentWeatherFromSource.date > CurrentWeather.date)
                        CurrentWeather = currentWeatherFromSource;
                }
            }
        }
        private WeatherCondition GetCurrentWeatherFromApixu()
        {
            dynamic data = JsonFromUrl(apixuURL, "Apixu");
            if (data == null)
                return null;

            WeatherCondition weather = new WeatherCondition();
            string descriptionAsStr = data["current"]["condition"]["text"].Value.ToString().ToLower();
            try
            {
                weather.airTemperature = data["current"]["temp_c"].Value;
                weather.description = WeatherDescription.GetWeatherDescription(descriptionAsStr); 
                weather.isDay = data["current"]["is_day"].Value == 1 ? true : false;
                weather.pressure = data["current"]["pressure_mb"].Value * WeatherCondition.mBarInMMGH;
                weather.humidity = data["current"]["humidity"].Value;
                weather.windSpeed = data["current"]["wind_kph"].Value * WeatherCondition.metersPerSecondInKPH;
                weather.windDirection = new WindDirection(data["current"]["wind_degree"].Value, data["current"]["wind_dir"].Value);
                if(descriptionAsStr.Contains("snow"))
                {
                    weather.rain = 0.0;
                    weather.snow = data["current"]["precip_mm"].Value;
                }
                else
                {
                    weather.rain = data["current"]["precip_mm"].Value;
                    weather.snow = 0.0;
                }
                weather.cloud = int.Parse(data["current"]["cloud"].ToString());
                weather.date = WeatherSource.DateTimeFromUnixTimestampSeconds((long)data["current"]["last_updated_epoch"].Value); 

                return weather;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(String.Format("An error occurred while trying to get current weather from the source: {0}; in the method : {1}; by the address {2}", "Apixu", "GetCurrentWeather", apixuURL), ex);
                return null;
            }
        }
        protected JObject JsonFromUrl(string URL, string sourceName)
        {
            try
            {
                var source = client.DownloadString(URL);

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