using Newtonsoft.Json.Linq;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using WeatherForecast.Models.Weather;
using System.Threading;
using WeatherForecast.Models.Context;
using System.Threading.Tasks;
using WeatherForecast.Models.WeatherForecast;
using WeatherForecast.Models.Log;
using WeatherForecast.Models.ForecastRating;
using WeatherForecast.Models.AnalyticForecast;
using System.Data.Entity;

namespace WeatherForecast.Models.WeatherSources
{
    public class WeatherSourceContainer : IJob
    {
        private int countRatingsForStorage = 20;
        private WeatherCondition weatherConditionForRating; //the variable necessity of which occurs when waiting from the source of information for more than 15 minutes.
        private List<ForecastFromSource> latestForecasts;
        private List<WeatherSource> weatherSources = new List<WeatherSource>();

        public WeatherSourceContainer()
        {
            using (WeatherContext db = new WeatherContext())
            {
                weatherSources = db.WeatherSources.ToList();          
                if (weatherSources.Count() == 0)
                {
                    Logger.Log.Debug("An error occurred while trying to get weather sources from db");
                }
            }

            weatherConditionForRating = new WeatherCondition();
            latestForecasts = new List<ForecastFromSource>();
        }

        public void Execute(IJobExecutionContext context)
        {
            Logger.Log.Debug("Starting of forecasts on schedule in " + DateTime.Now.ToString());
            List<DailyIcon> dailyIcons = new List<DailyIcon>();

            Task updateCurWeatherTask = new Task(UpdateCurrentWeatherForRating);
            updateCurWeatherTask.Start();

            Task<IEnumerable<ForecastFromSource>>[] tasks = new Task<IEnumerable<ForecastFromSource>>[weatherSources.Count()];

            int index = 0;
            foreach (WeatherSource source in weatherSources)
            {
                tasks[index++] = new Task<IEnumerable<ForecastFromSource>>(() => source.GetForecasts(DateTime.Now.AddHours(1), ref dailyIcons));
            }
            foreach (var t in tasks)
            {
                t.Start();
                Thread.Sleep(1000 * 60 * 2);// waiting two minute
            }

            Task.WaitAll(tasks); // waiting for completion of tasks

            foreach (var t in tasks)// save forecasts
            {
                try { 
                if (t.Result != null)
                {     
                    SaveForecasts(t.Result);
                    latestForecasts.AddRange(t.Result);
                    Logger.Log.Debug("Successfully saved the forecast " + t.Result.First().source.Name);
                }
                else
                { 
                    Logger.Log.Error("An error occurred while trying to save forecasts");
                }
                }
                catch (Exception ex)
                {
                    Logger.Log.Error("An error occurred while trying to save forecasts", ex);
                }      
            }

            updateCurWeatherTask.Wait();//waiting for the task of getting current weather for the test hour

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////// Start rating ////////////////////////////////////////////////////////////
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            Logger.Log.Debug("Starting rating process at " + DateTime.Now.ToString());

            var rat = RaitingFactory.GetRatings(weatherConditionForRating);
            if (rat != null)
                SaveRatings(rat);
            else
                Logger.Log.Error("An error occurred while trying to save ratings");

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////// Creating "Selective foreсast" ////////////////////////////////////////////////////////////
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


            Logger.Log.Debug("Starting rating process retriving a best rating at " + DateTime.Now.ToString());

            if (rat != null && rat.Count() > 0)// generating forecasts based on ratings and forecasts from sources
            {
                AnalyticForecastFactory.UpDateForecasts(latestForecasts, dailyIcons);
            }
            DeleteObsoletedData();
        }
        private void UpdateCurrentWeatherForRating()
        {
            DateTime timeForwaiting = DateTime.Now.Date.AddHours(DateTime.Now.Hour);
            WeatherCondition wcOld = CurrentWeatherSource.CurrentWeather;
            WeatherCondition wcNew = CurrentWeatherSource.CurrentWeather;

            while (wcNew == null || wcNew.date.Minute != 00)
            {
                if (wcOld != null)
                {
                    int currentcurrentDifferenceNew = Math.Abs((wcNew.date - timeForwaiting).Minutes);
                    int currentcurrentDifferenceOld = Math.Abs((wcOld.date - timeForwaiting).Minutes);
                    if (currentcurrentDifferenceNew <= 3)
                    {
                        weatherConditionForRating = wcNew;
                        weatherConditionForRating.date = timeForwaiting;
                        return;
                    }
                    if (currentcurrentDifferenceOld <= 3)
                    {
                        weatherConditionForRating = wcOld;
                        weatherConditionForRating.date = timeForwaiting;
                        return;
                    }
                    if (currentcurrentDifferenceNew < currentcurrentDifferenceOld)
                    {
                        wcOld = wcNew;
                    }
                }
                Logger.Log.Debug("Starting UpdateCurrentWeatherForRating in " + DateTime.Now.ToString() + " time of current weather - " + (CurrentWeatherSource.CurrentWeather == null ? "  -  null  -  " : CurrentWeatherSource.CurrentWeather.date.ToString()));
                Thread.Sleep(1000 * 60);// wait a minute
                wcNew = CurrentWeatherSource.CurrentWeather;
            }

            Logger.Log.Debug("Stop UpdateCurrentWeatherForRating in " + DateTime.Now.ToString() + " time of current weather - " + (CurrentWeatherSource.CurrentWeather == null ? "  -  null  -  " : CurrentWeatherSource.CurrentWeather.date.ToString()));

            weatherConditionForRating = wcNew;
            weatherConditionForRating.date = timeForwaiting;
        }

        private void SaveForecasts(IEnumerable<ForecastFromSource> forecasts)
        {
            using (WeatherContext db = new WeatherContext())
            {
                db.WeatherSources.Attach(weatherSources.Where(x => x.Name == forecasts.First().source.Name).First());

                foreach (var d in forecasts.Select(x => x.description).Distinct())
                {
                    db.WeatherDescriptions.Attach(d);
                }
 
                db.Forecasts.AddRange(forecasts); 
                db.SaveChanges();
            }
        }

        private void SaveRatings(IEnumerable<Rating> ratings)
        {
            if (ratings == null)
                return;
            try
            {
                using (WeatherContext db = new WeatherContext())
                {
                    foreach (var source in ratings)
                    {
                        db.WeatherSources.Attach(source.Source);
                    }
                    db.Ratings.AddRange(ratings);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error("An error occurred while trying to save ratings in WeatherSourceContainer", ex);
            }
        }
        private void DeleteObsoletedData()
        {

            using (WeatherContext db = new WeatherContext())
            {
                DateTime UTCtime = DateTime.Now.ToUniversalTime();
                var forecastForDeleting = from f in db.Forecasts
                                          where f.date < UTCtime
                                          select f;
                foreach (var f in forecastForDeleting)
                {
                    db.Forecasts.Remove(f);
                }

                var ratingBySource = db.Ratings.GroupBy(x => x.Source);
                foreach (var gr in ratingBySource)
                {
                    var ratingGroupForDeleting = from f in gr
                                                 group f by (f.ForecastDate - f.ForecastFor) into g//farness
                                                 select g.OrderByDescending(x => x.ForecastDate).Skip(countRatingsForStorage);
                    foreach (var deletingGroup in ratingGroupForDeleting)
                    {
                        db.Ratings.RemoveRange(deletingGroup);
                    }
                }
                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Logger.Log.Error("An error occurred while trying to Delete Obsoleted forecasts and ratings in DeleteObsoletedData in WeatherSourceContainer", ex);
                }
            }
        }
    }
}