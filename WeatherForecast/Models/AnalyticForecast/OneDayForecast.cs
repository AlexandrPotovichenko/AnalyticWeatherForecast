using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WeatherForecast.Models.AnalyticForecast;
using WeatherForecast.Models.Weather;

namespace WeatherForecast.Models.AnalyticForecast
{
    public class OneDayForecast
    {
        public int Id { get; set; }
        public DateTime date { get; internal set; }
        public WeatherDescription description{ get; internal set; }

        public OneDayAstro astro { get; internal set; }
        public List<WeatherCondition> Forecasts { get; internal set; }

        public OneDayForecast()
        {
            Forecasts = new List<WeatherCondition>();
        }
        public string GetMaxTemperature()
        {
            if (Forecasts != null && Forecasts.Count > 0)
            { 
                double maxTemp = Forecasts.Max(x => x.airTemperature);

                return "max " + (maxTemp > 0 ? "+" + Math.Round(maxTemp, 1).ToString("F1") : Math.Round(maxTemp, 1).ToString("F1")).Replace(",",".")+ " °C";
            }
            else
                return "max " + "-- --";
        }
        public string GetMinTemperature()
        {
            if (Forecasts != null && Forecasts.Count > 0)
            {
                double minTemp = Forecasts.Min(x => x.airTemperature);

                return "min " + (minTemp > 0 ? "+" + Math.Round(minTemp, 1).ToString("F1") : Math.Round(minTemp, 1).ToString("F1")).Replace(",", ".") + " °C";
            }
            else
                return "min " + "-- --";
        }
        public string GetFallout()
        {
            if (Forecasts != null && Forecasts.Count > 0)
            {
                double tempFallout = Forecasts.Sum(x => x.rain) + Forecasts.Sum(x => x.snow);

                return Math.Round(tempFallout, 1).ToString("F1") + " mm";
            }
            else
                return "0.0 mm";
        }

    }

}