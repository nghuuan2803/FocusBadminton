using OpenQA.Selenium;

namespace Test
{
    [TestFixture]

    public class Tests
    {
        IWebDriver driver;

        [SetUp]
        public void Setup()
        {
            driver = new OpenQA.Selenium.Chrome.ChromeDriver();
            driver.Url = "http://thanhnien.vn";
            driver.Navigate();
            Thread.Sleep(3000);
        }

        [Test]
        public void Test1()
        {
            var logo = driver.FindElement(By.ClassName("logothuong"));
            Assert.IsNotNull(logo);
        }
        [TearDown]
        public void Close()
        {
            driver.Quit();
            driver.Dispose();
        }
    }
}