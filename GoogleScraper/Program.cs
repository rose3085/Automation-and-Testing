using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
class GoogleScraper
{
    static void Main(string[] args)
    {
        Console.Write("Enter your search query: ");
        string query = Console.ReadLine();

        // Setup Chrome Driver
        var options = new ChromeOptions();
        //options.AddArgument("--headless"); // run in background
        IWebDriver driver = new ChromeDriver(options);

        try
        {
            // Open Google
            driver.Navigate().GoToUrl("https://www.google.com");

            // Accept cookies if popup exists (optional)
            try
            {
                var agreeButton = driver.FindElement(By.Id("L2AGLb"));
                if (agreeButton.Displayed)
                    agreeButton.Click();
            }
            catch (NoSuchElementException) { }

            // Enter query
            var searchBox = driver.FindElement(By.Name("q"));
            searchBox.SendKeys(query);
            searchBox.Submit();

            Thread.Sleep(3000); // wait for results to load

            // Extract top 5 results
            var results = driver.FindElements(By.CssSelector("div.g")).Take(5).ToList();

            List<string> csvLines = new List<string>();
            csvLines.Add("Title,Link,Snippet");

            foreach (var result in results)
            {
                try
                {
                    string title = result.FindElement(By.CssSelector("h3")).Text.Replace(",", " ");
                    string link = result.FindElement(By.CssSelector("a")).GetAttribute("href");
                    string snippet = "";

                    try
                    {
                        snippet = result.FindElement(By.CssSelector("span.aCOpRe")).Text.Replace(",", " ");
                    }
                    catch { }

                    csvLines.Add($"{title},{link},{snippet}");
                }
                catch { }
            }

            // Save to CSV
            string filePath = "GoogleResults.csv";
            File.WriteAllLines(filePath, csvLines);

            Console.WriteLine($"✅ Data saved to {filePath}");
        }
        finally
        {
            driver.Quit();
        }
    }
}