//using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using WeatherForecast.Models.Context;
using WeatherForecast.Models.Weather;
using WeatherForecast.Models.Log;
using System.Threading;

namespace WeatherForecast.Models.WeatherSources
{
    public class WeatherBit : WeatherSource
    {
        internal WeatherBit() : base(true)
        {
            this.Name = @"weatherbit.io";
            this.ForecastURL = @"https://api.weatherbit.io/v2.0/forecast/3hourly?city_id=698740&key=ac82d2131bf64ad3a9e4b91bd3bfc0bc";
            this.DailyIconURL = @"https://api.weatherbit.io/v2.0/forecast/daily?city_id=698740&key=ac82d2131bf64ad3a9e4b91bd3bfc0bc";
            this.CurrentWeatherURL = @"https://api.weatherbit.io/v2.0/current?city_id=698740&key=ac82d2131bf64ad3a9e4b91bd3bfc0bc";
            this.IsCurrentWeatherAvailable = true;
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
                };
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                var threeHourResult = from forecast in (JArray)data["data"]
                                      select new ForecastFromSource()
                                      {

                                          forecastCreating = DateTime.Today.AddHours(DateTime.Now.Hour),
                                          source = this,
                                          date = WeatherSource.DateTimeFromUnixTimestampSeconds((long)forecast["ts"]),
                                          airTemperature = Convert.ToDouble(forecast["temp"].ToString()),
                                          description = WeatherDescription.GetWeatherDescription(forecast["weather"]["description"].ToString().ToLower()),
                                          isDay = forecast["pod"].ToString() == "d" ? true : false,
                                          pressure = Convert.ToDouble(forecast["slp"].ToString()) * WeatherCondition.mBarInMMGH,
                                          humidity = Convert.ToDouble(forecast["rh"].ToString()),
                                          windSpeed = Convert.ToDouble(forecast["wind_spd"].ToString()),
                                          windDirection = new WindDirection(Convert.ToDouble(forecast["wind_dir"].ToString()), forecast["wind_cdir"].ToString()),
                                          rain = Convert.ToDouble(forecast["precip"].ToString()),   //precipitation in 3 hours 
                                          snow = Convert.ToDouble(forecast["snow"].ToString()),   //precipitation in 3 hours 
                                          cloud = Convert.ToInt32(forecast["clouds"].ToString())
                                      };

                if (threeHourResult.Count() == CountOfForecasts)
                {
                    icons.AddRange(localIcons);
                    return threeHourResult;
                }
                else
                {
                    if (deadline > DateTime.Now)
                    {
                        Thread.Sleep(1000 * 60 * 3);// 3 min
                        return GetForecasts(deadline, ref icons);
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(String.Format("An error occurred while trying to data processing from the source: {0}; in the method : {1}; by the address {2}", Name, "GetForecasts", ForecastURL), ex);

                if (deadline > DateTime.Now)
                {
                    Thread.Sleep(1000 * 60 * 3);// 3 min
                    return GetForecasts(deadline, ref icons);
                }
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

                var dailyResult = from forecast in (JArray)iconData["data"]
                                  where DateTime.ParseExact(forecast["datetime"].ToString(), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture) <= DateTime.Today.AddDays(5)
                                  select new DailyIcon
                                  {
                                      forecastFor = DateTime.ParseExact(forecast["datetime"].ToString(), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
                                      source = this,
                                      description = WeatherDescription.GetWeatherDescription(forecast["weather"]["description"].ToString().ToLower()),
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
                weather.airTemperature = data["data"][0]["temp"].Value;
                weather.isDay = data["data"][0]["pod"].ToString() == "d" ? true : false;
                weather.description = WeatherDescription.GetWeatherDescription(data["data"][0]["weather"]["description"].Value.ToString().ToLower());
                weather.pressure = data["data"][0]["slp"].Value * WeatherCondition.mBarInMMGH;
                weather.humidity = data["data"][0]["rh"].Value;
                weather.windSpeed = data["data"][0]["wind_spd"].Value;
                weather.windDirection = new WindDirection(data["data"][0]["wind_dir"].Value, data["data"][0]["wind_cdir"].Value);
                weather.rain = data["data"][0]["precip"].Value == null ? 0 : data["data"][0]["precip"].Value;
                ((Newtonsoft.Json.Linq.JObject)(data["data"][0])).Values("snow");
                weather.snow = (data["data"][0]["snow"] != null && data["data"][0]["snow"].Value != null) ? data["data"][0]["snow"].Value : 0;
                weather.cloud = int.Parse(data["data"][0]["clouds"].ToString());
                weather.date = WeatherSource.DateTimeFromUnixTimestampSeconds((long)data["data"][0]["ts"].Value);

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