//using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
//using System.Web;
using WeatherForecast.Models.Context;
using WeatherForecast.Models.Weather;
using WeatherForecast.Models.Log;
using System.Threading;

namespace WeatherForecast.Models.WeatherSources
{
    internal class Apixu : WeatherSource
    {
        internal Apixu() : base(false)
        {
            this.Name = @"apixu.com";
            this.ForecastURL = @"http://api.apixu.com/v1/forecast.json?key=ae97e7b3f0894de58ea153737171909&q=Odessa,ua&days=6";
            this.DailyIconURL = ForecastURL;
            this.CurrentWeatherURL = @"http://api.apixu.com/v1/current.json?key=ae97e7b3f0894de58ea153737171909&q=Odessa,ua";
        }

        public override IEnumerable<ForecastFromSource> GetForecasts(DateTime deadline, ref List<DailyIcon> icons)
        {
            IEnumerable<DailyIcon> localIcons = null;
            try
            {
                localIcons = GetDailyIcons();
                JObject data = JsonFromUrl(ForecastURL);
                if (data == null || icons == null)
                {
                    if (deadline > DateTime.Now)
                    {
                        Thread.Sleep(1000 * 60 * 3);// 3 min
                        return GetForecasts(deadline, ref icons);
                    }
                    return null;
                }

                // determine the first time point, which has already passed. according time zones  3,6,9,12,15,18,21,00
                int hours = DateTime.Now.Hour;
                if (hours % 3 > 0)
                    hours = hours - hours % 3;


                //as this source offers an hourly forecast
                //let's define an array of dates with time for selecting the forecasts we need
                var dates = Enumerable.Range(0, CountOfForecasts).Select(x => x * 3).Select(dd => DateTime.Today.AddHours(dd + hours).ToString("yyyy-MM-dd HH:mm")).ToArray();//2017-09-20 00:00

                //make a request to the object - the result of the request to the resource
                //inside the query, create and populate a collection of Forecast objects for a three-hour forecast
                var threeHourResult = from day in (JArray)data["forecast"]["forecastday"]
                                      from hour in (JArray)day["hour"]
                                      where dates.Contains(((JValue)hour["time"]).Value.ToString())
                                      select new ForecastFromSource()
                                      {
                                          forecastCreating = DateTime.Now,
                                          source = this,
                                          date = DateTime.ParseExact(hour["time"].ToString(), "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture),
                                          airTemperature = Convert.ToDouble(hour["temp_c"].ToString()),
                                          description = WeatherDescription.GetWeatherDescription(hour["condition"]["text"].ToString().ToLower()),
                                          isDay = hour["is_day"].ToString() == "1" ? true : false,
                                          pressure = Convert.ToDouble(hour["pressure_mb"].ToString()) * WeatherCondition.mBarInMMGH,
                                          humidity = Convert.ToDouble(hour["humidity"].ToString()),
                                          windSpeed = Convert.ToDouble(hour["wind_kph"].ToString()) * WeatherCondition.metersPerSecondInKPH,
                                          windDirection = new WindDirection(Convert.ToDouble(hour["wind_degree"].ToString()), hour["wind_dir"].ToString()),
                                          rain = Convert.ToDouble(hour["precip_mm"].ToString()),   //precipitation in 3 hours 
                                          cloud = Convert.ToInt32(hour["cloud"].ToString())
                                      };
                if (threeHourResult.Count() == CountOfForecasts)
                {
                    icons.AddRange(localIcons);
                    return threeHourResult;
                }
                return null;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(String.Format("An error occurred while trying to data processing from the source: {0}; in the method : {1}; by the address {2}", Name, "GetForecasts", ForecastURL), ex);
                return null;
            }
        }

        public override IEnumerable<DailyIcon> GetDailyIcons()
        {
            try
            {
                JObject iconData = JsonFromUrl(DailyIconURL);
                if (iconData == null)
                    return null;

                // request to the same "result of the request to the resource" to get a collection of DailyIcon objects
                var dailyResult = from day in (JArray)iconData["forecast"]["forecastday"]
                                  select new DailyIcon
                                  {
                                      forecastFor = DateTime.ParseExact(day["date"].ToString(), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
                                      source = this,
                                      description = WeatherDescription.GetWeatherDescription(day["condition"]["text"].ToString().ToLower()),
                                  };

                return dailyResult;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(String.Format("An error occurred while trying to data processing from the source: {0}; in the method : {1}; by the address {2}", Name, "GetDailyIcon", DailyIconURL), ex);
                return null;
            }
        }

        public override WeatherCondition GetCurrentWeather()
        {
            dynamic data = JsonFromUrl(CurrentWeatherURL);
            if (data == null)
                return null;

            WeatherCondition weather = new WeatherCondition();

            try
            {
                weather.airTemperature = data["current"]["temp_c"].Value;
                weather.isDay = data["current"]["is_day"].Value == 1 ? true : false;
                weather.description = WeatherDescription.GetWeatherDescription(data["current"]["condition"]["text"].ToString().ToLower());
                weather.pressure = data["current"]["pressure_mb"].Value * WeatherCondition.mBarInMMGH;
                weather.humidity = data["current"]["humidity"].Value;
                weather.windSpeed = data["current"]["wind_kph"].Value * WeatherCondition.metersPerSecondInKPH;
                weather.windDirection = new WindDirection(data["current"]["wind_degree"].Value, data["current"]["wind_dir"].Value);
                weather.rain = data["current"]["precip_mm"].Value;
                weather.cloud = int.Parse(data["current"]["cloud"].ToString());
                weather.date = WeatherSource.DateTimeFromUnixTimestampSeconds((long)data["current"]["last_updated_epoch"].Value);

                return weather;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(String.Format("An error occurred while trying to get current weather from the source: {0}; in the method : {1}; by the address {2}", Name, "GetCurrentWeather", CurrentWeatherURL), ex);
                return null;
            }

        }
    }
}