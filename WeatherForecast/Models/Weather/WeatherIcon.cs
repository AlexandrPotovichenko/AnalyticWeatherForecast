using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WeatherForecast.Models.Context;
using WeatherForecast.Models.Log;

namespace WeatherForecast.Models.Weather
{
    public class WeatherIcon // Represents a picture of weather conditions.
    {
        internal static readonly string PathBigIcon = "~/Content/Images/BigDailyIcon";
        internal static readonly string PathSmallDailyIcon = "~/Content/Images/DailyIcon";
        internal static readonly string PathNightIcon = "~/Content/Images/NightIcon";

        public int Id { get; set; }
        // Name of icon in our system is a result of transformasion the icon of weather from source into our icons system.  
        public string Name { get; set; }
        // Flag of night icon if it there is.
        public bool IsNightImageExist { get; set; }

        public WeatherIcon() {  }
        internal WeatherIcon(string name, bool nightImageExist)
        {
            Name = name;
            IsNightImageExist = nightImageExist;
        }


        static internal string GetBigIconURL(string description)
        {
            return PathBigIcon + "/" +  GetWeatherIcon(description).Name;
        }
   
            // This function searches for the appropriate weather icon.
            static internal string GetIconURL(string description, bool isDay)
        {

            WeatherIcon conditionOfClouds = GetWeatherIcon(description);
          
                    if(!isDay && conditionOfClouds.IsNightImageExist)
                    
                    {
                    return PathNightIcon + "/" + conditionOfClouds.Name;      //return the path to the folder with the night picture
            }

                return PathSmallDailyIcon + "/" + conditionOfClouds.Name;    //return the path to the folder with the daily picture

        }
        public  string GetIconURL(bool isDay)
        {
            if (!isDay && IsNightImageExist)

            {
                return PathNightIcon + "/" + Name;       //return the path to the folder with the night picture
            }

            return PathSmallDailyIcon + "/" + Name;     //return the path to the folder with the daily picture
        }
        static private WeatherIcon GetWeatherIcon(string description) // Returns the weathers icon based on the phrase from sources.
        {
            using (WeatherContext db = new WeatherContext())
            {
                var result = from descrip in db.WeatherDescriptions
                         join icon in db.WeatherIcons
                         on descrip.Icon equals icon// into joined
                         where (descrip.Description == description)
                         select icon;
            if (result.Count() == 0)
                {
                    var NAresult = from descrip in db.WeatherDescriptions
                                 join icon in db.WeatherIcons
                                 on descrip.Icon equals icon// into joined
                                 where (descrip.Description == "na")
                                 select icon;
                    Logger.Log.Error(String.Format("An error occurred while trying to get icon with description: {0};", description));
                    return NAresult.First();
                    //return null;
                }
                return result.First();
            }
        }
    }
}