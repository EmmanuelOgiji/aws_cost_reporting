using Amazon.CostExplorer.Model;
using System;
using System.IO;

namespace CostReporting
{
    public class CostReport
    {
        public DateInterval DateInterval { get; }
        private double UnblendedCostOverPeriod { get; }
        private readonly IAwsUtils _utils;


        public CostReport(int daysBack, IAwsUtils utils)
        {
            DateInterval = getDateIntervalFromNow(daysBack);
            UnblendedCostOverPeriod = GetUnblendedCostOverPeriod();
            _utils = utils ?? throw new ArgumentNullException();
        }

        private double GetUnblendedCostOverPeriod()
        {
            var costResponseOverPeriod = _utils.GetCostResponse(DateInterval, "UnblendedCost", "DAILY");
            Console.WriteLine("Calculating total cost over the period specified");
            double totalCost = 0.0;
            foreach (var result in costResponseOverPeriod.ResultsByTime)
            {
                var cost = Convert.ToDouble(result.Total["UnblendedCost"].Amount);
                totalCost += cost;
            }

            totalCost = Math.Round(totalCost, 2);
            Console.WriteLine($"Total Cost: {totalCost}");
            return totalCost;
        }
        
        private DateInterval getDateIntervalFromNow(int daysBack)
        {
            Console.WriteLine($"Building date interval of {daysBack} days ago from Today");
            var dateFormat = "yyyy-MM-dd";
            var dateInterval = new DateInterval
            {
                End = DateTime.Today.ToString(dateFormat),
                Start = DateTime.Today.AddDays(-daysBack).ToString(dateFormat)
            };
            Console.WriteLine($"Start date is {dateInterval.Start}");
            Console.WriteLine($"End date is {dateInterval.End}");
            return dateInterval;
        }
        public string GetMessage()
        {
            Console.WriteLine("Building Cost Report Text");
            var costReportWriter = new StringWriter();
            costReportWriter.WriteLine($"The cost incurred over the period between {DateInterval.Start}" +
                                       $" and {DateInterval.End} is ${UnblendedCostOverPeriod}");
            Console.WriteLine($"{costReportWriter}");
            return costReportWriter.ToString();
        }
    }
}
