using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Amazon.CostExplorer;
using Amazon.SimpleNotificationService;
using Amazon.CostExplorer.Model;
using Amazon.Lambda.Core;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Amazon.SimpleNotificationService.Model;
using Amazon.SimpleNotificationService.Util;
using Message = Amazon.SimpleEmail.Model.Message;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CostReporting
{
    public class CostReporter
    {
        private readonly IAwsUtils _utils;

        public CostReporter(IAwsUtils utils)
        {
            _utils = utils ?? throw new ArgumentNullException();
        }
        public string FunctionHandler(string input, ILambdaContext context)
        {
            Console.WriteLine($"Lambda function triggered by: {input}");
            var costReport = new CostReport(30, _utils).GetMessage();
            _utils.PublishCostReportToSns(costReport);
            _utils.SendCostReportEmails(costReport);
            return costReport;
        }
    }
}