using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;

namespace ChatBot
{
    public static class ChatBot
    {
        private static readonly int _closePosition = 13;
        private static string _symbol;

        [FunctionName("ChatBot")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            _symbol = req.Query["stockName"];

            List<string> splitted = new List<string>();
            string fileList = GetCSV(_symbol);
            string[] tempStr;

            tempStr = fileList.Split(',');

            foreach (string item in tempStr)
            {
                if (!string.IsNullOrWhiteSpace(item))
                {
                    splitted.Add(item);
                }
            }
            if (splitted[_closePosition] == "N/D"){
                log.LogError("Stock code is not valid", _symbol);
                return new OkObjectResult($"Stock code is not valid. StockCode = {_symbol}");
                throw new Exception("Stock code is not valid. StockCode = {_symbol}");                
            }

            string responseMessage = $"{_symbol.ToUpper()} quote is {splitted[_closePosition]} per share";

            return new OkObjectResult(responseMessage);
        }

        public static string GetCSV(string symbol)
        {
            var url = Environment.GetEnvironmentVariable("StockAPIURLBeginning") + symbol + Environment.GetEnvironmentVariable("StockAPIURLEnding");
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

            StreamReader sr = new StreamReader(resp.GetResponseStream());
            string results = sr.ReadToEnd();
            sr.Close();

            return results;
        }
    }
}


