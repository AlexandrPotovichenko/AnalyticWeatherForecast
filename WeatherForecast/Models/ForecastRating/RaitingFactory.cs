using WeatherForecast.Models.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using WeatherForecast.Models.Context;
using WeatherForecast.Models.Weather;
using WeatherForecast.Models.WeatherForecast;
using WeatherForecast.Models.WeatherSources;

namespace WeatherForecast.Models.ForecastRating
{
    public static class RaitingFactory
    {
        private static int topOfTheRating = 100;// is the upper limit of the rating (rating have range between 0 and 100)
        public static IEnumerable<Rating> GetRatings(WeatherCondition currentWeather)
        {
            DateTime date = currentWeather.date;
            WeatherCondition condition = currentWeather;

            using (WeatherContext db = new WeatherContext())
            {
                List<WeatherSource> countForecastSources = db.WeatherSources.ToList();
                // collection of the differences between current weather
                var allForecastsForTimeSlot = from fc in db.Forecasts.Include("description").Include("description.Icon")
                                              where (fc.date == date)
                                              select new
                                              {
                                                  forecastCreating = fc.forecastCreating,
                                                  forecastFor = fc.date,
                                                  source = fc.source,
                                                  airTemperature = Math.Abs((fc.airTemperature - Rating.absoluteZeroTemperature) - (condition.airTemperature - Rating.absoluteZeroTemperature)),

                                                  pressure = Math.Abs(fc.pressure - condition.pressure),
                                                  humidity = Math.Abs(fc.humidity - condition.humidity),
                                                  windSpeed = Math.Abs(fc.windSpeed - condition.windSpeed),
                                                  windDirection = Math.Abs(fc.windDirection.Degrees - condition.windDirection.Degrees) > 180 ? (360 - Math.Abs(fc.windDirection.Degrees - condition.windDirection.Degrees)) : Math.Abs(fc.windDirection.Degrees - condition.windDirection.Degrees),
                                                  fallout = Math.Abs(fc.rain - condition.rain),
                                                  snow = Math.Abs(fc.snow - condition.snow),
                                                  cloud = Math.Abs(fc.cloud - condition.cloud),
                                                  weatherDescription = fc.description.Icon.Id == condition.description.Icon.Id ? 1 : 0
                                              };
              

                if (allForecastsForTimeSlot.Count() == 0)
                {
                    Logger.Log.Error("An error occurred because there are no forecasts on " + date);
                    return null;
                }

                List<Rating> result = null;// new List<Rating>();

                foreach (var forecasts in allForecastsForTimeSlot.GroupBy(x => x.forecastCreating))// forecasts - прогнозы на таймслот сгруппированы по дню(дате) создания( список всех источников с прогнозами)
                {

                    var ratings = from forecast in forecasts
                                  select new Rating
                                  {
                                      ForecastDate = forecast.forecastCreating,//DateTime.Today,
                                      ForecastFor = forecast.forecastFor,
                                      Source = forecast.source,
                                      AirTemperature = GetRatingsValue(forecasts.Max(x => x.airTemperature), forecast.airTemperature),
                                      Pressure = GetRatingsValue(forecasts.Max(x => x.pressure), forecast.pressure),
                                      Humidity = GetRatingsValue(forecasts.Max(x => x.humidity), forecast.pressure),
                                      WindSpeed = GetRatingsValue(forecasts.Max(x => x.windSpeed), forecast.windSpeed),
                                      WindDirection = GetRatingsValue(forecasts.Max(x => x.windDirection), forecast.windDirection),
                                      Fallout = GetRatingsValue(forecasts.Max(x => x.fallout), forecast.fallout),
                                      Snow = GetRatingsValue(forecasts.Max(x => x.snow), forecast.snow),
                                      Cloud = GetRatingsValue(forecasts.Max(x => x.cloud), forecast.cloud),
                                      WeatherDescription = forecasts.Max(x => x.weatherDescription)
                                  };

                    if (ratings.Count() > 0)
                    {

                        if (ratings.Count() < countForecastSources.Count())// case handling when one of the sources does not have forecasts
                        {
                            foreach (var s in countForecastSources)
                            {
                                if (ratings.Where(x => x.Source == s).Count() == 0) //Add the missing forecast with the worst ratings
                                {
                                    Rating rat = new Rating();
                                    rat.ForecastDate = forecasts.FirstOrDefault().forecastCreating;
                                    rat.ForecastFor = date;//forecasts.FirstOrDefault.fo
                                    rat.Source = s;
                                    rat.AirTemperature = ratings.Min(x => x.AirTemperature);
                                    rat.Pressure = ratings.Min(x => x.Pressure);
                                    rat.Humidity = ratings.Min(x => x.Humidity);
                                    rat.WindSpeed = ratings.Min(x => x.WindSpeed);
                                    rat.WindDirection = ratings.Min(x => x.WindDirection);
                                    rat.Fallout = ratings.Min(x => x.Fallout);
                                    rat.Snow = ratings.Min(x => x.Snow);
                                    rat.Cloud = ratings.Min(x => x.Cloud);
                                    rat.WeatherDescription = ratings.Min(x => x.WeatherDescription);
                                    result.Add(rat);
                                }
                            }
                        }

                        if (result == null)
                            result = new List<Rating>();

                        result.AddRange(ratings);
                    }
                }

                if (result == null)
                {
                    Logger.Log.Error(String.Format("An error occurred while trying to calculate rating in GetRatings method."));
                    return null;
                }
                else
                {
                    if (result.Count() > 0)
                    {
                        return result;
                    }
                    else
                    {
                        Logger.Log.Error(String.Format("An error occurred while trying to calculate rating in GetRatings method"));
                        return null;
                    }
                }
            }
        }

        private static int GetRatingsValue(int maxDifference, int forecastDifferense)
        {
            if (maxDifference == 0)
                return 0;
            return (int)Math.Round(topOfTheRating - (double)(forecastDifferense * topOfTheRating / maxDifference));
        }
        private static int GetRatingsValue(double maxDifference, double forecastDifferense)
        {
            if (maxDifference == 0)
                return 0;
            return (int)Math.Round(topOfTheRating - (forecastDifferense * topOfTheRating / maxDifference));
        }
    }
}