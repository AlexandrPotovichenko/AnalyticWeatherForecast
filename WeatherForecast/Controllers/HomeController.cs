using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using WeatherForecast.Models.WeatherSources;
using WeatherForecast.Models.Weather;
using WeatherForecast.Models.ViewModels;
using WeatherForecast.Models.WeatherForecast;
using WeatherForecast.Models.Context;
using System.Data.Entity;
using WeatherForecast.Models.ForecastRating;
using System.Text;
using WeatherForecast.Models.AnalyticForecast;
using System.Globalization;
using WeatherForecast.Models.Log;

namespace WeatherForecast.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {

            List<DailyIcon> dailyIcons = new List<DailyIcon>();
            AnalyticWeatherForecastView forecast = new AnalyticWeatherForecastView();

            return View(forecast);
        }
      
        public string GetForecast(int day)
        {
            OneDayForecast f = AnalyticWeatherForecastView.GetForecastForDay(day);

            if (f == null || f.Forecasts.Count == 0)
                return "Now there are no forecasts";
            StringBuilder dataHTML = new StringBuilder();

            dataHTML.Append(@" <div class='row'>
            <div class='col-sm-4' style='text-align :center'>
                        <h2> Odessa ");
            dataHTML.Append(f.date.ToString("dddd dd MMMM yyyy",
              CultureInfo.CreateSpecificCulture("en-US")));

            dataHTML.Append(@"</h2>
                        <img class='image' src='");
            dataHTML.Append(f.description.Icon.GetIconURL(true).Replace("~", ""));

            dataHTML.Append(@"' style='width: 65%; height: 65%; '/> <h3>");

            dataHTML.Append(f.description.Description);

            dataHTML.Append(@"</h3>");

            if (f.astro != null)
            {
                dataHTML.Append(@" <table class='table astroTable' style='margin-top: 20px';>");

                dataHTML.Append(@" <tr><td>Sunrise</td> <td> " + (f.astro.Sunrise.HasValue ? f.astro.Sunrise.Value.ToString("hh:mm") : "--:--") + "</td>");
                dataHTML.Append(@" <tr><td>Sunset</td> <td> " + (f.astro.Sunset.HasValue ? f.astro.Sunset.Value.ToString("hh:mm") : "--:--") + "</td>");
                dataHTML.Append(@" <tr><td>Moonrise</td> <td> " + (f.astro.Moonrise.HasValue ? f.astro.Moonrise.Value.ToString("hh:mm") : "--:--") + "</td>");
                dataHTML.Append(@" <tr><td>Moonset</td> <td> " + (f.astro.Moonset.HasValue ? f.astro.Moonset.Value.ToString("hh:mm") : "--:--") + "</td>");
                dataHTML.Append(@" </table> ");

            }
            dataHTML.Append(@"</div> <div class='col-sm-8'>
                        <div class='container'>
                            <p>Latest update :   ");
            dataHTML.Append(AnalyticForecastFactory.LatestForecastUpdate.ToString("dd.MM.yyyy HH:mm"));
            dataHTML.Append(@" </p> <table class='table'> <thead> <tr><th>conditions</th>");

            StringBuilder thTemp = new StringBuilder("<tr> <td>Temperature,°C</td>");
            StringBuilder thPressure = new StringBuilder("<tr> <td>Pres., mm Hg</td>");
            StringBuilder thHumidity = new StringBuilder("<tr> <td>Humidity, %</td>");
            StringBuilder thWindSpeed = new StringBuilder("<tr> <td>WindSpeed,m/s</td>");
            StringBuilder thWindDirection = new StringBuilder("<tr> <td>Wind direction</td>");
            StringBuilder thCloud = new StringBuilder("<tr> <td>Cloud, %</td>");
            StringBuilder thImage = new StringBuilder("<tr> <td> &nbsp </td>");

            StringBuilder thRain = null;
            StringBuilder thSnow = null;

            if (f.Forecasts.Sum(x => x.rain) > 0)
                thRain = new StringBuilder("<tr> <td> Rain ,mm/3h</td>"); // millimeters per 3 hours
            if (f.Forecasts.Sum(x => x.snow) > 0)
                thSnow = new StringBuilder("<tr> <td> Snow ,mm/3h</td>");// millimeters per 3 hours

            foreach (var sf in f.Forecasts)
            {
                //< th >
                dataHTML.Append(@"<th>" + sf.date.ToString("H:mm") + "</th>");
                thImage.Append(@"<td><img class='image' src='" + sf.description.Icon.GetIconURL(sf.isDay).Replace("~", "") + @"' style='width: 100%; height: 100%;' /></td>");

                thTemp.Append("<td> " + (sf.airTemperature > 0 ? "+" : "") + Math.Round(sf.airTemperature, 1).ToString("F1", CultureInfo.InvariantCulture) + "</td>");

                thPressure.Append("<td> " + Math.Round(sf.pressure, 0).ToString("F0") + "</td>");
                thHumidity.Append("<td> " + Math.Round(sf.humidity, 0).ToString("F0") + "</td>");
                thWindSpeed.Append("<td> " + Math.Round(sf.windSpeed, 1).ToString("F1") + "</td>");
                thWindDirection.Append("<td> " + sf.windDirection.ShortName + "</td>");
                thCloud.Append("<td> " + sf.cloud.ToString() + "</td>");
                if (thRain != null)
                    thRain.Append("<td> " + Math.Round(sf.rain, 1).ToString("F1") + "</td>");
                if (thSnow != null)
                    thSnow.Append("<td> " + Math.Round(sf.snow, 1).ToString("F1") + "</td>");
            }
            dataHTML.Append("</tr></thead><tbody>");


            dataHTML.Append(thImage);
            dataHTML.Append("</tr>");

            dataHTML.Append(thTemp);
            dataHTML.Append("</tr>");

            dataHTML.Append(thPressure);
            dataHTML.Append("</tr>");

            dataHTML.Append(thHumidity);
            dataHTML.Append("</tr>");

            dataHTML.Append(thWindSpeed);
            dataHTML.Append("</tr>");

            dataHTML.Append(thWindDirection);
            dataHTML.Append("</tr>");

            dataHTML.Append(thCloud);
            dataHTML.Append("</tr>");

            if (thRain != null)
            {
                dataHTML.Append(thRain);
                dataHTML.Append("</tr>");
            }
            if (thSnow != null)
            {
                dataHTML.Append(thSnow);
                dataHTML.Append("</tr>");
            }


            dataHTML.Append(@" </tbody>   </table> </div>  </div>  </div>");

            return dataHTML.ToString();
        }

        public string GetCurrentWeather()
        {
            // CurrentWeatherSource.CurrentWeather

            StringBuilder dataHTML = new StringBuilder();

            dataHTML.Append(@" <div class='row'>
            <div class='col-sm-4' style='text-align :center'>
                        <h2> Odessa ");
            dataHTML.Append(CurrentWeatherSource.CurrentWeather.date.ToString("dddd dd MMMM yyyy",
              CultureInfo.CreateSpecificCulture("en-US")));

            dataHTML.Append(@"</h2>
                        <img class='image' src='");
            dataHTML.Append(CurrentWeatherSource.CurrentWeather.description.Icon.GetIconURL(true).Replace("~", ""));

            dataHTML.Append(@"' style='width: 65%; height: 65%; '/> <h3>");

            dataHTML.Append(CurrentWeatherSource.CurrentWeather.description.Description);

            dataHTML.Append(@"</h3> </div>");


            dataHTML.Append(@"<div class='col-sm-offset-1 col-sm-6'><div class='container'><p>Latest update :   ");
            dataHTML.Append(CurrentWeatherSource.CurrentWeather.date.ToString("dd.MM.yyyy HH:mm"));

            dataHTML.Append(@"</p> <table class='table'> <thead> <tr><th>Conditions</th>");

            StringBuilder thTemp = new StringBuilder("<tr> <td>Temperature,°C</td>");
            StringBuilder thPressure = new StringBuilder("<tr> <td>Pres., mm Hg</td>");
            StringBuilder thHumidity = new StringBuilder("<tr> <td>Humidity, %</td>");
            StringBuilder thWindSpeed = new StringBuilder("<tr> <td>WindSpeed,m/s</td>");
            StringBuilder thWindDirection = new StringBuilder("<tr> <td>Wind direction</td>");
            StringBuilder thCloud = new StringBuilder("<tr> <td>Cloud, %</td>");

            StringBuilder thImage = new StringBuilder("<tr> <td> &nbsp </td>");

            StringBuilder thRain = null;
            StringBuilder thSnow = null;


            if (CurrentWeatherSource.CurrentWeather.rain == 0.0)
                thRain = new StringBuilder("<tr> <td> Rain ,mm/3h</td>"); // millimeters per 3 hours
            if (CurrentWeatherSource.CurrentWeather.snow == 0.0)
                thSnow = new StringBuilder("<tr> <td> Snow ,mm/3h</td>");// millimeters per 3 hours


            dataHTML.Append(@"<th> &nbsp </th>");

            thTemp.Append("<td> " + (CurrentWeatherSource.CurrentWeather.airTemperature > 0 ? "+" : "") + Math.Round(CurrentWeatherSource.CurrentWeather.airTemperature, 1).ToString("F1", CultureInfo.InvariantCulture) + "</td>");

            thPressure.Append("<td> " + Math.Round(CurrentWeatherSource.CurrentWeather.pressure, 0).ToString("F0") + "</td>");
            thHumidity.Append("<td> " + Math.Round(CurrentWeatherSource.CurrentWeather.humidity, 0).ToString("F0") + "</td>");
            thWindSpeed.Append("<td> " + Math.Round(CurrentWeatherSource.CurrentWeather.windSpeed, 1).ToString("F1") + "</td>");
            thWindDirection.Append("<td> " + CurrentWeatherSource.CurrentWeather.windDirection.ShortName + "</td>");
            thCloud.Append("<td> " + CurrentWeatherSource.CurrentWeather.cloud.ToString() + "</td>");
            if (thRain != null)
                thRain.Append("<td> " + Math.Round(CurrentWeatherSource.CurrentWeather.rain, 1).ToString("F1") + "</td>");
            if (thSnow != null)
                thSnow.Append("<td> " + Math.Round(CurrentWeatherSource.CurrentWeather.snow, 1).ToString("F1") + "</td>");

            dataHTML.Append("</tr></thead><tbody>");


            dataHTML.Append(thTemp);
            dataHTML.Append("</tr>");

            dataHTML.Append(thPressure);
            dataHTML.Append("</tr>");

            dataHTML.Append(thHumidity);
            dataHTML.Append("</tr>");

            dataHTML.Append(thWindSpeed);
            dataHTML.Append("</tr>");

            dataHTML.Append(thWindDirection);
            dataHTML.Append("</tr>");

            dataHTML.Append(thCloud);
            dataHTML.Append("</tr>");

            if (thRain != null)
            {
                dataHTML.Append(thRain);
                dataHTML.Append("</tr>");
            }
            if (thSnow != null)
            {
                dataHTML.Append(thSnow);
                dataHTML.Append("</tr>");
            }


            dataHTML.Append(@" </tbody>   </table> </div> </div>");

            //////////////////////////////////////////////////////////////////////////////

            return dataHTML.ToString();
        }
        public ActionResult HowItWorks()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}