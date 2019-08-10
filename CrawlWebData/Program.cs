using HtmlAgilityPack;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using CrawlWebData.Models;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
namespace CrawlWebData
{
    class Program
    {
        static void Main(string[] args)
        {
            int page = 10;
            string url = "https://tiki.vn/laptop/c8095?filter_laptop_price=8.000.000%2B-%2B10.000.000&src=c.8095.hamburger_menu_fly_out_banner&page=";
            Console.WriteLine("------------------- Start running CrawlWebData");
            executeCrawlAsync(url, page);
            Console.WriteLine("------------------- End running CrawlWebData");
            Console.ReadKey();
        }

        public static async Task executeCrawlAsync(string url, int page = 1)
        {
            try
            {
                List<ProductModel> prods = new List<ProductModel>();
                var httpClient = new HttpClient();
                for(int i = 1; i <= page; i++)
                {
                    var html = await httpClient.GetStringAsync($"{url}{page}");
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(html);
                    var divContents = htmlDoc.DocumentNode.Descendants("div").Where(node => node.GetAttributeValue("class", "").Contains("product-item")).ToList();
                    foreach (var item in divContents)
                    {
                        ProductModel product = new ProductModel()
                        {
                            product_name = item.Descendants("a").FirstOrDefault().ChildAttributes("title").FirstOrDefault().Value,
                            url = item.Descendants("a").FirstOrDefault().ChildAttributes("href").FirstOrDefault().Value,
                            image_url = item.Descendants("img").FirstOrDefault().ChildAttributes("src").FirstOrDefault().Value,
                            price_sale = item.Descendants("span").Where(node => node.GetAttributeValue("class", "").Contains("final-price")).FirstOrDefault().InnerText.Replace("&nbsp;", "").Replace("₫", "").Replace("\n", "").Split('-')[0].Trim(),
                            price = item.Descendants("span").Where(node => node.GetAttributeValue("class", "").Contains("price-regular")).FirstOrDefault().InnerText.Replace("&nbsp;", "").Replace("₫", "").Replace("\n", "").Trim(),
                            price_sale_persent = item.Descendants("span").Where(node => node.GetAttributeValue("class", "").Contains("sale-tag sale-tag-square")).FirstOrDefault().InnerText.Replace("-", "").Replace("%", ""),
                        };
                        prods.Add(product);
                    }
                }
                // save to file
                if (prods.Count > 0)
                {
                    if (!Directory.Exists("data"))
                    {
                        Directory.CreateDirectory("data");
                        File.WriteAllText($"data/data_{DateTime.Now.ToString("ddMMyyHHmmss")}.json", JsonConvert.SerializeObject(prods));
                    }
                    else
                        File.WriteAllText($"data/data_{DateTime.Now.ToString("ddMMyyHHmmss")}.json", JsonConvert.SerializeObject(prods));
                }
            }
            catch(Exception ex)
            {

            }
        }
    }
}
