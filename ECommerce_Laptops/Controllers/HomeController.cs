using ECommerce_Laptops.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECommerce_Laptops.Controllers
{
    public class HomeController : Controller
    {
        EcommerceEntities db = new EcommerceEntities();
        public ActionResult Index()
        {
            Session["UserId"] = 1;
            if (TempData["Cart"] != null)
            {
                float sum = 0;
                List<Cart> list2 = TempData["Cart"] as List<Cart>;
                foreach (var item in list2) {

                    sum += item.bill;
                }
                TempData["total"] = sum;

            }
            TempData.Keep();
            return View(db.Products.OrderByDescending(x=>x.ProductId).ToList());
        }

        public ActionResult Addtocart(int? Id)
        {

            Product prod = db.Products.Where(x => x.ProductId == Id).SingleOrDefault();
            return View(prod);
        }

        List<Cart> cartlist = new List<Cart>();
        [HttpPost]
        public ActionResult Addtocart(Product P, string qty,int Id)
        {

            Product prod = db.Products.Where(x => x.ProductId == Id).SingleOrDefault();
            Cart c = new Cart();
            c.ProductId = prod.ProductId;
            c.ProductName = prod.ProductName;
            c.Price = (float)prod.ProductPrice;
            c.qty = Convert.ToInt32(qty);
            c.bill = c.Price * c.qty;
            if (TempData["Cart"] == null)
            {
                cartlist.Add(c);

                TempData["Cart"] = cartlist;

            }
            else
            {
                List<Cart> list2 = TempData["Cart"] as List<Cart>;
                int flag = 0;
                foreach(var item in list2)
                {
                    if(item.ProductId == c.ProductId)
                    {
                        item.qty +=c.qty;
                        item.bill += c.bill;
                        flag = 1;
                    }
                }
                if (flag == 0)
                {
                    list2.Add(c);

                }
                
                TempData["Cart"] = list2;

            }
            
            TempData.Keep();

            return RedirectToAction("Index");

        }

        public ActionResult CheckOut()
        {

            TempData.Keep();
            return View();
        }
        [HttpPost]
        public ActionResult CheckOut(Order odr)
        {

            List<Cart> cart = TempData["Cart"] as List<Cart>;
            Invoice Fi = new Invoice();
            Fi.Inv_User = Convert.ToInt32(Session["UserId"].ToString());
            Fi.InvoiceDate = System.DateTime.Now;
            Fi.TotalBill = (float)TempData["total"];
            db.Invoices.Add(Fi);
            db.SaveChanges();
            foreach (var item in cart)
            {
                Order o = new Order();
                o.ProductId= item.ProductId;
                o.InvoiceNo = Fi.InvoiceId;
                o.OrderDate = System.DateTime.Now;
                o.OrderQty = item.qty;
                o.OrderUnitPrice = (int)item.Price;
                o.OrderBill = item.bill;
                db.Orders.Add(o);
                db.SaveChanges();
            }
            TempData.Remove("total");
            TempData.Remove("Cart");

            TempData["msg"] = "Laptop Ordered Successfully";
            TempData.Keep();


            return RedirectToAction("Index");
        }

        public ActionResult remove(int? id)
        {
            List<Cart> cart = TempData["Cart"] as List<Cart>;
            Cart c = cart.Where(X => X.ProductId == id).SingleOrDefault();
            cart.Remove(c);
            float a = 0;
            foreach (var item in cart)
            {
                a += item.bill;
            }
            TempData["total"] = a;
            return RedirectToAction("CheckOut");
        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}