using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WeatherForecast.Models.Weather
{
    [ComplexType]
    public class WindDirection
    {
        public WindDirection()
        {
            this.Degrees = 0.0;
            this.ShortName = "Default";
        }
        public WindDirection(double degrees, string shortName = "Default")
        {
            this.Degrees = degrees;
            this.ShortName = shortName == "Default" ? GetShortName(degrees) : shortName;
        }
        public string ShortName { get; private set; }
        public double Degrees { get; private set; }

        internal const double stepBetweenDirections = 22.5;
        internal static string GetShortName(double degrees)
        {
            degrees -= stepBetweenDirections / 2;//this is a centre of direction
            int entireDirection = (int)(degrees / stepBetweenDirections);
            string[] caridnals = { "N", "NNE", "NE", "ENE", "E", "ESE", "SE", "SSE", "S", "SSW", "SW", "WSW", "W", "WNW", "NW", "NNW", "N" };
            int indexDirection = (degrees % stepBetweenDirections) == 0 ? entireDirection : entireDirection++;
            return caridnals[indexDirection];
        }
        public static int GetDifference(double degrees, double otherDegrees) //return the difference in degrees
        {

            double difference = Math.Abs(degrees - otherDegrees);
            if (difference > 180)
                return (int)(360 - difference);
            return (int)(difference);
        }
    }
}