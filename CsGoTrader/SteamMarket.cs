using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;

namespace CsGoTrader
{
    public class SteamMarket
    {
        public static List<double> getPrices(string skinName, Quality quality)
        {
            return getPrices(
                string.Format(
                    @"http://steamcommunity.com/market/listings/730/{0}%20%28{1}%29/render?currency=1",
                    skinName,
                    EnumUtil.qualityToString(quality)));
        }

        public static List<double> getPrices(string requestUrl)
        {
            Thread.Sleep(1000);

            List<double> prices = new List<double>();

            try
            {
                //string requestUrl = @"http://steamcommunity.com/market/listings/730/AK-47%20%7C%20Aquamarine%20Revenge%20%28Factory%20New%29/render?currency=1";


                HttpWebResponse response = getResponseWithRetry(requestUrl);

                if (response.StatusCode != HttpStatusCode.OK)
                    throw new Exception(String.Format(
                    "Server error (HTTP {0}: {1}).",
                    response.StatusCode,
                    response.StatusDescription));
                Stream dataStream = response.GetResponseStream();
                dynamic jsonResponse = DeserializeFromStream(dataStream);
                var listings = jsonResponse["listinginfo"];
                double totalPrice;
                foreach (var property in listings)
                {
                    var listing = property.Value;
                    if (listing["converted_price"] == null || listing["converted_fee"] == null)
                    {
                        continue;
                    }

                    totalPrice = ((double)listing["converted_price"] + (double)listing["converted_fee"]) / 100;
                    prices.Add(totalPrice);
                }

                response.Dispose();

            }
            catch (Exception e)
            {


                Console.WriteLine(e.Message);
                return prices;
            }

            return prices;
        }

        private static HttpWebResponse getResponseWithRetry(string requestUrl)
        {
            // Baseline delay of 1 second
            int baselineDelayMs = 60000;

            const int MaxAttempts = 5;
            Random random = new Random();
            int attempt = 0;

            while (++attempt <= MaxAttempts)
            {
                try
                {
                    HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
                    request.Method = WebRequestMethods.Http.Get;
                    request.Accept = "application/json";
                    return request.GetResponse() as HttpWebResponse;
                }
                catch (Exception ex)
                {
                    if (attempt == MaxAttempts)
                    {
                        throw;
                    }

                    int delayMs = baselineDelayMs ;

                    Console.WriteLine("Exception caught:{0}", ex.Message);
                    Console.WriteLine("Retry attemt: {0}, Delay: {1}", attempt, delayMs);

                    Thread.Sleep(delayMs);

                    // Increment base-delay time
                    baselineDelayMs *= 2;
                }
            }

            throw new NotImplementedException();
        }

        public static dynamic DeserializeFromStream(Stream stream)
        {
            var serializer = new JsonSerializer();

            using (var sr = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                return serializer.Deserialize<dynamic>(jsonTextReader);
            }
        }
    }
}