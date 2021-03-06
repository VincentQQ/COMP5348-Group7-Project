﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using BookStore.Services.MessageTypes;

namespace BookStore.Services.Interfaces
{
    [ServiceContract]
    public interface IOrderService
    {
        [OperationContract]
        [FaultContract(typeof(InsufficientStockFault))]
        void SubmitOrder(Order pOrder);
        [OperationContract]
        List<Order> GetOrders(int pUserId);
        [OperationContract]
        List<Order> GetAllOrders(int pUserId);
        [OperationContract]
        void RequestDelivery(int pOrderId);
        [OperationContract]
        void CancelOrder(int pOrderId);
    }
}
