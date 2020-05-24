using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BookStore.Services.Interfaces;
using BookStore.Business.Components.Interfaces;
using Microsoft.Practices.ServiceLocation;
using BookStore.Services.MessageTypes;

using System.ServiceModel;

namespace BookStore.Services
{
    public class OrderService : IOrderService
    {

        private IOrderProvider OrderProvider
        {
            get
            {
                return ServiceFactory.GetService<IOrderProvider>();
            }
        }

        public void SubmitOrder(Order pOrder)
        {
            try
            {
                OrderProvider.SubmitOrder(
                    MessageTypeConverter.Instance.Convert<
                    BookStore.Services.MessageTypes.Order,
                    BookStore.Business.Entities.Order>(pOrder)
                );
            }
            catch(BookStore.Business.Entities.InsufficientStockException ise)
            {
                throw new FaultException<InsufficientStockFault>(
                    new InsufficientStockFault() { ItemName = ise.ItemName });
            }
        }

        public List<Order> GetOrders(int pUserId)
        {
            var internalResult = OrderProvider.GetOrders(pUserId);
            var externalResult = MessageTypeConverter.Instance.Convert<
                List<BookStore.Business.Entities.Order>,
                List<BookStore.Services.MessageTypes.Order>>(internalResult);
            return externalResult;
        }

        public List<Order> GetAllOrders(int pUserId)
        {
            var internalResult = OrderProvider.GetAllOrders(pUserId);
            var externalResult = MessageTypeConverter.Instance.Convert<
                List<BookStore.Business.Entities.Order>,
                List<BookStore.Services.MessageTypes.Order>>(internalResult);
            return externalResult;
        }

        public void RequestDelivery(int pOrderId)
        {
            OrderProvider.RequestDelivery(pOrderId);
        }

        public void CancelOrder(int pOrderId)
        {
            OrderProvider.CancelOrder(pOrderId);
        }
    }
}
