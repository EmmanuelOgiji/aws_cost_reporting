using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.CostExplorer;
using Amazon.CostExplorer.Model;
using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;
using Amazon.Runtime;
using Moq;
using Xunit.Abstractions;

namespace CostReporting.Tests
{
    public class CostReporterTests
    {
        
        [Fact]
        public void GetCostReportOutput()
        {
            var mockAwsUtils = new Mock<IAwsUtils>();
            var costReport = new CostReport(30, mockAwsUtils.Object);
            var result = costReport.GetMessage();
            Assert.Equal("", result);
        }
        
    }

    public class DateIntervalTests
    {
        [Fact]
        public void GetDateIntervalTest()
        {
            var mockAwsUtils = new Mock<IAwsUtils>();
            var costReport = new CostReport(30, mockAwsUtils.Object);
            var result = costReport.DateInterval;
            Assert.Equal(DateTime.Today.ToString("yyyy-MM-dd"), result.End);
            Assert.Equal(DateTime.Today.AddDays(-30).ToString("yyyy-MM-dd"), result.Start);
        }
    }

    public class AwsUtilsTests
    {
        [Fact]
        public void PublishCostReportToSnsIsCalled()
        {
            //Arrange
            var mockAwsUtils = new Mock<IAwsUtils>();
            var sut = new CostReporter(mockAwsUtils.Object);
            
            //Act
            sut.FunctionHandler("", null);
            
            //Assert
            mockAwsUtils.Verify(x => x.PublishCostReportToSns(It.IsAny<string>()), Times.Once());

        }
    }
}