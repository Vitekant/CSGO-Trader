using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace CsGoTrader
{
    public class SteamMarket
    {
        public static List<double> getPrices(string skinName, Quality quality)
        {
            return getPrices(
                string.Format(
                    @"http://steamcommunity.com/market/listings/730/AK-47%20%7C%20{0}%20%28{1}%29/render?currency=1",
                    skinName,
                    EnumUtil.qualityToString(quality)));
        }

        public static List<double> getPrices(string requestUrl)
        {
            List<double> prices = new List<double>();

            try
            {
                //string requestUrl = @"http://steamcommunity.com/market/listings/730/AK-47%20%7C%20Aquamarine%20Revenge%20%28Factory%20New%29/render?currency=1";

                HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
                request.Method = WebRequestMethods.Http.Get;
                request.Accept = "application/json";
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
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
                        totalPrice = ((double)listing["converted_price"] + (double)listing["converted_fee"]) / 100;
                        prices.Add(totalPrice);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }

            return prices;
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