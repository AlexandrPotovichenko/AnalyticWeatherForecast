using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using WeatherForecast.Models.Context;
using WeatherForecast.Models.Log;
using WeatherForecast.Models.Weather;
using WeatherForecast.Models.WeatherSources;

namespace WeatherForecast.App_Start
{
    public class Scheduler
    {
        public static void Start()
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();

            //  //////////////////////////////////////////setting up the execution to get weather forecasting according to the schedule////////////////////////////////////////////////////////////////////////////////////////

            //// determine the first time point, which has not already passed. according time zones   2,5,8,11,14,17,20,23

            int hours = DateTime.Now.ToUniversalTime().Hour;//UTC 
                                                            //if (hours < 0)
                                                            //    hours += 24;
            if (hours % 3 > 0)
                hours = hours - hours % 3; // hours are already passed.
            hours += 3;// UTC & Nex is +3 hour 

            DateTime startForecastShed = DateTime.Now.ToUniversalTime().Date.AddHours(hours).ToLocalTime();

            IJobDetail jobForecasts = JobBuilder.Create<WeatherSourceContainer>()
             .WithIdentity("forecasts")
             .Build();
            // create a trigger
            ITrigger triggerForecasts;
            // configure the execution of the action
            double differenceInMinutes = (DateTime.Now - startForecastShed).TotalMinutes;
            if (differenceInMinutes > 0 && differenceInMinutes <= 15) //should work no later than 15 min. after scheduled
            {
                triggerForecasts = TriggerBuilder.Create()
           .WithSimpleSchedule(s => s.WithIntervalInHours(3).RepeatForever()).StartAt(new DateTimeOffset(DateTime.Now)).Build();
            }
            else
            {
                triggerForecasts = TriggerBuilder.Create()
           .WithSimpleSchedule(s => s.WithIntervalInHours(3).RepeatForever()).StartAt(new DateTimeOffset(startForecastShed)).Build();
            }

            scheduler.ScheduleJob(jobForecasts, triggerForecasts);

            Logger.Log.Debug("Starting the schedule forecast in" + DateTime.Now.ToUniversalTime().Date.AddHours(hours).ToLocalTime());

            ///////////////////////////////////////////////////////////setting up the execution to get current weather according to the schedule/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            IJobDetail jobCurrentWeather = JobBuilder.Create<CurrentWeatherSource>()
         .WithIdentity("currentWeather")
         .Build();
            // create a trigger
            // configure the execution of the action
            ITrigger triggerCurrentWeather = TriggerBuilder.Create()
           .WithSimpleSchedule(s => s.WithIntervalInMinutes(5).RepeatForever()).StartNow().Build();
            scheduler.ScheduleJob(jobCurrentWeather, triggerCurrentWeather);

            Logger.Log.Debug("Starting the schedule current weather in " + DateTime.Now.ToString());

            /////////////////////////////////////////////////////////setting up the execution to get astro according to the schedule////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            IJobDetail jobAstro = JobBuilder.Create<Astro>()
          .WithIdentity("astro")
          .Build();

            // create a trigger
            // configure the execution of the action
            ITrigger triggerAstro = TriggerBuilder.Create()
           .WithSimpleSchedule(s => s.WithIntervalInHours(24).RepeatForever()).StartAt(new DateTimeOffset(DateTime.Now.Hour<2?DateTime.Now:DateTime.Today.AddDays(1).AddMinutes(30))).Build(); 
            scheduler.ScheduleJob(jobAstro, triggerAstro);

            Logger.Log.Debug("Starting the schedule astro in " + triggerAstro.StartTimeUtc.ToString());

            //execution of the actions
            scheduler.Start();

        }
    }
}