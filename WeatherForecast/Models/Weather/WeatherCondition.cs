using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WeatherForecast.Models.Weather
{
       public class WeatherCondition
    {
         public int Id { get; set; }
        public DateTime date { get; set; }
        public double airTemperature { get; set; } // degree Celsius
        public WeatherDescription description { get; set; }
        public bool isDay { get; set; }
        public double pressure { get; set; }  // millimeter mercury
        public double humidity { get; set; }  // procent
        public double windSpeed { get; set; } // meter per second
        public WindDirection windDirection { get; set; }
        public double rain { get; set; } // millimeters per 3 hours
        public double snow { get; set; } // millimeters per 3 hours
        public int cloud { get; set; } // procent

        // Conversion factor from millibar to millimeter mercury.
        public const double mBarInMMGH = 0.750062;

        // Conversion factor from kilometer per hour to meter per second.
        public const double metersPerSecondInKPH = 0.277778;


        public override bool Equals(object other)
        {
            if (other == null)
                return false;

            if (this.GetType() != other.GetType())
                return false;

            WeatherCondition condition = other as WeatherCondition;
            if (condition != null)
            {
                return AreConditionEquals(condition);
            }

               return false;
        }

        private bool AreConditionEquals(WeatherCondition condition)
        {

            if (airTemperature != condition.airTemperature)
                return false;

            if (isDay != condition.isDay)
                return false;

            if (description != condition.description)
                return false;

            if (pressure != condition.pressure)
                return false;

            if (humidity != condition.humidity)
                return false;

            if (windSpeed != condition.windSpeed)
                return false;

            if (windDirection.Degrees != condition.windDirection.Degrees)
                return false;

            if (rain != condition.rain)
                return false;

            if (cloud != condition.cloud)
                return false;

            return true;
        }

        public override int GetHashCode()
        {

            int result = 17;


            long value = BitConverter.DoubleToInt64Bits(airTemperature);

            result = 37 * result + (int)(value - (value >> 32));

            result = 37 * result + (isDay ? 1 : 0);

            result = 37 * result + (description == null ? 0 : description.Description.GetHashCode());

            value = BitConverter.DoubleToInt64Bits(pressure);
            result = 37 * result + (int)(value - (value >> 32));

            value = BitConverter.DoubleToInt64Bits(humidity);
            result = 37 * result + (int)(value - (value >> 32));
            value = BitConverter.DoubleToInt64Bits(windSpeed);
            result = 37 * result + (int)(value - (value >> 32));

            value = BitConverter.DoubleToInt64Bits(windDirection.Degrees);
            result = 37 * result + (int)(value - (value >> 32));

            value = BitConverter.DoubleToInt64Bits(rain);
            result = 37 * result + (int)(value - (value >> 32));

            result = 37 * result + cloud;

            return result;

        }
     
    }
}