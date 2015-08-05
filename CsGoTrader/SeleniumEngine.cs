using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace CsGoTrader
{
    public class SeleniumEngine
    {
        FirefoxDriver driver;

        public SeleniumEngine()
        {
            
        }

        public void initializeWebDriver()
        {
            var path = @"C:\Users\Witold\AppData\Roaming\Mozilla\Firefox\Profiles\243mda9k.Selenium";
            var ffProfile = new FirefoxProfile(path, false);
            driver = new FirefoxDriver(ffProfile);
            driver.Navigate().GoToUrl("https://steamcommunity.com/login/home/?goto=0");
            Thread.Sleep(1000);
            var userNameField = driver.FindElementById("steamAccountName");
            var userPasswordField = driver.FindElementById("steamPassword");
            var loginButton = driver.FindElementById("SteamLogin");
            userNameField.SendKeys(SteamCredentials.login);
            userPasswordField.SendKeys(SteamCredentials.password);
            loginButton.Click();
            Thread.Sleep(1000);
        }

        internal void deinitializeDriver()
        {
            driver.Dispose();
        }

        public void buySkins(List<Tuple<Skin, Quality, double, int>> skinsToBuy)
        {
            Dictionary<Tuple<Skin, Quality, double, int>, int> skinsBought = new Dictionary<Tuple<Skin, Quality, double, int>, int>();
            foreach(var order in skinsToBuy)
            {
                skinsBought.Add(order, 0);
            }

            while(skinsToBuy.Count != 0)
            {
                foreach(var order in skinsToBuy)
                {
                    var url = getMarketUrl(order.Item1.name, order.Item2);
                    if (buyAttempt(url, order.Item3))
                    {
                        skinsBought[order]++;
                    }

                    Thread.Sleep(5000);
                }

                skinsToBuy = skinsToBuy.Where(order => order.Item4 > skinsBought[order]).ToList();
                Thread.Sleep(15000);
            }
        }

        public void buySkin(Skin skin, Quality quality, double priceLimit, int skinsToBuy)
        {

            var url = getMarketUrl(skin.name, quality);
            int skinsBought = 0;

            while(skinsBought < skinsToBuy)
            {
                driver.Navigate().GoToUrl(url);
                var results = driver.FindElementById("searchResultsRows");
                string expr = "//span[@class='market_listing_price market_listing_price_with_fee']";
                //results.FindElement(By.ClassName("market_listing_price_with_fee"));
                var listings = driver.FindElementsByClassName("market_recent_listing_row");
                double price = -1;
                foreach(var listing in listings)
                {
                    var priceElement = listing.FindElement(By.ClassName("market_listing_price_with_fee"));
                    var priceString = priceElement.Text;
                    priceString = Regex.Replace(priceString, "[^0-9.,]", "");

                    if (string.IsNullOrWhiteSpace(priceString))
                    {
                        continue;
                    }

                    if (double.TryParse(priceString, out price))
                    {
                        break;
                    }
                }

                if(!(price > -1))
                {
                    continue;
                }

                try{
                    if (price <= priceLimit)
                    {
                        var button = listings.First().FindElement(By.XPath("//a[contains(@class,'item_market_action_button_green')]"));
                        button.Click();
                        if (IsElementPresent(By.Id("market_buynow_dialog_accept_ssa")))
                        {
                            var checkBox = driver.FindElementById("market_buynow_dialog_accept_ssa");
                            if (checkBox.Selected != true)
                            {
                                checkBox.Click();
                            }

                            var finalButton = driver.FindElementById("market_buynow_dialog_purchase");
                            finalButton.Click();   

                            if (IsElementPresentWithRetry(By.Id("market_buynow_dialog_purchasecomplete_message"), 15))
                            {
                                Console.WriteLine("{0}:Item purchased for: {1}", DateTime.Now, price);
                                skinsBought++;
                            }
                            else
                            {
                                //check visibility div id = market_buynow_dialog_error
                                Console.WriteLine("Purchase failed");
                            }
                        }
                        else
                        {
                            Console.WriteLine("{0}:Item probably purchased without rules confirmation for: {1}", DateTime.Now, price);

                            skinsBought++;
                        }
                    }
                    else
                    {
                        Console.WriteLine(string.Format("{0}: Available price: {1} is higher that required price: {2}", DateTime.Now, price, priceLimit));
                        Thread.Sleep(15000);
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(string.Format("{1}: Exception encountered {0}", ex, DateTime.Now));
                }

            }
        }


        private bool IsElementPresent(By by)
        {
            try
            {
                var element = driver.FindElement(by);
                if (element.Displayed)
                {
                    return true;
                }

                return false;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        private bool IsElementPresentWithRetry(By by, int seconds)
        {
            for(int i = 0; i < seconds; i++)
            {
                try
                {
                    var element = driver.FindElement(by);
                    if (element.Displayed)
                    {
                        return true;

                    }
                }
                catch (NoSuchElementException){}

                Thread.Sleep(1000);
            }

            return false;
        }

        public string getMarketUrl(string skinName, Quality quality)
        {
            return string.Format(
                    @"http://steamcommunity.com/market/listings/730/{0}%20%28{1}%29",
                    skinName,
                    EnumUtil.qualityToString(quality));
        }

        public bool buyAttempt(string url, double priceLimit)
        {
            driver.Navigate().GoToUrl(url);
            if (!IsElementPresent(By.Id("searchResultsRows"))){
                Thread.Sleep(15000);
                return false;
            }

            var results = driver.FindElementById("searchResultsRows");
            var listings = driver.FindElementsByClassName("market_recent_listing_row");
            double price = -1;
            foreach (var listing in listings)
            {
                var priceElement = listing.FindElement(By.ClassName("market_listing_price_with_fee"));
                var priceString = priceElement.Text;
                priceString = Regex.Replace(priceString, "[^0-9.,]", "");

                if (string.IsNullOrWhiteSpace(priceString))
                {
                    continue;
                }

                if (double.TryParse(priceString, out price))
                {
                    break;
                }
            }

            if (!(price > -1))
            {
                return false;
            }

            try
            {
                if (price <= priceLimit)
                {
                    var button = listings.First().FindElement(By.XPath("//a[contains(@class,'item_market_action_button_green')]"));
                    button.Click();
                    if (IsElementPresent(By.Id("market_buynow_dialog_accept_ssa")))
                    {
                        var checkBox = driver.FindElementById("market_buynow_dialog_accept_ssa");
                        if (checkBox.Selected != true)
                        {
                            checkBox.Click();
                        }

                        var finalButton = driver.FindElementById("market_buynow_dialog_purchase");
                        finalButton.Click();

                        if (IsElementPresentWithRetry(By.Id("market_buynow_dialog_purchasecomplete_message"), 15))
                        {
                            Console.WriteLine("{0}:Item purchased for: {1}", DateTime.Now, price);
                            return true;
                        }
                        else
                        {
                            //check visibility div id = market_buynow_dialog_error
                            Console.WriteLine("Purchase failed");
                        }
                    }
                    else
                    {
                        Console.WriteLine("{0}:Item probably purchased without rules confirmation for: {1}", DateTime.Now, price);

                        return true;
                    }
                }
                else
                {
                    Console.WriteLine(string.Format("{0}: Available price: {1} is higher that required price: {2}", DateTime.Now, price, priceLimit));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("{1}: Exception encountered {0}", ex, DateTime.Now));
            }

            return false;
        }
    }
}
