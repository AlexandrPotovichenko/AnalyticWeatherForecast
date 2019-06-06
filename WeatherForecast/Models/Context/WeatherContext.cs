using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Web;
using WeatherForecast.Models.AnalyticForecast;
using WeatherForecast.Models.Weather;
using WeatherForecast.Models.WeatherForecast;
using WeatherForecast.Models.WeatherSources;

namespace WeatherForecast.Models.Context
{
    public class WeatherContext : DbContext
    {
        public DbSet<ForecastFromSource> Forecasts { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<WeatherIcon> WeatherIcons { get; set; }
        public DbSet<WeatherDescription> WeatherDescriptions { get; set; }
        public DbSet<WeatherSource> WeatherSources { get; set; }
        public DbSet<OneDayAstro> AstroByDays { get; set; }
        public DbSet<OneDayForecast> OneDayAnalyticForecast { get; set; }
        public DbSet<WeatherCondition> WeatherConditions { get; set; }
    }
}