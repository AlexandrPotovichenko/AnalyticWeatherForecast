using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WeatherForecast.Models.Weather;
using WeatherForecast.Models.WeatherSources;

namespace WeatherForecast.Models.Context
{
    public class WeatherContextInitializer : CreateDatabaseIfNotExists<WeatherContext> 
    {
        protected override void Seed(WeatherContext db)
        {


            db.WeatherSources.Add(new OpenWeatherMap());
            db.WeatherSources.Add(new WeatherBit());
            db.WeatherSources.Add(new Wunderground());

            WeatherIcon cloudiness;
            List<WeatherDescription> description;

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            description = new List<WeatherDescription>();
            ///
        /////////////////////////////////////////////////////// CLOUDS /////////////////////////////////////////////////////////////////////////

            cloudiness = new WeatherIcon("ClearSky.png", true);//Clear
            db.WeatherIcons.Add(cloudiness);
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "clear sky", Icon = cloudiness }); //OPEN & WeatherBit
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "clear", Icon = cloudiness });  //wunderground & Apixu
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "sunny", Icon = cloudiness });  //Apixu
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "calm", Icon = cloudiness });  //OPEN
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "sky is clear", Icon = cloudiness });  //OPEN


            cloudiness = new WeatherIcon("PartlyCloudy.png", true);//Partly Cloudy
            db.WeatherIcons.Add(cloudiness);
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "few clouds", Icon = cloudiness }); //OPEN & WeatherBit
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "partly cloudy", Icon = cloudiness });    //wunderground & Apixu

            cloudiness = new WeatherIcon("MostlyCloudy.png", true);//Mostly Cloudy
            db.WeatherIcons.Add(cloudiness);
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "scattered clouds", Icon = cloudiness }); //OPEN & WeatherBit
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "broken clouds", Icon = cloudiness }); //OPEN & WeatherBit
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "mostly cloudy", Icon = cloudiness });    //wunderground
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "scattered clouds", Icon = cloudiness });    //wunderground
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "cloudy", Icon = cloudiness });    //Apixu

            cloudiness = new WeatherIcon("Overcast.png", false);//Overcast clouds
            db.WeatherIcons.Add(cloudiness);
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "overcast clouds", Icon = cloudiness }); //OPEN & WeatherBit
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "overcast", Icon = cloudiness });    //wunderground & Apixu


            /////////////////////////////////////////////////////// Atmosphere /////////////////////////////////////////////////////////////////////////

            cloudiness = new WeatherIcon("Mist.png", false);//Mist
            db.WeatherIcons.Add(cloudiness);
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "mist", Icon = cloudiness }); //OPEN & WeatherBit & wunderground & Apixu
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "smoke", Icon = cloudiness }); //OPEN & WeatherBit  & wunderground
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "fog", Icon = cloudiness }); //OPEN & WeatherBit & wunderground & Apixu
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "haze", Icon = cloudiness }); //OPEN & WeatherBit 
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "freezing fog", Icon = cloudiness }); //&WeatherBit & wunderground & Apixu

            //db.CloudinessDescriptions.Add(new DescriptionOfClouds() { Description = "freezing fog", CloudState = cloudiness });    // & wunderground
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "light freezing fog", Icon = cloudiness });    //wunderground
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "heavy freezing fog", Icon = cloudiness });    //wunderground

            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "patches of fog", Icon = cloudiness });    //wunderground
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "shallow fog", Icon = cloudiness });    //wunderground
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "partial fog", Icon = cloudiness });    //wunderground

            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "light mist", Icon = cloudiness });    //wunderground
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "heavy mist", Icon = cloudiness });    //wunderground

            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "light fog", Icon = cloudiness });    //wunderground
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "heavy fog", Icon = cloudiness });    //wunderground

            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "fog patches", Icon = cloudiness });    //wunderground
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "light fog patches", Icon = cloudiness });    //wunderground
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "heavy fog Patches", Icon = cloudiness });    //wunderground

            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "light smoke", Icon = cloudiness });    //wunderground
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "heavy smoke", Icon = cloudiness });    //wunderground

            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "light haze", Icon = cloudiness });    //wunderground
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "heavy haze", Icon = cloudiness });    //wunderground


            /////////////////////////////////////////////////////// Drizzle /////////////////////////////////////////////////////////////////////////


            cloudiness = new WeatherIcon("Drizzle.png", false);//Drizzle
            db.WeatherIcons.Add(cloudiness);
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "drizzle", Icon = cloudiness }); //OPEN & WeatherBit & wunderground
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "light intensity drizzle", Icon = cloudiness }); //OPEN 
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "heavy intensity drizzle", Icon = cloudiness }); //OPEN

            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "light drizzle", Icon = cloudiness });    //WeatherBit & wunderground & Apixu
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "patchy light drizzle", Icon = cloudiness });    //Apixu
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "heavy drizzle", Icon = cloudiness });    //WeatherBit & wunderground
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "light rain mist", Icon = cloudiness });    //wunderground
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "light spray", Icon = cloudiness });    //wunderground
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "spray", Icon = cloudiness });    //wunderground


            /////////////////////////////////////////////////////// Rain /////////////////////////////////////////////////////////////////////////


            cloudiness = new WeatherIcon("LightRain.png", true);//Light Rain
            db.WeatherIcons.Add(cloudiness);
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "light rain", Icon = cloudiness }); //OPEN & WeatherBit & wunderground & Apixu
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "patchy light rain", Icon = cloudiness }); //Apixu
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "patchy rain possible", Icon = cloudiness }); //Apixu
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "light intensity drizzle rain", Icon = cloudiness }); //OPEN 
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "heavy spray", Icon = cloudiness });    //wunderground
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "chance of rain", Icon = cloudiness });    //wunderground


            cloudiness = new WeatherIcon("Rain.png", true);//Rain
            db.WeatherIcons.Add(cloudiness);

            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "moderate rain", Icon = cloudiness }); //OPEN & WeatherBit & Apixu
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "moderate rain at times", Icon = cloudiness }); //Apixu

            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "drizzle rain", Icon = cloudiness }); //OPEN
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "shower drizzle", Icon = cloudiness }); //OPEN
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "rain", Icon = cloudiness });    //wunderground
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "rain mist", Icon = cloudiness });    //wunderground
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "heavy intensity drizzle rain", Icon = cloudiness }); //OPEN
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "light rain showers", Icon = cloudiness });    //wunderground
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "light rain shower", Icon = cloudiness });    //Apixu
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "light shower rain", Icon = cloudiness });    //WeatherBit
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "light intensity shower rain", Icon = cloudiness }); //OPEN

            cloudiness = new WeatherIcon("HeavyRain.png", true);//Heavy Rain
            db.WeatherIcons.Add(cloudiness);

            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "heavy rain", Icon = cloudiness }); //WeatherBit & wunderground & Apixu
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "heavy rain at times", Icon = cloudiness }); //Apixu
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "heavy rain mist", Icon = cloudiness }); //wunderground
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "heavy intensity rain", Icon = cloudiness }); //OPEN
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "very heavy rain", Icon = cloudiness }); //OPEN
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "extreme rain", Icon = cloudiness }); //OPEN
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "shower rain and drizzle", Icon = cloudiness }); //OPEN
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "heavy shower rain and drizzle", Icon = cloudiness }); //OPEN
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "heavy intensity shower rain", Icon = cloudiness }); //OPEN
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "ragged shower rain", Icon = cloudiness }); //OPEN

            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "shower rain", Icon = cloudiness }); //OPEN & WeatherBit
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "rain showers", Icon = cloudiness }); //wunderground
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "heavy rain showers", Icon = cloudiness }); //wunderground
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "moderate or heavy rain shower", Icon = cloudiness }); //Apixu
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "torrential rain shower", Icon = cloudiness }); //Apixu
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "heavy shower rain", Icon = cloudiness }); //WeatherBit

            cloudiness = new WeatherIcon("FreezingRain.png", true);//Freezing Rain
            db.WeatherIcons.Add(cloudiness);

            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "freezing rain", Icon = cloudiness }); //OPEN & WeatherBit & wunderground
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "light freezing rain", Icon = cloudiness }); //wunderground & Apixu
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "moderate or heavy freezing rain", Icon = cloudiness }); //Apixu
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "heavy freezing rain", Icon = cloudiness }); //wunderground
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "freezing drizzle", Icon = cloudiness }); //wunderground & Apixu
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "patchy freezing drizzle possible", Icon = cloudiness }); //Apixu
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "light freezing drizzle", Icon = cloudiness }); //wunderground
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "heavy freezing drizzle", Icon = cloudiness }); //wunderground & Apixu


            /////////////////////////////////////////////////////// Snow /////////////////////////////////////////////////////////////////////////


            cloudiness = new WeatherIcon("LightSnow.png", true);//Light Snow
            db.WeatherIcons.Add(cloudiness);
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "light snow", Icon = cloudiness }); //OPEN & WeatherBit & wunderground & Apixu
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "patchy light snow", Icon = cloudiness }); //Apixu
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "patchy snow possible", Icon = cloudiness }); //Apixu
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "chance of snow", Icon = cloudiness }); //Apixu

            cloudiness = new WeatherIcon("Snow.png", true);//Snow
            db.WeatherIcons.Add(cloudiness);
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "snow", Icon = cloudiness }); //OPEN & WeatherBit & wunderground
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "light shower snow", Icon = cloudiness }); //OPEN
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "light snow showers", Icon = cloudiness }); //wunderground & Apixu
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "patchy moderate snow", Icon = cloudiness }); //Apixu
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "moderate snow", Icon = cloudiness }); //Apixu

            cloudiness = new WeatherIcon("HeavySnow.png", true);//Heavy Snow
            db.WeatherIcons.Add(cloudiness);
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "heavy snow", Icon = cloudiness }); //OPEN & WeatherBit & wunderground & Apixu
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "patchy heavy snow", Icon = cloudiness }); //Apixu
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "moderate or heavy snow showers", Icon = cloudiness }); //Apixu
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "blowing snow", Icon = cloudiness }); //Apixu
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "blizzard", Icon = cloudiness }); //Apixu
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "shower snow", Icon = cloudiness }); //OPEN
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "heavy shower snow", Icon = cloudiness }); //OPEN
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "snow shower", Icon = cloudiness }); //WeatherBit
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "heavy snow shower", Icon = cloudiness }); //WeatherBit
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "snow showers", Icon = cloudiness }); //wunderground
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "heavy snow showers", Icon = cloudiness }); //wunderground

            cloudiness = new WeatherIcon("Sleet.png", true);//Sleet
            db.WeatherIcons.Add(cloudiness);
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "sleet", Icon = cloudiness }); //OPEN & WeatherBit
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "light sleet", Icon = cloudiness }); //Apixu
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "patchy sleet possible", Icon = cloudiness }); //Apixu
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "moderate or heavy sleet", Icon = cloudiness }); //Apixu
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "light sleet showers", Icon = cloudiness }); //Apixu
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "moderate or heavy sleet showers", Icon = cloudiness }); //Apixu
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "shower sleet", Icon = cloudiness }); //OPEN
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "light rain and snow", Icon = cloudiness }); //OPEN
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "rain and snow", Icon = cloudiness }); //OPEN
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "heavy sleet", Icon = cloudiness }); //WeatherBit
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "mix snow/rain", Icon = cloudiness }); //WeatherBit


            /////////////////////////////////////////////////////// Thunderstorm /////////////////////////////////////////////////////////////////////////


            cloudiness = new WeatherIcon("ThunderstormWithRain.png", true);//Thunderstorm with rain
            db.WeatherIcons.Add(cloudiness);
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "thunderstorm with light rain", Icon = cloudiness }); //OPEN & WeatherBit
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "thunderstorm with rain", Icon = cloudiness }); //OPEN & WeatherBit
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "thunderstorm with heavy rain", Icon = cloudiness }); //OPEN & WeatherBit
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "thunderstorm with light drizzle", Icon = cloudiness }); //OPEN & WeatherBit
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "thunderstorm with drizzle", Icon = cloudiness }); //OPEN & WeatherBit
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "thunderstorm with heavy drizzle", Icon = cloudiness }); //OPEN & WeatherBit
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "thunderstorms and rain", Icon = cloudiness }); //wunderground
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "light thunderstorms and rain", Icon = cloudiness }); //wunderground
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "heavy thunderstorms and rain", Icon = cloudiness }); //wunderground
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "patchy light rain with thunder", Icon = cloudiness }); //Apixu
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "moderate or heavy rain with thunder", Icon = cloudiness }); //Apixu

            cloudiness = new WeatherIcon("Thunderstorm.png", true);//Thunderstorm
            db.WeatherIcons.Add(cloudiness);
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "light thunderstorm", Icon = cloudiness }); //OPEN & wunderground
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "thunderstorm", Icon = cloudiness }); //OPEN & wunderground
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "heavy thunderstorm", Icon = cloudiness }); //OPEN & wunderground
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "ragged thunderstorm", Icon = cloudiness }); //OPEN
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "thundery outbreaks possible", Icon = cloudiness }); //Apixu
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "chance of a thunderstorm", Icon = cloudiness }); //wunderground



            cloudiness = new WeatherIcon("ThunderstormWithHail.png", true);//Thunderstorm with hail
            db.WeatherIcons.Add(cloudiness);
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "thunderstorm with hail", Icon = cloudiness }); //WeatherBit
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "thunderstorms with hail", Icon = cloudiness }); //wunderground
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "light thunderstorms with hail", Icon = cloudiness }); //wunderground
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "heavy thunderstorms with hail", Icon = cloudiness }); //wunderground
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "thunderstorms and ice pellets", Icon = cloudiness }); //wunderground
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "light thunderstorms and ice pellets", Icon = cloudiness }); //wunderground
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "heavy thunderstorms and ice pellets", Icon = cloudiness }); //wunderground
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "thunderstorms with small hail", Icon = cloudiness }); //wunderground
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "light thunderstorms with small hail", Icon = cloudiness }); //wunderground
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "heavy thunderstorms with small hail", Icon = cloudiness }); //wunderground
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "patchy light snow with thunder", Icon = cloudiness }); //Apixu
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "moderate or heavy snow with thunder", Icon = cloudiness }); //Apixu

            cloudiness = new WeatherIcon("Windy.png", false);//Windy
            db.WeatherIcons.Add(cloudiness);
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "squalls", Icon = cloudiness }); //OPEN & wunderground
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "flurries", Icon = cloudiness }); //WeatherBit
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "windy", Icon = cloudiness }); //OPEN
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "windy", Icon = cloudiness }); //OPEN
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "high wind, near gale", Icon = cloudiness }); //OPEN
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "gale", Icon = cloudiness }); //OPEN
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "severe gale", Icon = cloudiness }); //OPEN
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "storm", Icon = cloudiness }); //OPEN
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "violent storm", Icon = cloudiness }); //OPEN


            // breeze is Windy icon?

            //db.CloudinessDescriptions.Add(new DescriptionOfClouds() { Description = "light breeze", CloudState = cloudiness }); //OPEN
            //db.CloudinessDescriptions.Add(new DescriptionOfClouds() { Description = "gentle breeze", CloudState = cloudiness }); //OPEN
            //db.CloudinessDescriptions.Add(new DescriptionOfClouds() { Description = "moderate breeze", CloudState = cloudiness }); //OPEN
            //db.CloudinessDescriptions.Add(new DescriptionOfClouds() { Description = "fresh breeze", CloudState = cloudiness }); //OPEN
            //db.CloudinessDescriptions.Add(new DescriptionOfClouds() { Description = "strong breeze", CloudState = cloudiness }); //OPEN


            /////////////////////////////////////////////////////// Hail /////////////////////////////////////////////////////////////////////////

            cloudiness = new WeatherIcon("LightHail.png", true);//Ligh Hail
            db.WeatherIcons.Add(cloudiness);
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "light hail", Icon = cloudiness }); //wunderground
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "light snow grains", Icon = cloudiness }); //wunderground
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "light ice crystals", Icon = cloudiness }); //wunderground
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "light ice pellets", Icon = cloudiness }); //wunderground
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "light showers of ice pellets", Icon = cloudiness }); //Apixu

            cloudiness = new WeatherIcon("Hail.png", true);//Hail
            db.WeatherIcons.Add(cloudiness);
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "hail", Icon = cloudiness }); //OPEN & wunderground
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "snow grains", Icon = cloudiness }); //wunderground
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "ice crystals", Icon = cloudiness }); //wunderground
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "ice pellets", Icon = cloudiness }); //wunderground & Apixu

            cloudiness = new WeatherIcon("HeavyHail.png", true);//Heavy Hail
            db.WeatherIcons.Add(cloudiness);
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "heavy hail", Icon = cloudiness }); //wunderground
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "heavy snow grains", Icon = cloudiness }); //wunderground
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "heavy ice crystals", Icon = cloudiness }); //wunderground
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "heavy ice pellets", Icon = cloudiness }); //wunderground
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "moderate or heavy showers of ice pellets", Icon = cloudiness }); //Apixu

            /////////////////////////////////////////////////////// N/A /////////////////////////////////////////////////////////////////////////
            cloudiness = new WeatherIcon("na.png", false);//n/a
            db.WeatherIcons.Add(cloudiness);
            db.WeatherDescriptions.Add(new WeatherDescription() { Description = "na", Icon = cloudiness });

            db.SaveChanges();
        }
    }
}