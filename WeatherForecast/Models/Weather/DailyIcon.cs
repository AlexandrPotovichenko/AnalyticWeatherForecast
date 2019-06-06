using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WeatherForecast.Models.WeatherSources;

namespace WeatherForecast.Models.Weather
{
    public class DailyIcon
    {
        public DateTime forecastFor { get; set; }
        // The source of weather forecast.
        public WeatherSource source { get; set; }
        public WeatherDescription description { get; set; }

    }
}