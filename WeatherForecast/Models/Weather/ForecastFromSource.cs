using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
//using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using WeatherForecast.Models.WeatherSources;

namespace WeatherForecast.Models.Weather
{
    public class ForecastFromSource : WeatherCondition
    {
        public DateTime forecastCreating { get; set; } // date and time the forecast was created
        public WeatherSource source { get; set; }    // source - site that provides forecast data
    }
}