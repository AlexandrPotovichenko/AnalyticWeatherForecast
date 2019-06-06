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
    public class OpenWeatherMap: WeatherSource
    {
        internal OpenWeatherMap() : base(true)
        {
            this.Name = @"openweathermap.org";

            this.ForecastURL = @"http://api.openweathermap.org/data/2.5/forecast?id=698740&units=metric&APPID=9c22b55a2f9c0b8b8400d4618f13582f";          
            this.DailyIconURL = @"http://api.openweathermap.org/data/2.5/forecast/daily?id=698740&units=metric&APPID=5e9b034901c16e0d8325192d9bf5a2a0&cnt=6";          
            this.CurrentWeatherURL = @"http://api.openweathermap.org/data/2.5/weather?q=Odessa,ua&units=metric&APPID=9c22b55a2f9c0b8b8400d4618f13582f";           
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
                }

                var threeHourResult = from day in (JArray)data["list"]
                                      select new ForecastFromSource()
                                      {
                                          forecastCreating = DateTime.Today.AddHours(DateTime.Now.Hour),
                                          source = this,
                                          date = WeatherSource.DateTimeFromUnixTimestampSeconds((long)day["dt"]),
                                          airTemperature = Convert.ToDouble(day["main"]["temp"].ToString()),
                                          description = WeatherDescription.GetWeatherDescription(day["weather"][0]["description"].ToString().ToLower()),
                                 isDay = day["sys"]["pod"].ToString() == "d" ? true : false,
                                     pressure = Convert.ToDouble(day["main"]["sea_level"].ToString()) * WeatherCondition.mBarInMMGH,
                                     humidity = Convert.ToDouble(day["main"]["humidity"].ToString()),
                                     windSpeed = Convert.ToDouble(day["wind"]["speed"].ToString()),
                                 windDirection = new WindDirection(Convert.ToDouble(day["wind"]["deg"].ToString())),
                                     rain = day.SelectToken("rain") == null ? 0.0 : day["rain"].HasValues? Convert.ToDouble(day["rain"]["3h"].ToString()): 0.0,
                                     snow = day.SelectToken("snow") == null ? 0.0 : day["snow"].HasValues ? Convert.ToDouble(day["snow"]["3h"].ToString()) : 0.0,
                                 cloud = Convert.ToInt32(day["clouds"]["all"].ToString())
                             };
                if (threeHourResult.Count() == CountOfForecasts)
                {
                    icons.AddRange(localIcons);
                    return threeHourResult;
                }
                else
                {
                    if (threeHourResult.Count() >= CountOfForecasts - 5)
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
                        Logger.Log.Debug("the number of forecasts is less than normal(40-5) and equal  " + threeHourResult.Count());
                        return null;
                    }
                }             
            }
            catch (Exception ex)
            {
                Logger.Log.Error(String.Format("An error occurred while trying to data processing from the source: {0}; in the method : {1}; by the address {2}", Name, "GetForecasts", ForecastURL), ex);
                
                    if (deadline > DateTime.Now)
                    {
                        Thread.Sleep(1000 * 60 * 3);// 3 min
                        return GetForecasts(deadline,ref icons);
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
                var dailyResult = from day in (JArray)iconData["list"]
                                  select new DailyIcon
                                  {
                                      forecastFor = DateTimeFromUnixTimestampSeconds(Convert.ToInt64(day["dt"].ToString())),
                                      source = this,
                                      description = WeatherDescription.GetWeatherDescription(day["weather"][0]["description"].ToString().ToLower()),                       
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
            dynamic data = JsonFromUrl(CurrentWeatherURL);//JObject.Parse(source);
            if (data == null)
                return null;

            WeatherCondition weather = new WeatherCondition();
            try
            {
                weather.airTemperature = data["main"]["temp"].Value;
                weather.description = WeatherDescription.GetWeatherDescription(data["weather"][0]["description"].Value.ToString().ToLower());
                weather.isDay = data["weather"][0]["icon"].ToString().IndexOf("d") >= 0 ? true : false;
                weather.pressure = data["main"]["pressure"].Value * WeatherCondition.mBarInMMGH;
                weather.humidity = data["main"]["humidity"].Value;
                weather.windSpeed = data["wind"]["speed"].Value;
                weather.windDirection = data["wind"]["deg"]==null? new WindDirection(): new WindDirection(data["wind"]["deg"].Value);
                weather.rain = data.SelectToken("rain") == null ? 0.0 : data["rain"].HasValues ? data["rain"]["3h"].Value : 0.0;
                weather.snow = data.SelectToken("snow") == null ? 0.0 : data["snow"].HasValues ? data["snow"]["3h"].Value : 0.0;  
                weather.cloud = int.Parse(data["clouds"]["all"].ToString());
                weather.date = WeatherSource.DateTimeFromUnixTimestampSeconds((long)data["dt"].Value);

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