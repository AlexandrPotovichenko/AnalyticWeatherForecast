using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WeatherForecast.Models.Weather;

namespace WeatherForecast.Models.AnalyticForecast
{
    public class ThreeHoursForecast
    {
            public WeatherCondition condition { get; set; }
            public string IconURL { get; set; }
    }
}