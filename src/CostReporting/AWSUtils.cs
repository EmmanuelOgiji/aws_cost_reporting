using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Amazon.CostExplorer;
using Amazon.CostExplorer.Model;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;

namespace CostReporting
{
    public interface IAwsUtils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateInterval"></param>
        /// <param name="costMetric"></param>
        /// <param name="granularity"></param>
        /// <returns></returns>
        GetCostAndUsageResponse GetCostResponse(DateInterval dateInterval, string costMetric, string granularity);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="costReportText"></param>
        void PublishCostReportToSns(string costReportText);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="costReportText"></param>
        void SendCostReportEmails(string costReportText);
    }

    public class AwsUtils : IAwsUtils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateInterval"></param>
        /// <param name="costMetric"></param>
        /// <param name="granularity"></param>
        /// <returns></returns>
        public GetCostAndUsageResponse GetCostResponse(DateInterval dateInterval, string costMetric, string granularity)
        {
            var costExplorer = new AmazonCostExplorerClient();
            var getLastMonthCostRequest = BuildCostRequest(dateInterval, costMetric, granularity);
            try
            {
                var costResponse = costExplorer.GetCostAndUsageAsync(
                    getLastMonthCostRequest, CancellationToken.None
                ).Result;
                Console.WriteLine(costResponse.ResultsByTime.ToString());
                return costResponse;
            }
            catch (Exception ex)
            {
                Console.WriteLine("There was an issue getting the cost response");
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateInterval"></param>
        /// <param name="costMetric"></param>
        /// <param name="granularity"></param>
        /// <returns></returns>
        private GetCostAndUsageRequest BuildCostRequest(DateInterval dateInterval, string costMetric, string granularity)
        {
            Console.WriteLine("Building GetCostAndUsageRequest for Cost Explorer");
            var getCostAndUsageRequest = new GetCostAndUsageRequest
            {
                Granularity = granularity,
                Metrics = new List<String> {costMetric},
                TimePeriod = dateInterval
            };
            return getCostAndUsageRequest;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="costReportText"></param>
        public void PublishCostReportToSns(string costReportText)
        {
            Console.WriteLine("Publishing cost report to SNS");
            var sns = new AmazonSimpleNotificationServiceClient();
            var topicArn = Environment.GetEnvironmentVariable("sns_topic_arn");
            Console.WriteLine("Topic Arn from environment var: " + topicArn);
            try
            {
                var publishResponse = sns.PublishAsync(new PublishRequest
                {
                    Subject = Constants.EmailSubject,
                    Message = costReportText,
                    TopicArn = topicArn
                }).Result;
                Console.WriteLine(publishResponse.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine("There was an issue publishing to SNS");
                Console.WriteLine(ex.Message);
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="costReportText"></param>
        /// <returns></returns>
        private SendEmailRequest BuildSesSendRequest(string costReportText)
        {
            Console.WriteLine("Building Send Request for SES emails");
            var senderAddress = Environment.GetEnvironmentVariable("sender_address");
            Console.WriteLine($"Sender email: {senderAddress}");
            var receiverAddresses = Environment.GetEnvironmentVariable("receiver_addresses")?.Split(',').ToList();
            Console.WriteLine($"Sender email: {receiverAddresses}");
            return new SendEmailRequest
            {
                Source = senderAddress,
                Destination = new Destination
                {
                    ToAddresses = receiverAddresses
                },
                Message = new Message
                {
                    Subject = new Content(Constants.EmailSubject),
                    Body = new Body
                    {
                        Text = new Content
                        {
                            Charset = "UTF-8",
                            Data = costReportText
                        }
                    }
                }
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="costReportText"></param>
        public void SendCostReportEmails(string costReportText)
        {
            Console.WriteLine("Sending cost report email using Amazon SES");
            var ses = new AmazonSimpleEmailServiceClient();
            var sendRequest = BuildSesSendRequest(costReportText);
            try
            {
                var response = ses.SendEmailAsync(sendRequest).Result;
                Console.WriteLine("The email was sent successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("The email was not sent.");
                Console.WriteLine("Error message: " + ex.Message);
            }
        }
    }
}