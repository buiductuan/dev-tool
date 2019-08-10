using System;
using System.Collections.Generic;
using System.Text;

namespace CrawlWebData.Models
{
    class ProductModel
    {
        public string product_name { get; set; }
        public string price { get; set; }
        public string price_sale { get; set; }
        public string price_sale_persent { get; set; }
        public string url { get; set; }
        public string image_url { get; set; }
    }
}
