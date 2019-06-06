using System;
using System.Collections.Generic;
using WeatherForecast.Models.AnalyticForecast;
using WeatherForecast.Models.Log;
using WeatherForecast.Models.Weather;
using WeatherForecast.Models.WeatherSources;

namespace WeatherForecast.Models.ViewModels
{
    public class AnalyticWeatherForecastView
    {

        public DateTime LatestForecastUpdate { get; internal set; }
        public List<OneDayForecast> AllForecasts { get; internal set; }
        public  WeatherCondition CurrentWeather { get; internal set; }

        public AnalyticWeatherForecastView()
        {
            AllForecasts = AnalyticForecastFactory.LatestForecasts;
            CurrentWeather = CurrentWeatherSource.CurrentWeather;
            LatestForecastUpdate = AnalyticForecastFactory.LatestForecastUpdate;
        }
        public static OneDayForecast GetForecastForDay(int dayIndex)
        {
            if (dayIndex < 0 || dayIndex > AnalyticForecastFactory.LatestForecasts.Count - 1)
            {
                if(AnalyticForecastFactory.LatestForecasts.Count==0)
                {
                    Logger.Log.Debug("LatestForecasts.Count = 0!");
                }
                else { 
                Logger.Log.Error("An error occurred while trying to get forecast for day in AnalyticWeatherForecastView");
                }
                return new OneDayForecast();
            }
            return AnalyticForecastFactory.LatestForecasts[dayIndex];

        }

    }
}