using Microsoft.AspNetCore.Mvc;
using NET_WebScraping_API.Models;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace NET_WebScraping_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebScraperController : ControllerBase
    {
        const string offshoreUrl = "https://offshoreleaks.icij.org/";
        const string worldbankUrl = "https://projects.worldbank.org/en/projects-operations/procurement/debarred-firms";
        const string sanctionSearchUrl = "https://sanctionssearch.ofac.treas.gov/";

        private ChromeOptions _chromeOptions = new ChromeOptions();

        public PageOffshore pageOffshore = new PageOffshore(0, new List<DataOffshore>());
        public PageSanction pageSanction = new PageSanction(0, new List<DataSanction>());
        public PageWorldbank pageWorldbank = new PageWorldbank(0, new List<DataWorldbank>());
        public ResponseModel responseModel = new ResponseModel();


        [HttpGet(Name = "GetWebElements")]
        public async Task<IActionResult> GetWebElements(string searchQuery)
        {
            try
            {
                _chromeOptions.AddArgument("--headless");
                _chromeOptions.AddArgument("log-level=3");
                IWebDriver driver = new ChromeDriver(/*_chromeOptions*/);


                responseModel.pagOffshore = pageOffshore;
                responseModel.pagSanction = pageSanction;
                responseModel.pagWorldbank = pageWorldbank;

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
                wait.PollingInterval = TimeSpan.FromMilliseconds(200);

                //Busqueda en Offshore
                driver.Navigate().GoToUrl(offshoreUrl);

                //Buscar searchQuery en Offshore
                var searchBar = driver.FindElement(By.XPath("/html/body/div[3]/div[1]/div/form/input[1]"));
                var search = driver.FindElement(By.XPath("/html/body/div[3]/div[1]/div/form/div/button"));
                var statementAccept = driver.FindElement(By.XPath("//*[@id=\"accept\"]"));
                var statementSubmit = driver.FindElement(By.XPath("//*[@id=\"__BVID__73___BV_modal_body_\"]/form/div/div[2]/button"));
                string offshoreTableXpath = @$"//*[@id=""search_results""]/div/table/tbody/tr";

                statementAccept.Click();
                statementSubmit.Click();
                searchBar.SendKeys(@$"""{searchQuery}""");
                search.Click();

                // Esperar carga de elementos
                try
                {
                    wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(offshoreTableXpath)));
                }
                catch (WebDriverTimeoutException)
                {
                    Console.WriteLine("Elementos no encontrados en Offshore");
                }

                //Extrar resultados de busqueda
                var offshoreRows = driver.FindElements(By.XPath(offshoreTableXpath));

                for (int i = 0; i < offshoreRows.Count; i++)
                {
                    DataOffshore dataLine = new DataOffshore();
                    var rowColumnsOffshore = offshoreRows[i].FindElements(By.TagName("td"));

                    int j = 0;
                    foreach (var col in rowColumnsOffshore)
                    {
                        switch (j)
                        {
                            case 0:
                                dataLine.entity = col.Text;
                                break;
                            case 1:
                                dataLine.jurisdiction = col.Text;
                                break;
                            case 2:
                                dataLine.linkedTo = col.Text;
                                break;
                            case 3:
                                dataLine.dataFrom = col.Text;
                                break;
                        }
                        j++;
                    }
                    pageOffshore.response.Add(dataLine);
                }

                pageOffshore.count = offshoreRows.Count();

                //Busqueda Worldbank
                driver.Navigate().GoToUrl(worldbankUrl);

                // Esperar carga de elementos
                try
                {
                    wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(@"//*[@id=""k-debarred-firms""]/ div[2]/div/table/thead")));
                }
                catch (WebDriverTimeoutException)
                {
                    Console.WriteLine("Elementos no encontrados en Worldbank");
                }

                //Buscar searchQuery en Worldbank
                searchBar = driver.FindElement(By.XPath("//*[@id=\"category\"]"));
                searchBar.SendKeys(searchQuery);

                //Extrar resultados de busqueda
                var worldbankRows = driver.FindElements(By.XPath(@"//*[@id=""k-debarred-firms""]/ div[3]/table/tbody/tr"));

                for (int i = 0; i < worldbankRows.Count; i++)
                {
                    DataWorldbank dataLine = new DataWorldbank();
                    var rowColumnsWorldbank = worldbankRows[i].FindElements(By.TagName("td"));

                    int j = 0;
                    foreach (var col in rowColumnsWorldbank)
                    {
                        switch (j)
                        {
                            case 0:
                                dataLine.firstName = col.Text;
                                break;
                            case 1:
                                dataLine.address = col.Text;
                                break;
                            case 2:
                                dataLine.country = col.Text;
                                break;
                            case 3:
                                dataLine.periodFrom = col.Text;
                                break;
                            case 4:
                                dataLine.periodTo = col.Text;
                                break;
                            case 5:
                                dataLine.grounds = col.Text;
                                break;
                        }
                        j++;
                    }
                    pageWorldbank.response.Add(dataLine);
                }

                pageWorldbank.count = worldbankRows.Count();

                //Busqueda Sanction
                driver.Navigate().GoToUrl(sanctionSearchUrl);

                //Buscar searchQuery en Sanctions
                searchBar = driver.FindElement(By.XPath("//*[@id=\"ctl00_MainContent_txtLastName\"]"));
                search = driver.FindElement(By.XPath("//*[@id=\"ctl00_MainContent_btnSearch\"]"));
                searchBar.SendKeys(searchQuery);
                search.Click();

                //Extrar resultados de busqueda
                var SanctionRows = driver.FindElements(By.XPath(@"//*[@id=""gvSearchResults""]/tbody/tr"));

                for (int i = 0; i < SanctionRows.Count; i++)
                {
                    DataSanction dataLine = new DataSanction();
                    var rowColumnsSanction = SanctionRows[i].FindElements(By.TagName("td"));

                    int j = 0;
                    foreach (var col in rowColumnsSanction)
                    {
                        switch (j)
                        {
                            case 0:
                                dataLine.name = col.Text;
                                break;
                            case 1:
                                dataLine.adress = col.Text;
                                break;
                            case 2:
                                dataLine.type = col.Text;
                                break;
                            case 3:
                                dataLine.programs = col.Text;
                                break;
                            case 4:
                                dataLine.list = col.Text;
                                break;
                            case 5:
                                dataLine.score = col.Text;
                                break;
                        }
                        j++;
                    }
                    pageSanction.response.Add(dataLine);
                }
                pageSanction.count = SanctionRows.Count();

                Console.WriteLine("responseModel.ToJson()");
                Console.WriteLine(responseModel.ToString());

                driver.Quit();
                return Ok(JsonConvert.SerializeObject(responseModel));
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "Fallo en el procesamiento del query");
            }
            
        }

    }
}
