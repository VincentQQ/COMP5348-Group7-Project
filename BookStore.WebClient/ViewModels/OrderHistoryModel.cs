using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BookStore.Services.Interfaces;
using BookStore.Services.MessageTypes;

namespace BookStore.WebClient.ViewModels
{
    public class OrderHistoryModel
    {
        public OrderHistoryModel(User pUser)
        {
            UserId = pUser.Id;
            isAdmin = pUser.Id == 1;
        }

        private IOrderService OrderService
        {
            get
            {
                return  ServiceFactory.Instance.OrderService;
            }
        }

        public int UserId { get; set; }
        
        public bool isAdmin { get; set; }

        public List<Order> Orders
        {
            get
            {
                if (isAdmin)
                {
                    return OrderService.GetAllOrders(UserId);
                } else
                {
                    return OrderService.GetOrders(UserId);
                }
            }
        }
    }
}