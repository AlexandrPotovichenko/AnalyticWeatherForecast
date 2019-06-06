using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Data.Entity;
using WeatherForecast.Models.Context;
using WeatherForecast.Models.Log;
using System.Threading;

namespace WeatherForecast.Models.Weather
{
    public class WeatherDescription // Represents a description of the weather conditions.
    {
        public int Id { get; set; }
        public string Description { get; set; } // Condition phrases(can be  Light Drizzle, Heavy Drizzle and etc. ).
        public virtual WeatherIcon Icon { get; set; } // Represents a picture of weather conditions.

        private static List<WeatherDescription> internalListOfDescription = null;
        static WeatherDescription()
        {
            using (WeatherContext db = new WeatherContext())
            {
                internalListOfDescription = new List<WeatherDescription>();
            }
        }
        static public WeatherDescription GetWeatherDescription(string description) // Returns the weathers description based on the phrase from sources.
        {
            var result = from descrip in internalListOfDescription
                         where (descrip.Description == description)
                         select descrip;
            if (result.Count() == 0)
            {
                if (internalListOfDescription.Count() == 0)
                {
                    using (WeatherContext db = new WeatherContext())
                    {
                        internalListOfDescription = db.WeatherDescriptions.Include("Icon").ToList();
                    }
                    return GetWeatherDescription(description);
                }
                Logger.Log.Error(String.Format("An error occurred while trying to get weather description from description: {0}; internalListOfDescription : {1}", description, internalListOfDescription.Count().ToString()));
                return null;
            }
            return result.First();
        }

    }
}