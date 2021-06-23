using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Functions.Tests
{
    public class FunctionsTests
    {
        private readonly ILogger logger = TestFactory.CreateLogger();

        [Fact]
        public async void Http_trigger_should_return_Parsed_CSV_HappyPath()
        {
            //Arrange
            Environment.SetEnvironmentVariable("StockAPIURLBeginning", "https://stooq.com/q/l/?s=");
            Environment.SetEnvironmentVariable("StockAPIURLEnding", "&f=sd2t2ohlcv&h&e=csv");
            var stockNameValue = "aapl.us";
            var request = TestFactory.CreateHttpRequest("stockName", stockNameValue);

            //Act
            var response = (OkObjectResult)await ChatBot.ChatBot.Run(request, logger);

            //Assert
            Assert.StartsWith("AAPL.US quote is", response.Value.ToString());
        }

        [Fact]
        public async void Http_trigger_should_return_Bad_Request_Because_Of_Invalid_Stock_Code()
        {
            //Arrange
            Environment.SetEnvironmentVariable("StockAPIURLBeginning", "https://stooq.com/q/l/?s=");
            Environment.SetEnvironmentVariable("StockAPIURLEnding", "&f=sd2t2ohlcv&h&e=csv");
            var stockNameValue = "aapl.uss";
            var request = TestFactory.CreateHttpRequest("stockName", stockNameValue);

            //Act
            var response = await ChatBot.ChatBot.Run(request, logger);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }
    }
}