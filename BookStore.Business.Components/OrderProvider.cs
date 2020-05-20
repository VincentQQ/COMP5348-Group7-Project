using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BookStore.Business.Components.Interfaces;
using BookStore.Business.Entities;
using System.Transactions;
using Microsoft.Practices.ServiceLocation;
using DeliveryCo.MessageTypes;
using System.Data.Entity;

namespace BookStore.Business.Components
{
    public class OrderProvider : IOrderProvider
    {
        public IEmailProvider EmailProvider
        {
            get { return ServiceLocator.Current.GetInstance<IEmailProvider>(); }
        }

        public IUserProvider UserProvider
        {
            get { return ServiceLocator.Current.GetInstance<IUserProvider>(); }
        }

        public void SubmitOrder(Entities.Order pOrder)
        {
            using (TransactionScope lScope = new TransactionScope())
            {
                //LoadBookStocks(pOrder);
                //MarkAppropriateUnchangedAssociations(pOrder);

                using (BookStoreEntityModelContainer lContainer = new BookStoreEntityModelContainer())
                {
                    try
                    {
                        int lUserId = pOrder.Customer.Id;
                        pOrder.OrderNumber = Guid.NewGuid();
                        pOrder.Store = "OnLine";

                        User lCustomer = lContainer.Users.Where(user => user.Id == lUserId).First();

                        pOrder.Customer = lCustomer;

                        // Book objects in pOrder are missing the link to their Stock tuple (and the Stock GUID field)
                        // so fix up the 'books' in the order with well-formed 'books' with 1:1 links to Stock tuples
                        foreach (OrderItem lOrderItem in pOrder.OrderItems)
                        {
                            int bookId = lOrderItem.Book.Id;
                            lOrderItem.Book = lContainer.Books.Where(book => bookId == book.Id).First();
                            List<int> stockIds = lOrderItem.Book.Stocks.Select(stock => stock.Id).ToList();
                            lOrderItem.Book.Stocks = lContainer.Stocks.Where(stock => stockIds.Contains(stock.Id)).ToList();
                        }
                        // and update the stock levels
                        List<UsedStock> lUsedStocks = pOrder.UpdateStockLevels();
                        foreach (var used in lUsedStocks)
                        {
                            used.Stock.Quantity -= used.Quantity;
                        }

                        // add the modified Order tree to the Container (in Changed state)
                        lContainer.Orders.Add(pOrder);
                        lContainer.UsedStocks.AddRange(lUsedStocks);

                        // ask the Bank service to transfer funds
                        TransferFundsFromCustomer(UserProvider.ReadUserById(pOrder.Customer.Id).BankAccountNumber, pOrder.Total ?? 0.0);

                        // and save the order
                        lContainer.SaveChanges();
                        SendOrderPlacedConfirmation(pOrder);

                        lScope.Complete();
                    }
                    catch (Exception lException)
                    {
                        SendOrderErrorMessage(pOrder, lException);
                        Console.WriteLine(DateTime.Now.ToString() + "    An exception occured: " + lException.Message);
                        IEnumerable<System.Data.Entity.Infrastructure.DbEntityEntry> entries = lContainer.ChangeTracker.Entries();
                        throw;
                    }
                }
            }
            // OrderPostProcessing(pOrder);
        }

        public void CancelOrder(int pOrderId)
        {
            using (TransactionScope lScope = new TransactionScope())
            {
                using (BookStoreEntityModelContainer lContainer = new BookStoreEntityModelContainer())
                {
                    Order lOrder = lContainer.Orders.Where(order => order.Id == pOrderId).FirstOrDefault();

                    if (lOrder != null && lOrder.Delivery == null)
                    {
                        foreach (var item in lOrder.OrderItems)
                        {
                            foreach (var used in item.UsedStocks)
                            {
                                used.Stock.Quantity += used.Quantity;
                            }
                            lContainer.UsedStocks.RemoveRange(item.UsedStocks);
                        }
                        lContainer.OrderItems.RemoveRange(lOrder.OrderItems);
                        RefundCustomer(lOrder.Customer.BankAccountNumber, lOrder.Total ?? 0.0);
                        lContainer.Orders.Remove(lOrder);
                    }

                    lContainer.SaveChanges();
                    lScope.Complete();
                }
            }
        }

        public void RequestDelivery(int pOrderId)
        {
            using (TransactionScope lScope = new TransactionScope())
            {
                using (BookStoreEntityModelContainer lContainer = new BookStoreEntityModelContainer())
                {
                    try
                    {
                        Order lOrder = lContainer.Orders.Where(order => order.Id == pOrderId).FirstOrDefault();

                        if (lOrder !=  null)
                        { 
                            // ask the delivery service to organise delivery
                            PlaceDeliveryForOrder(lOrder);
                        }

                        // SendOrderPlacedConfirmation(pOrder);
                        lContainer.SaveChanges();
                        lScope.Complete();
                    }
                    catch (Exception lException)
                    {
                        Console.WriteLine(DateTime.Now.ToString() + "    An exception occured: " + lException.Message);
                    }
                }
            }
        }

        public List<Order> GetOrders(int pUserId)
        {
            using (BookStoreEntityModelContainer lContainer = new BookStoreEntityModelContainer())
            {
                return (from Order in lContainer.Orders.Include("Customer")
                        .Include("Delivery")
                        .Include("OrderItems")
                        .Include("OrderItems.Book")
                        .Include("OrderItems.Book.Stocks")
                        .Include("Warehouses")
                        .Include("Customer.LoginCredential")
                        .Include("Warehouses")
                        where Order.Customer.Id == pUserId
                        orderby Order.OrderDate
                        select Order).ToList();
            }
        }

        public List<Order> GetAllOrders(int pUserId)
        {
            using (BookStoreEntityModelContainer lContainer = new BookStoreEntityModelContainer())
            {
                return (from Order in lContainer.Orders.Include("Customer")
                        .Include("Delivery")
                        .Include("OrderItems")
                        .Include("OrderItems.Book")
                        .Include("OrderItems.Book.Stocks")
                        .Include("Warehouses")
                        .Include("Customer.LoginCredential")
                        .Include("Warehouses")
                        orderby Order.OrderDate
                        select Order).ToList();
            }
        }

        //private void MarkAppropriateUnchangedAssociations(Order pOrder)
        //{
        //    pOrder.Customer.MarkAsUnchanged();
        //    pOrder.Customer.LoginCredential.MarkAsUnchanged();
        //    foreach (OrderItem lOrder in pOrder.OrderItems)
        //    {
        //        lOrder.Book.Stock.MarkAsUnchanged();
        //        lOrder.Book.MarkAsUnchanged();
        //    }
        //}

        private void LoadBookStocks(Order pOrder)
        {
            using (BookStoreEntityModelContainer lContainer = new BookStoreEntityModelContainer())
            {
                foreach (OrderItem lOrderItem in pOrder.OrderItems)
                {
                    lOrderItem.Book.Stocks = lContainer.Stocks.Where((pStock) => pStock.Book.Id == lOrderItem.Book.Id).ToList();    
                }
            }
        }

        private void SendOrderErrorMessage(Order pOrder, Exception pException)
        {
            EmailProvider.SendMessage(new EmailMessage()
            {
                ToAddress = pOrder.Customer.Email,
                Message = "There was an error in processsing your order " + pOrder.OrderNumber + ": "+ pException.Message + ". Please contact Book Store"
            });
        }

        private void SendOrderPlacedConfirmation(Order pOrder)
        {
            EmailProvider.SendMessage(new EmailMessage()
            {
                ToAddress = pOrder.Customer.Email,
                Message = "Your order " + pOrder.OrderNumber + " has been placed"
            });
        }

        public void PlaceDeliveryForOrder(Order pOrder)
        {
            using (TransactionScope lScope = new TransactionScope())
            {

                using (BookStoreEntityModelContainer lContainer = new BookStoreEntityModelContainer())
                {
                    Delivery lDelivery = new Delivery() { DeliveryStatus = DeliveryStatus.Submitted, SourceAddress = "Book Store Address", DestinationAddress = pOrder.Customer.Address, Order = pOrder };

                    Guid lDeliveryIdentifier = ExternalServiceFactory.Instance.DeliveryService.SubmitDelivery(new DeliveryInfo()
                    {
                        OrderNumber = lDelivery.Order.OrderNumber.ToString(),
                        SourceAddress = lDelivery.SourceAddress,
                        DestinationAddress = lDelivery.DestinationAddress,
                        DeliveryNotificationAddress = "net.tcp://localhost:9010/DeliveryNotificationService"
                    });

                    lDelivery.ExternalDeliveryIdentifier = lDeliveryIdentifier;
                    pOrder.Delivery = lDelivery;

                    // and save the order
                    lContainer.SaveChanges();
                    lScope.Complete();
                }
            }
        }

        private void TransferFundsFromCustomer(int pCustomerAccountNumber, double pTotal)
        {
            try
            {
                ExternalServiceFactory.Instance.TransferService.Transfer(pTotal, pCustomerAccountNumber, RetrieveBookStoreAccountNumber());
            }
            catch
            {
                throw new Exception("Error when transferring funds for order.");
            }
        }

        private void RefundCustomer(int pCustomerAccountNumber, double pTotal)
        {
            try
            {
                ExternalServiceFactory.Instance.TransferService.Transfer(pTotal, RetrieveBookStoreAccountNumber(), pCustomerAccountNumber);
            }
            catch
            {
                throw new Exception("Error when transferring funds for refund.");
            }
        }


        private int RetrieveBookStoreAccountNumber()
        {
            return 123;
        }


    }
}
