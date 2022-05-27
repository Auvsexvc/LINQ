using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using LINQ1;

// ReSharper disable UseFormatSpecifierInInterpolation

namespace FirstProject
{
    class Program
    {
        static void Main(string[] args)
        {
            string csvPath = @"c:\etc\googleplaystore1.csv";
            var googleApps = LoadGoogleAps(csvPath);

            //Display(googleApps);
            //GetData(googleApps);
            //ProjectData(googleApps);
            //DivideData(googleApps);
            //OrderData(googleApps);
            //DataSetOperation(googleApps);
            //DataVerification(googleApps);
            //GroupData(googleApps);
            GroupDataOperations(googleApps);
        }


        static void GroupDataOperations(IEnumerable<GoogleApp> googleApps)
        {
            var categoryGroup = googleApps
                .GroupBy(a => a.Category)
                .Where(g=>g.Min(a=>a.Reviews)>=10);
            
            foreach (var item in categoryGroup)
            {
                var averageReviews = item.Average(a => a.Reviews);
                var minReviews = item.Min(a => a.Reviews);
                var maxReviews = item.Max(a => a.Reviews);
                var totalReviews = item.Sum(a => a.Reviews);

                var allAppsFromGroupRatingOf3 = item.All(a => a.Rating > 3.0);
                var countApps = item.TakeWhile(a => a.Rating >= 3.0).ToList();

                Console.WriteLine($"{item.Key}\n avg: {averageReviews}\n min: {minReviews}\n max: {maxReviews}\n total:{totalReviews}\n list of app.Rating>=3: {countApps.Count}");
                //while (allAppsFromGroupRatingOf3)
                {
                    foreach (var app in countApps)
                    {
                        Console.Write(app.Name+"; ");
                    }
                }
                Console.WriteLine("\n");   
            }
        }
        static void GroupData(IEnumerable<GoogleApp> googleApps)
        {
            var categoryGroup = googleApps
                .GroupBy(a => new
                {
                    a.Category,
                    a.Type
                });


            foreach (var group in categoryGroup)
            {
                var key = group.Key;
                var apps = group.ToList();
                Console.WriteLine($"Displaying elems for ggroup {group.Key.Category} , {group.Key.Type}");
                Display(apps);
            }
            //var artAndDesignGroup = categoryGroup.First(a => a.Key == Category.ART_AND_DESIGN);

            //var apps = artAndDesignGroup.Select(a => a);
            
        }
        static void DataVerification(IEnumerable<GoogleApp> googleApps)
        {
            var allOperatorResult = googleApps.Where(app => app.Category == Category.WEATHER)
                .All(app => app.Reviews > 10);

            Console.WriteLine($"All : {allOperatorResult}");

            var anyOperatorResult = googleApps.Where(app => app.Category == Category.WEATHER)
                .Any(app => app.Reviews > 3000000);

            Console.WriteLine($"Any : {anyOperatorResult}");
        }

        static void DataSetOperation(IEnumerable<GoogleApp> googleApps)
        {
            var paidAppsCategories = googleApps.Where(app => app.Type == Type.Paid)
                .Select(app => app.Category).Distinct();

            //Console.WriteLine($"Płatne kategorie: {string.Join(", ",paidAppsCategories)}");

            var setA = googleApps.Where(app => app.Rating > 4.7 && app.Type == Type.Paid && app.Reviews > 1000);
            var setB = googleApps.Where(app => app.Name.Contains("Pro") && app.Rating > 4.6 && app.Reviews > 10000);

            //Display(setA);
            //Console.WriteLine("******************************************************");
            //Display(setB);

            var appsUnion = setA.Union(setB);
            Console.WriteLine("App UNION");
            Display(appsUnion.OrderBy(app=>app.Name));

            Console.WriteLine("App INTERSECT: ");
            var appsIntersect = setA.Intersect(setB);
            Display(appsIntersect);

            var appsExcept = setA.Except(setB);
            Console.WriteLine("App EXCEPT: ");
            Display(appsExcept);
        }
        static void OrderData(IEnumerable<GoogleApp> googleApps)
        {
            var highRatedBeautyApps = googleApps.Where(app => app.Rating > 4.4 && app.Category == Category.BEAUTY);
            //Display(highRatedBeautyApps);

            var sortedResults = highRatedBeautyApps
                .OrderByDescending(app => app.Rating)
                .ThenByDescending(app=>app.Reviews)
                .Take(5);
            Display(sortedResults);

        }
        static void DivideData(IEnumerable<GoogleApp> googleApps)
        {
            var highRatedBeautyApps = googleApps.Where(app => app.Rating > 4.4 && app.Category == Category.BEAUTY);

            //var first5highRatedBeautyApps = new List<GoogleApp>();

            //foreach (var item in highRatedBeautyApps) 
            //{
            //    first5highRatedBeautyApps.Add(item);
            //    if (first5highRatedBeautyApps.Count() == 5)
            //    {
            //        break;
            //    }
            //}

            //var first5highRatedBeautyApps = highRatedBeautyApps.Take(5);
            //var first5highRatedBeautyApps = highRatedBeautyApps.TakeLast(5);
            //var first5highRatedBeautyApps = highRatedBeautyApps.TakeWhile(app => app.Reviews>1000);
            //Display(first5highRatedBeautyApps);

            var skippedResults = highRatedBeautyApps.SkipWhile(app => app.Reviews > 1000); ///warunek dziala do pierwszego wystapienia!!!
            Display(skippedResults);
        }
        static void ProjectData(IEnumerable<GoogleApp> googleApps)
        {
            var highRatedBeautyApps = googleApps.Where(app => app.Rating > 4.6 && app.Category == Category.BEAUTY);
            var highRatedBeautyAppsNames = highRatedBeautyApps.Select(app => app.Name);

            var dtos = highRatedBeautyApps.Select(app => new GoogleAppDto()
            {
                Reviews = app.Reviews,
                Name = app.Name
            });

            foreach (var item in dtos)
            {
                Console.WriteLine($"{item.Name} - {item.Reviews}");
            }

            //Console.WriteLine(string.Join(", ", highRatedBeautyAppsNames));

            var genres = highRatedBeautyApps.SelectMany(app => app.Genres);

            Console.WriteLine(string.Join(":", genres));

            var anonymouseDtos = highRatedBeautyApps.Select(app => new 
            {
                Reviews = app.Reviews,
                Name = app.Name,
                Category = app.Category
            });

            foreach (var item in anonymouseDtos)
            {
                Console.WriteLine($"{item.Name} - {item.Reviews} - {item.Category} ");
            }

            //List<string> highRatedBeautyAppsNames = new List<string>();

            //foreach (var item in highRatedBeautyAppsNames)
            //{
            //    highRatedBeautyAppsNames.Add(item);
            //}
        }
        static void GetData(IEnumerable<GoogleApp> googleApps)
        {
            

            var highRatedApps = googleApps.Where(app => app.Rating > 4.6 && app.Category == Category.BEAUTY);
            var highRatedBeautyApps = highRatedApps.Where(app => app.Category == Category.BEAUTY);
            //Display(highRatedApps);

            //var firstHighRatedBeautyApp = highRatedBeautyApps.FirstOrDefault(app=>app.Reviews<50);
            //var firstHighRatedBeautyApp = highRatedBeautyApps.SingleOrDefault(app => app.Reviews < 300);
            var firstHighRatedBeautyApp = highRatedBeautyApps.LastOrDefault(app => app.Reviews < 50);
            Display(firstHighRatedBeautyApp);
        }
        static void Display(IEnumerable<GoogleApp> googleApps)
        {
            foreach (var googleApp in googleApps)
            {
                Console.WriteLine(googleApp);
            }

        }
        static void Display(GoogleApp googleApp)
        {
            Console.WriteLine(googleApp);
        }

        static List<GoogleApp> LoadGoogleAps(string csvPath)
        {
            using (var reader = new StreamReader(csvPath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<GoogleAppMap>();
                var records = csv.GetRecords<GoogleApp>().ToList();
                return records;
            }

        }

    }


}

