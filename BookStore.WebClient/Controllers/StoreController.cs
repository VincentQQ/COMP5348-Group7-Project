using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookStore.WebClient.ViewModels;

namespace BookStore.WebClient.Controllers
{
    public class StoreController : Controller
    {
        // GET: Store
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListBooks()
        {
            return View(new CatalogueViewModel());
        }

        public ActionResult OrderHistory(UserCache pUser)
        {
            return View(new OrderHistoryModel(pUser.Model));
        }

        public RedirectToRouteResult CancelOrder(int pOrderId, string pReturnUrl)
        {
            ServiceFactory.Instance.OrderService.CancelOrder(pOrderId);
            Console.WriteLine(DateTime.Now.ToString() + "   Cancel Order "+pOrderId);
            return RedirectToAction("OrderHistory", new { pReturnUrl });
        }

        public RedirectToRouteResult RequestDelivery(int pOrderId, string pReturnUrl)
        {
            ServiceFactory.Instance.OrderService.RequestDelivery(pOrderId);
            Console.WriteLine(DateTime.Now.ToString() + "   Request Delivery " + pOrderId);
            return RedirectToAction("OrderHistory", new { pReturnUrl });
        }
    }
}