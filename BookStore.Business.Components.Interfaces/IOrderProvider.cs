using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BookStore.Business.Entities;

namespace BookStore.Business.Components.Interfaces
{
    public interface IOrderProvider
    {
        void SubmitOrder(Order pOrder);
        List<Order> GetOrders(int pUserId);
        List<Order> GetAllOrders(int pUserId);
        void RequestDelivery(int pOrderId);
        void CancelOrder(int pOrderId);
    }
}
