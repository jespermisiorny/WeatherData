﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using static WeatherData.Data.FileData;
using WeatherData.Data;
using static WeatherData.Program;
using System.Collections.Immutable;
using WeatherData.Classes;
using static WeatherData.Classes.Helpers;
using System.Security.Cryptography.X509Certificates;
using System.Globalization;

namespace WeatherData.Models
{
    public interface iMeasurable
    {
        public string? Location { get; set; }
        public string Name { get; set; }

        void AvgValues();
        void MaxMinWeatherDay(string chooseOrderBy);
        //string CalculateTotalRiskPerTimeSpan(string timeSpan);
        //List<string> CalculateTotalRiskPerTimeSpan(string timeSpan);
        void MeteorologicalDate(int temp);
    }
    public class Data1
    {
        private const string filePath = "../../../Data/tempdata5-med fel.txt";
        public string? Location { get; set; }
        public string Name { get; set; }

        public static List<iMeasurable> CreateData()
        {
            List<iMeasurable> data = new List<iMeasurable>();
            data.Add(new Inomhus() { Location = "Inne", Name = "Inside" });
            data.Add(new Utomhus() { Location = "Ute", Name = "Outside" });

            return data;
        }
        // Search for a specific Date
        public virtual void AvgValues()
        {
            List<FileData> dataList = new List<FileData>();
            dataList = ReadTextFile(filePath);
            DateTime inputDate = PromptUserForDate();


            List<FileData> filteredData = dataList.Where(d => d.DateTime.Date == inputDate.Date && d.Location == Location).ToList();
            if (filteredData.Count > 0)
            {
                double dataAvgTemp = filteredData.Average(x => x.Temperature);
                double dataAvgResult = filteredData.Average(x => x.Humidity);

                if (Location == "Inne")
                {
                    Console.WriteLine(new String('=', 30));
                    Console.WriteLine("Average temperature inside: " + Math.Round(dataAvgTemp, 2));
                }
                else if (Location == "Ute")
                {
                    Console.WriteLine(new String('=', 30));
                    Console.WriteLine("Average temperature outside: " + Math.Round(dataAvgTemp, 2));
                    Console.WriteLine(new String('-', 30));
                    Console.WriteLine("Average humidity: " + Math.Round(dataAvgResult, 2));
                }
                else
                {
                    // This will probably never happen, but better safe than sorry..
                    Console.WriteLine(new String('-', 30));
                    Console.WriteLine("Couldn't find any data from inside on that day.");
                }
            }
            else
            {
                Console.WriteLine(new String('-', 30));
                Console.WriteLine("Couldn't find any data from inside on that day.");
            }


        }
        public static void Test()
        {
            Data1 data = new();
            List<FileData> dataList = new List<FileData>();
            dataList = Helpers.ReadTextFile(filePath);
            //string ute = "Ute";
            //string inne = "Inne";

            string tempInsideToLogFile = "";
            string tempOutsideToLogFile = "";

            string humInsideToLogFile = "";
            string humOutsideToLogFile = "";

            string moldInsideToLogFile = "";
            string moldOutsideToLogFile = "";

            List<FileData> filterUte = dataList.Where(d => d.Location == "Ute").ToList();
            var uteData = filterUte.GroupBy(d => d.DateTime.Month);

            foreach (var month in uteData)
            {
                tempOutsideToLogFile += ($"Month {month.Key.ToString("MMMM")} - Average temperature outside: {Math.Round(month.Average(y => y.Temperature), 1)}°C\n");
                humOutsideToLogFile += ($"Month {month.Key.ToString()} - Average humidity outside: {Math.Round(month.Average(y => y.Humidity), 1)}°C\n");
                //moldOutsideToLogFile = data.CalculateTotalRiskPerTimeSpan("Month");
            }

            List<FileData> filterInne = dataList.Where(d => d.Location == "Inne").ToList();
            var inneData = filterInne.GroupBy(d => d.DateTime.Month);

            foreach (var month in inneData)
            {
                tempInsideToLogFile += ($"Month {month.Key.ToString()} - Average temperature inside: {Math.Round(month.Average(y => y.Temperature), 1)}°C\n");
                humInsideToLogFile += ($"Month {month.Key.ToString()} - Average humidity inside: {Math.Round(month.Average(y => y.Humidity), 1)}°C\n");
                //moldInsideToLogFile = data.CalculateTotalRiskPerTimeSpan("Month");
            }

            ("Outside").ViewBox(0);
            //moldOutsideToLogFile = data.CalculateTotalRiskPerTimeSpan("Month");
            Console.WriteLine(new String('-', 35));
            Console.WriteLine("Ute");
            Console.WriteLine(tempOutsideToLogFile);
            Console.WriteLine(new String('-', 35));
            Console.WriteLine(humOutsideToLogFile);
            //Console.WriteLine(moldOutsideToLogFile);
            ("Inside").ViewBox(35);
            //moldInsideToLogFile = data.CalculateTotalRiskPerTimeSpan("Month");
            Console.WriteLine(new String('-', 35));
            Console.WriteLine("Inne");
            Console.WriteLine(tempInsideToLogFile);
            Console.WriteLine(new String('-', 35));
            Console.WriteLine(humInsideToLogFile);
            //Console.WriteLine(moldInsideToLogFile);



            //foreach (var month in monthData)
            //{
            //    location = "Inne";
            //    tempInsideToLogFile = ($"{month.Key.ToString("MMMM")} - Average temperature inside: {Math.Round(month.Average(y => y.Temperature), 1)}°C");
            //    humInsideToLogFile = ($"{month.Key.ToString("MMMM")} - Average humidity inside: {Math.Round(month.Average(y => y.Humidity), 1)}°C");
            //    moldInsideToLogFile = data.CalculateTotalRiskPerDay("Month");

            //    location = "Ute";
            //    tempOutsideToLogFile = ($"{month.Key.ToString("MMMM")} - Average temperature outside: {Math.Round(month.Average(y => y.Temperature), 1)}°C");
            //    humOutsideToLogFile = ($"{month.Key.ToString("MMMM")} - Average humidity outside: {Math.Round(month.Average(y => y.Humidity), 1)}°C");
            //    moldOutsideToLogFile = data.CalculateTotalRiskPerDay("Month");
            //}



        }

        // Warm/Cold || Wet/Dry
        public virtual void MaxMinWeatherDay(string chooseOrderBy)
        {
            List<FileData> dataList = new List<FileData>();
            dataList = Helpers.ReadTextFile(filePath);
            var filteredData = dataList.Where(x => x.Location == Location).ToList();
            var dayData = filteredData.GroupBy(x => x.DateTime.Date);
            var sortedData = dayData;

            if (chooseOrderBy == "Temperature")
            {
                sortedData = dayData.OrderByDescending(x => x.Average(y => y.Temperature));
            }
            else if (chooseOrderBy == "Humidity")
            {
                sortedData = dayData.OrderByDescending(x => x.Average(y => y.Humidity));
            }

            foreach (var item in sortedData)
            {
                if (chooseOrderBy == "Temperature")
                {
                    Console.WriteLine($"{item.Key.ToString("dd MMM")} - Average temperature: {Math.Round(item.Average(y => y.Temperature), 1)}°C");
                }
                if (chooseOrderBy == "Humidity")
                {
                    Console.WriteLine($"{item.Key.ToString("dd MMM")} - Average humidity: {Math.Round(item.Average(y => y.Humidity), 1)}%");
                }
            }
        }
        // Sort by risk of mold 
        //public virtual List<string> CalculateTotalRiskPerTimeSpan(string timeSpan)
        //{
        //    //string result = "";
        //    Dictionary<DateTime, double> riskList = new Dictionary<DateTime, double>();
        //    List<FileData> dataList = Helpers.ReadTextFile(filePath);
        //    //var filteredData = dataList.Where(x => x.Location == Location).ToList();

        //    var groupedData = GroupDataByTimeSpan(dataList, timeSpan).ToList();
        //    foreach (var dayData in groupedData)
        //    {
        //        double risk = CalculateMoldRisk(Math.Round(dayData.Average(y => y.Temperature), 1), Math.Round(dayData.Average(y => y.Humidity), 1));
        //        var parsedDate = GetParsedDate(dayData.Key, timeSpan);
        //        riskList.Add(parsedDate, risk);
        //    }

        //    var sortedRiskList = riskList.OrderByDescending(x => x.Value);
        //    List<string> results2 = new List<string>();
        //    foreach (var dayD in sortedRiskList)
        //    {
        //        string result = (dayD.Key.ToString("MMMM") + " - Total risk of mold: " + dayD.Value + "%");
        //        Console.WriteLine(result);
        //        results2.Add(result);
        //    }

        //    return results2;
        //}
        //public static void koko()
        //{
        //    List<FileData> dataList = Helpers.ReadTextFile(filepath);
        //    var avgValuesForEachDay = GetAverageValueForEachDay(dataList);

        //    foreach (var dayData in avgValuesForEachDay)
        //    {
        //        Console.WriteLine("For day " + dayData.Key.ToString("dd-MM") + " the average temperature is " + dayData.Value.Item1 + " and the average humidity is: " + dayData.Value.Item2);
        //    }
        //    var avgValuesForEachMonth = GetAverageValueForEachMonth(dataList);
        //    Console.WriteLine("For the month the average is: " avgValuesForEachMonth);

        //}

        //public static Dictionary<DateTime, double> CalculateDailyRisk(List<FileData> dataList, string timeSpan)
        //{
        //    Dictionary<DateTime, double> riskList = new Dictionary<DateTime, double>();

        //    var groupedData = GroupDataByTimeSpan(dataList, timeSpan).ToList();
        //    foreach (var dayData in groupedData)
        //    {
        //        double risk = CalculateMoldRisk(Math.Round(dayData.Average(y => y.Temperature), 1), Math.Round(dayData.Average(y => y.Humidity), 1));
        //        var parsedDate = GetParsedDate(dayData.Key, timeSpan);
        //    }
        //    return riskList;
        //}

        //public static  Dictionary<DateTime, double> CalculateMonthlyRisk(List<FileData> dataList)
        //{
        //    Dictionary<DateTime, double> riskList = new Dictionary<DateTime, double>();
        //    var groupedData = dataList.GroupBy(x => new DateTime(x.DateTime.Year, x.DateTime.Month, 1));

        //    foreach (var monthData in groupedData)
        //    {
        //        double risk = CalculateMoldRisk(Math.Round(monthData.Average(y => y.Temperature), 1), Math.Round(monthData.Average(y => y.Humidity), 1));
        //        var date = new DateTime(monthData.Key.Year, monthData.Key.Month, 1);
        //        riskList.Add(date, risk);
        //    }
        //    return riskList;
        //}


        //private static IEnumerable<IGrouping<DateTime, FileData>> GroupDataByTimeSpan(List<FileData> data, string timeSpan)
        //{
        //    if (timeSpan == "Month")
        //    {
        //        return data.GroupBy(x => new DateTime(x.DateTime.Year, x.DateTime.Month, 1));
        //    }
        //    else
        //    {
        //        return data.GroupBy(x => x.DateTime.Date);
        //    }
        //}

        private static DateTime GetParsedDate(DateTime key, string timeSpan)
        {
            if (timeSpan == "Month")
            {
                return new DateTime(key.Year, key.Month, 1);
            }
            else
            {
                return key.Date;
            }
        }

        // Calculate the risk of mold
        public static double CalculateMoldRisk(double temp, double hum)
        {
            double risk = 0;

            if ((temp > 0 && temp < 10) && (hum > 95 && hum <= 100))
            {
                risk = 100;
            }
            else if (temp < 0 || hum < 75)
            {
                risk = 0;
                return risk;
            }
            else
            {
                //TEMP
                if (temp < 10)
                {
                    risk += 20;
                }

                if (temp < 20)
                {
                    risk += 15;
                }

                if (temp < 30)
                {
                    risk += 10;
                }

                if (temp < 40)
                {
                    risk += 5;
                }

                //HUM
                if (hum > 80)
                {
                    risk += 10;
                }

                else if (hum > 90)
                {
                    risk += 15;
                }

                else if (hum > 95)
                {
                    risk += 20;
                }
            }

            return risk;
        }

        public static Dictionary<DateTime, double> CreateDicForAutumn()
        {
            string filePath = "../../../Data/tempdata5-med fel.txt";


            List<FileData> dataList = new List<FileData>();
            dataList = Helpers.ReadTextFile(filePath);
            var filteredData = dataList.Where(x => x.Location == "Ute").ToList();
            var dayData = filteredData.GroupBy(x => x.DateTime.Date);
            Dictionary<DateTime, double> autumnList = new Dictionary<DateTime, double>();

            foreach (var day in dayData.OrderBy(x => x.Key))
            {
                string stringToParse = day.Key.ToString("dd MMM");
                var parsedDate = DateTime.Parse(stringToParse);
                var avgTemp = (Math.Round(day.Average(y => y.Temperature), 1));
                autumnList.Add(parsedDate, avgTemp);
            }

            return autumnList;

        }


    }
    public class Inomhus : Data1, iMeasurable
    {
        public void MeteorologicalDate(int temp)
        {

        }
    }
    public class Utomhus : Data1, iMeasurable
    {
        public void MeteorologicalDate(int temp)
        {
            string filePath = "../../../Data/tempdata5-med fel.txt";
            Dictionary<DateTime, double> list = CreateDicForAutumn();

            DateTime startDate = new DateTime(2016, 08, 01);
            int tempCount = 0;

            foreach (var day in list)
            {
                if (day.Key > startDate)
                {
                    if (tempCount < 5)
                    {
                        if (day.Value <= temp)
                        {
                            tempCount++;
                        }
                        else
                        {
                            startDate = day.Key;
                            tempCount = 0;
                        }
                    }

                }
            }
            Console.WriteLine("Meterological " + (temp == 10 ? "fall" : "winter") + " occurs on the " + startDate.ToString("dd MMM"));
        }

    }
}
