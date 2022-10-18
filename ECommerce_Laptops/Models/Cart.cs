using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECommerce_Laptops.Models
{
    public class Cart
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public float Price { get; set; }

        public int qty { get; set; }    

        public float bill { get; set; }
    }
}