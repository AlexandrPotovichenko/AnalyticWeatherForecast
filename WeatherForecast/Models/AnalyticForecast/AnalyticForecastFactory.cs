using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using WeatherForecast.Models.Context;
using WeatherForecast.Models.Log;
using WeatherForecast.Models.Weather;
using WeatherForecast.Models.WeatherSources;

namespace WeatherForecast.Models.AnalyticForecast
{
    public static class AnalyticForecastFactory
    {
        public static DateTime LatestForecastUpdate { get; internal set; }
        public static List<OneDayForecast> LatestForecasts { get; internal set; }

        static AnalyticForecastFactory()
        {
            using (WeatherContext db = new WeatherContext())
            {
                LatestForecasts = new List<OneDayForecast>();

                if (db.OneDayAnalyticForecast.Count() > 0)
                {
                    List<OneDayForecast> ForecastsFromDB = db.OneDayAnalyticForecast.Include("Forecasts.description.Icon").Include("description").Include("description.Icon").Include("astro").ToList();// db.OneDayAnalyticForecast.Include(f => f.Forecasts).ToList();//.Include(f => f.description.Icon).ToList();

                    LatestForecasts = ForecastsFromDB.Where(x => x.date >= DateTime.Today).ToList(); 
                    foreach (var oneDay in LatestForecasts)
                    {
                        oneDay.Forecasts = oneDay.Forecasts.Where(x => x.date >= DateTime.Now.ToUniversalTime()).OrderBy(x=>x.date).ToList();
                    }
                    LatestForecastUpdate = db.WeatherConditions.Min(x => x.date);
                }
            }
        }
        public static List<OneDayForecast> GetLatestForecast()
        {
            lock (LatestForecasts)
            {
                return LatestForecasts.ToList();
            }
        }
        internal static void UpDateForecasts(IEnumerable<ForecastFromSource> latestForecastsFromSource, IEnumerable<DailyIcon> dailyIcons)
        {
            DateTime fullHours = DateTime.Today.AddHours(DateTime.Now.Hour);
            try
            {
                using (WeatherContext db = new WeatherContext())
                {
                    WeatherSource[] source = latestForecastsFromSource.Select(x => x.source).Distinct().ToArray();
                    foreach (var s in source)
                    {
                        db.WeatherSources.Attach(s);
                    }
                    // Group ratings by the farness of the forecast
                    var result = from f in db.Ratings
                                 group f by DbFunctions.DiffHours(f.ForecastDate, f.ForecastFor).HasValue ? DbFunctions.DiffHours(f.ForecastDate, f.ForecastFor).Value : 0 into g//farness
                                 select g;

                    result.ToList();
                    //select best ratings by farness
                    var topOfRatings = (from g in result

                                        select new
                                        {
                                            airTemperature = (from p in g
                                                              group p.AirTemperature by p.Source into gr
                                                              select new
                                                              {
                                                                  val = gr.Sum(),
                                                                  source = gr.Key
                                                              }).OrderByDescending(x => x.val).FirstOrDefault().source,
                                            iconDescription = (from p in g
                                                               group p.WeatherDescription by p.Source into gr
                                                               select new
                                                               {
                                                                   val = gr.Sum(),
                                                                   source = gr.Key
                                                               }).OrderByDescending(x => x.val).FirstOrDefault().source,
                                            pressure = (from p in g
                                                        group p.Pressure by p.Source into gr
                                                        select new
                                                        {
                                                            val = gr.Sum(),
                                                            source = gr.Key
                                                        }).OrderByDescending(x => x.val).FirstOrDefault().source,
                                            humidity = (from p in g
                                                        group p.Humidity by p.Source into gr
                                                        select new
                                                        {
                                                            val = gr.Sum(),
                                                            source = gr.Key
                                                        }).OrderByDescending(x => x.val).FirstOrDefault().source,
                                            WindSpeed = (from p in g
                                                         group p.WindSpeed by p.Source into gr
                                                         select new
                                                         {
                                                             val = gr.Sum(),
                                                             source = gr.Key
                                                         }).OrderByDescending(x => x.val).FirstOrDefault().source,
                                            WindDirection = (from p in g
                                                             group p.WindDirection by p.Source into gr
                                                             select new
                                                             {
                                                                 val = gr.Sum(),
                                                                 source = gr.Key
                                                             }).OrderByDescending(x => x.val).FirstOrDefault().source,
                                            Fallout = (from p in g
                                                       group p.Fallout by p.Source into gr
                                                       select new
                                                       {
                                                           val = gr.Sum(),
                                                           source = gr.Key
                                                       }).OrderByDescending(x => x.val).FirstOrDefault().source,
                                            Snow = (from p in g
                                                    group p.Snow by p.Source into gr
                                                    select new
                                                    {
                                                        val = gr.Sum(),
                                                        source = gr.Key
                                                    }).OrderByDescending(x => x.val).FirstOrDefault().source,
                                            Cloud = (from p in g
                                                     group p.Cloud by p.Source into gr
                                                     select new
                                                     {
                                                         val = gr.Sum(),
                                                         source = gr.Key
                                                     }).OrderByDescending(x => x.val).FirstOrDefault().source,
                                            Farness = g.Key
                                        }).AsEnumerable();// database query ends here, the rest is a query in memory


                    var resultForecast = (from rat in topOfRatings// select forecasts by best ratings
                                          select new
                                          {
                                              rating = rat,
                                              forecastse = (from lf in latestForecastsFromSource
                                                            where fullHours.AddHours(rat.Farness) == lf.date
                                                            select lf).ToList()
                                          }).AsEnumerable();
                    IEnumerable<WeatherCondition> res = new List<WeatherCondition>();
                    try { 
                     res = from obj in resultForecast
                              where obj.forecastse.Count > 0
                              select new WeatherCondition
                              {
                                  date = fullHours.AddHours(obj.rating.Farness),
                                  airTemperature = obj.forecastse.Where(x => x.source == obj.rating.airTemperature).FirstOrDefault().airTemperature,
                                  isDay = obj.forecastse.Where(x => x.source == obj.rating.iconDescription).FirstOrDefault().isDay,
                                  description = obj.forecastse.Where(x => x.source == obj.rating.iconDescription).FirstOrDefault().description,
                                  pressure = obj.forecastse.Where(x => x.source == obj.rating.pressure).FirstOrDefault().pressure,
                                  humidity = obj.forecastse.Where(x => x.source == obj.rating.humidity).FirstOrDefault().humidity,
                                  windSpeed = obj.forecastse.Where(x => x.source == obj.rating.WindSpeed).FirstOrDefault().windSpeed,
                                  windDirection = obj.forecastse.Where(x => x.source == obj.rating.WindDirection).FirstOrDefault().windDirection,
                                  rain = obj.forecastse.Where(x => x.source == obj.rating.Fallout).FirstOrDefault().rain,
                                  snow = obj.forecastse.Where(x => x.source == obj.rating.Snow).FirstOrDefault().snow,
                                  cloud = obj.forecastse.Where(x => x.source == obj.rating.Cloud).FirstOrDefault().cloud

                              };
                    }
                    catch(Exception ex)
                    {
                        Logger.Log.Error("", ex);
                    }
                    ///////////////////////////////////////////////////////////////////// code for description////////////////////////////////////////////////////

                    var TopOfDescriptionRating = from g in topOfRatings
                                                 select new
                                                 {
                                                     date = fullHours.AddHours(g.Farness).Date,
                                                     sourse = g.iconDescription
                                                 };

                    var SourcesDescriptionRating = from g in TopOfDescriptionRating
                                                   group g by g.date into gr
                                                   select new
                                                   {
                                                       date = gr.Key,
                                                       sourses = gr.GroupBy(x => x.sourse).OrderByDescending(x => x.Count())
                                                   };
                    var SourceDescriptionRating = from g in SourcesDescriptionRating
                                                  select new
                                                  {
                                                      date = g.date,
                                                      source = g.sourses.FirstOrDefault().Key
                                                  };
                    ///////////////////////////////////////////////////////////////////// end code for description////////////////////////////////////////////////////

                    lock (LatestForecasts)
                    {
                        LatestForecasts.Clear();

                        // breakdown by day

                        var resGroupByDay = from forecast in res
                                            group forecast by forecast.date.Date into forecastOnDay
                                            select forecastOnDay;

                        foreach (var forecastsOnDay in resGroupByDay.OrderBy(x => x.Key))
                        {
                            OneDayForecast f = new OneDayForecast();
                            f.date = forecastsOnDay.Key;
                            f.astro = Astro.GetAstroByDate(forecastsOnDay.Key);
                            WeatherSource oneDaySource = SourceDescriptionRating.Where(x => x.date == forecastsOnDay.Key).FirstOrDefault().source;
                            f.description = dailyIcons.Where(x => x.forecastFor == forecastsOnDay.Key && x.source == oneDaySource).FirstOrDefault().description;
                            f.Forecasts.AddRange(forecastsOnDay.OrderBy(x => x.date));
                            LatestForecasts.Add(f);
                        }
                        LatestForecastUpdate = DateTime.Now;
                    }

                    //delete old data from the table OneDayAnalyticForecast end WeatherCondition
                    try
                    {                    
                        foreach (var forecast in db.OneDayAnalyticForecast.Include("Forecasts"))
                        {
                            db.WeatherConditions.RemoveRange(forecast.Forecasts);
                            db.OneDayAnalyticForecast.Remove(forecast);
                        }
                        db.SaveChanges();
                    
                    }
                    catch (Exception ex)
                    {
                        Logger.Log.Error("An error occurred while trying to delete OneDayAnalyticForecast", ex);
                    }

                    try 
                    {
                        var descriptions = LatestForecasts.Select(f => f.description).Distinct();

                        var icons = LatestForecasts.Select(f => f.description.Icon).Distinct();

                        var astros = LatestForecasts.Select(f => f.astro).Distinct();
                        foreach (var astro in astros)
                        {
                            if (astro!=null)
                            {
                               if(db.Entry(astro).State == EntityState.Detached)
                                {
                                    db.AstroByDays.Attach(astro);
                                }
                                    
                            }
                        }



                        foreach (var forecast in LatestForecasts)
                        {
                            foreach (var condition in forecast.Forecasts)
                            {
                                if (!descriptions.Contains(condition.description))
                                {
                                    if (db.Entry(condition.description).State == EntityState.Detached)
                                        db.WeatherDescriptions.Attach(condition.description);
                                }

                                if (!icons.Contains(condition.description.Icon))
                                {
                                    if (db.Entry(condition.description.Icon).State == EntityState.Detached)
                                        db.WeatherIcons.Attach(condition.description.Icon);
                                }
                            }

                            foreach(var descript in descriptions)
                            {
                                if (db.Entry(descript).State == EntityState.Detached)
                                    db.WeatherDescriptions.Attach(descript);
                            }
                            foreach (var icon in icons)
                            {
                                if (db.Entry(icon).State == EntityState.Detached)
                                    db.WeatherIcons.Attach(icon);
                            }
                            


                        }
                        foreach (var forecast in LatestForecasts)
                        {
                            db.OneDayAnalyticForecast.Add(forecast);
                        }

                            db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        Logger.Log.Error("An error occurred while trying to save analytic forecasts in AnalyticForecastFactory", ex);
                    }

                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error("An error occurred while trying to create analytic forecasts", ex);
            }
        }
    }
}