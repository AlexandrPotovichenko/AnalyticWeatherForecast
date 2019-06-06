using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeatherForecast.Models.Weather
{
    public class OneDayAstro
    {
        public int Id { get; set; }
        public DateTime? Sunrise { get; internal set; }
        public DateTime? Sunset { get; internal set; }
        public DateTime? Moonrise { get; internal set; }
        public DateTime? Moonset { get; internal set; }

        public OneDayAstro() { }
        public OneDayAstro(DateTime? sunrise, DateTime? sunset, DateTime? moonrise, DateTime? moonset)
        {
            // sunrise == DateTime.MinValue
            this.Sunrise = sunrise;
            this.Sunset = sunset;
            this.Moonrise = moonrise;
            this.Moonset = moonset;
        }
    }
}