
using OpenQA.Selenium;
using NUnit.Framework;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;


namespace AutomationSelenium
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void NUnitSeleniumTest()
        {


            //string driverPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\")); ;
            //IWebDriver chrome = new ChromeDriver(driverPath + "\\Drivers");
            new DriverManager().SetUpDriver(new ChromeConfig());

            IWebDriver chrome  = new ChromeDriver();
            chrome.Manage().Window.Maximize();
            chrome.Url = "https://www.linkedin.com/login";


            IJavaScriptExecutor js = (IJavaScriptExecutor)chrome;
            DefaultWait<IWebDriver> wait =new DefaultWait<IWebDriver>(chrome);
            wait.Timeout = TimeSpan.FromSeconds(5);

            chrome.FindElement(By.Id("username")).SendKeys("your-email@example.com");
            chrome.FindElement(By.Id("password")).SendKeys("your-password");
            chrome.FindElement(By.XPath("//button[@type='submit']")).Click();

            wait.Timeout = TimeSpan.FromSeconds(5);

            for (int i = 0; i < 5; i++)
            {
                js.ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
                wait.Timeout = TimeSpan.FromSeconds(5);
            }
            var postLinks = new List<string>();
            var anchor = chrome.FindElements(By.TagName("a"));

            foreach (var urls in anchor)
            {
                var hrefs = urls.GetAttribute("href");
                var postUrlFormat = "linkedin.com/feed/update/urn:li:activity:";
                if (!string.IsNullOrEmpty(hrefs) && hrefs.Contains(postUrlFormat))
                {
                    postLinks.Add(hrefs.Split("?")[0]);
                }
            }
        var commentDictionary = new List<string>();

            foreach (var url in postLinks)
            {
                chrome.Navigate().GoToUrl(url);
                wait.Timeout = TimeSpan.FromSeconds(5);

            }
                for (int i = 0; i < 6; i++)
                {
                    chrome.FindElement(By.TagName("Body")).SendKeys("End");
                }

                try
                {

                    var moreCommentButton = chrome.FindElements(By.XPath("//button[contains(text(), 'Load more comments')]"));
                    if (moreCommentButton != null)
                    {

                        foreach (var button in moreCommentButton)
                        {
                            js.ExecuteScript("arguments[0].click(); ");
                            wait.Timeout = TimeSpan.FromSeconds(5);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                var comments = chrome.FindElements(By.XPath("//span[@dir='ltr']"));
                foreach (var comment in comments)
                {
                    var text = comment.Text.Trim();
                    if (text is not null)
                    {
                    commentDictionary.Add(text);
                    }
                }
           
            chrome.Quit();

            Assert.Pass();
        }

        public async Task LoadMoreComments(IWebDriver chrome, IJavaScriptExecutor js, DefaultWait<IWebDriver> wait)
        {
            try
            {

                var moreCommentButton = chrome.FindElements(By.XPath("//button[contains(text(), 'Load more comments')]"));
                if (moreCommentButton != null)
                {
                    return;
                }
                foreach (var button in moreCommentButton)
                {
                    js.ExecuteScript("arguments[0].click(); ");
                    wait.Timeout = TimeSpan.FromSeconds(5);
                }
            }
            catch(Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }

    }
}