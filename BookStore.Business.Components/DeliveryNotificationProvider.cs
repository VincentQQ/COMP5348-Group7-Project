﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BookStore.Business.Components.Interfaces;
using BookStore.Business.Entities;
using Microsoft.Practices.ServiceLocation;
using System.Transactions;

namespace BookStore.Business.Components
{
    public class DeliveryNotificationProvider : IDeliveryNotificationProvider
    {
        public IEmailProvider EmailProvider
        {
            get { return ServiceLocator.Current.GetInstance<IEmailProvider>(); }
        }

        public void NotifyDeliveryCompletion(Guid pDeliveryId, Entities.DeliveryStatus status)
        {
            Console.WriteLine(DateTime.Now.ToString() + "   + Notifying Delivery " + pDeliveryId.ToString());
            Order lAffectedOrder = RetrieveDeliveryOrder(pDeliveryId);
            UpdateDeliveryStatus(pDeliveryId, status);
            if (status == Entities.DeliveryStatus.Delivered)
            {
                EmailProvider.SendMessage(new EmailMessage()
                {
                    ToAddress = lAffectedOrder.Customer.Email,
                    Message = "Our records show that your order " +lAffectedOrder.OrderNumber + " has been delivered. Thank you for shopping at video store"
                });
            }
            if (status == Entities.DeliveryStatus.Failed)
            {
                EmailProvider.SendMessage(new EmailMessage()
                {
                    ToAddress = lAffectedOrder.Customer.Email,
                    Message = "Our records show that there was a problem " + lAffectedOrder.OrderNumber + " delivering your order. Please contact Book Store"
                });
            }
            if (status == Entities.DeliveryStatus.PickedUp)
            {
                EmailProvider.SendMessage(new EmailMessage()
                {
                    ToAddress = lAffectedOrder.Customer.Email,
                    Message = "Our records show that your order " + lAffectedOrder.OrderNumber + " has been picked up from the warehouse. It will be delivered soon"
                });
            }
            if (status == Entities.DeliveryStatus.OnTheWay)
            {
                EmailProvider.SendMessage(new EmailMessage()
                {
                    ToAddress = lAffectedOrder.Customer.Email,
                    Message = "Our records show that your order " + lAffectedOrder.OrderNumber + " is on the way. It will arrive soon"
                });
            }
        }

        private void UpdateDeliveryStatus(Guid pDeliveryId, DeliveryStatus status)
        {
            using (TransactionScope lScope = new TransactionScope())
            using (BookStoreEntityModelContainer lContainer = new BookStoreEntityModelContainer())
            {
                Delivery lDelivery = lContainer.Deliveries.Where((pDel) => pDel.ExternalDeliveryIdentifier == pDeliveryId).FirstOrDefault();
                if (lDelivery != null)
                {
                    lDelivery.DeliveryStatus = status;
                    lContainer.SaveChanges();
                }
                Console.WriteLine(DateTime.Now.ToString() + "   + Update Delivery " + pDeliveryId.ToString() + " to status-" + status.ToString());
                lScope.Complete();
            }
        }

        private Order RetrieveDeliveryOrder(Guid pDeliveryId)
        {
 	        using(BookStoreEntityModelContainer lContainer = new BookStoreEntityModelContainer())
            {
                Delivery lDelivery =  lContainer.Deliveries.Include("Order.Customer").Where((pDel) => pDel.ExternalDeliveryIdentifier == pDeliveryId).FirstOrDefault();
                return lDelivery.Order;
            }
        }
    }


}
