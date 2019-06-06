using WeatherForecast.Models.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WeatherForecast.Models.Context;
using WeatherForecast.Models.ViewModels;
using WeatherForecast.Models.Weather;
using WeatherForecast.Models.WeatherSources;

namespace WeatherForecast.Models.WeatherForecast
{


    public class Rating  
    {
        public int Id { get; set; }
        public DateTime ForecastFor { get; set; } // time for 
        public DateTime ForecastDate { get; set; }// forecast date
        public WeatherSource Source { get; set; }
        public int AirTemperature { get; set; }
        public int Pressure { get; set; }
        public int Humidity { get; set; }
        public int WindSpeed { get; set; }
        public int WindDirection { get; set; }
        public int Fallout { get; set; }
        public int Snow { get; set; }
        public int Cloud { get; set; }
        public int WeatherDescription { get; set; }

        public const double absoluteZeroTemperature = -273.15;

    }
}