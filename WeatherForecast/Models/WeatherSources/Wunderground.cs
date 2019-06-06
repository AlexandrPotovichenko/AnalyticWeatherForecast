using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WeatherForecast.Models.Weather;
using WeatherForecast.Models.Log;
using Newtonsoft.Json.Linq;
using WeatherForecast.Models.Context;
using System.Threading;

namespace WeatherForecast.Models.WeatherSources
{
    public class Wunderground : WeatherSource
    {
        private string signOfTheDay = @"http://icons.wxug.com/i/c/k/nt_";
        internal Wunderground() : base(false)
        {
            this.Name = @"wunderground.com";
            this.ForecastURL = @"http://api.wunderground.com/api/1086a4a1b0619326/hourly10day/q/UA/Odessa.json";
            this.DailyIconURL = @"http://api.wunderground.com/api/1086a4a1b0619326/forecast10day/q/UA/Odessa.json";
            this.CurrentWeatherURL = @"http://api.wunderground.com/api/1086a4a1b0619326/conditions/q/UA/Odessa.json";
            IsCurrentWeatherAvailable = false;
        }
        public override IEnumerable<ForecastFromSource> GetForecasts(DateTime deadline, ref List<DailyIcon> icons)
        {
            IEnumerable<DailyIcon> localIcons = null;
            try
            {
                localIcons = GetDailyIcons();
                JObject data = JsonFromUrl(ForecastURL);//JObject.Parse(source);
                if (data == null || localIcons == null)
                {
                    if (deadline > DateTime.Now)
                    {
                        Thread.Sleep(1000 * 60 * 3);// 3 min
                        return GetForecasts(deadline, ref icons);
                    }
                    Logger.Log.Debug("In Wunderground deadline " + deadline + " at" + DateTime.Now);
                    return null;
                };

                // determine the first time point, which has not already passed. according time zones   2,5,8,11,14,17,20,23
                int hours = DateTime.Now.ToUniversalTime().Hour;//UTC 
                if (hours % 3 > 0)
                    hours = hours - hours % 3; // hours are already passed.
                hours += 3;// UTC & Nex is +3 hour

                //as this source offers an hourly forecast,  Define an array of dates with time for selecting the forecasts we need.
                var dates = Enumerable.Range(0, CountOfForecasts).Select(x => x * 3).Select(dd => UnixTimestampSecondsFromUTC(DateTime.Now.ToUniversalTime().Date.AddHours(dd + hours))).ToArray();//2017-09-20 00:00

                var threeHourResult = from hour in (data["hourly_forecast"])
                                      where dates.Contains((long)hour["FCTTIME"]["epoch"])
                                      select new ForecastFromSource()
                                      {
                                          forecastCreating = DateTime.Today.AddHours(DateTime.Now.Hour),
                                          source = this,
                                          date = DateTimeFromUnixTimestampSeconds((long)hour["FCTTIME"]["epoch"]),
                                          airTemperature = Convert.ToDouble(hour["temp"]["metric"].ToString()),
                                          description = WeatherDescription.GetWeatherDescription(hour["condition"].ToString().ToLower()),
                                          isDay = hour["icon_url"].ToString().Contains(signOfTheDay) ? false : true,
                                          pressure = Convert.ToDouble(hour["mslp"]["metric"].ToString()) * WeatherCondition.mBarInMMGH,
                                          humidity = Convert.ToDouble(hour["humidity"].ToString()),
                                          windSpeed = Convert.ToDouble(hour["wspd"]["metric"].ToString()) * WeatherCondition.metersPerSecondInKPH,
                                          windDirection = new WindDirection(Convert.ToDouble(hour["wdir"]["degrees"].ToString()), hour["wdir"]["dir"].ToString()),
                                          rain = Convert.ToDouble(hour["qpf"]["metric"].ToString()),   //precipitation in 3 hours 
                                          snow = Convert.ToDouble(hour["snow"]["metric"].ToString()),   //precipitation in 3 hours 
                                          cloud = Convert.ToInt32(hour["sky"].ToString())
                                      };

                if (threeHourResult.Count() == CountOfForecasts)
                {
                    icons.AddRange(localIcons);
                    return threeHourResult;
                }
                else
                {
                    Logger.Log.Debug("In Wunderground " + threeHourResult.Count() + " forecasts.");
                    if (deadline > DateTime.Now)
                    {
                        Thread.Sleep(1000 * 60 * 3);// 3 min
                        return GetForecasts(deadline, ref icons);
                    }
                    Logger.Log.Debug("In Wunderground deadline " + deadline + " at " + DateTime.Now);
                    return null;
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Debug("In Wunderground ", ex);

                if (deadline > DateTime.Now)
                {
                    Thread.Sleep(1000 * 60 * 3);// 3 min
                    return GetForecasts(deadline, ref icons);
                }
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
                // request to the same "result" to get a collection of DailyIcon objects
                var dailyResult = from day in (JArray)iconData["forecast"]["simpleforecast"]["forecastday"]
                                  where DateTimeFromUnixTimestampSeconds((long)day["date"]["epoch"]) <= DateTime.Today.AddDays(5)
                                  select new DailyIcon
                                  {
                                      forecastFor = DateTimeFromUnixTimestampSeconds((long)day["date"]["epoch"]),
                                      source = this,
                                      description = WeatherDescription.GetWeatherDescription(day["conditions"].ToString().ToLower())
                                  };

                return dailyResult;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(String.Format("An error occurred while trying to data processing from the source: {0}; in the method : {1}; by the address {2}", Name, "GetDailyIcon", DailyIconURL), ex);
                return null;
            }
        }

    }
}